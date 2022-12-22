using System.Text;
using Lab;
using Microsoft.AspNetCore.Mvc;

namespace LabWebApplication.Controllers;

public class TextFilesController : Controller {
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save(string fileContents) {
        int asteriskIndex = fileContents.IndexOf('*');
        string fileName = fileContents.Substring(0, asteriskIndex);
        fileContents = fileContents.Substring(asteriskIndex + 1, fileContents.Length - asteriskIndex - 1);

        return File(Encoding.ASCII.GetBytes(fileContents), "text/plain", fileName);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Open(IFormFile fileTdb) {
        using (var stream = new FileStream(_tmpFilePath, FileMode.Create)) {
            fileTdb.CopyTo(stream);
        }

        _dbManager.OpenDatabase(_tmpFilePath);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save() {
        _dbManager.SaveDatabase(_tmpFilePath);
        
        var dbBytes = System.IO.File.ReadAllBytes(_tmpFilePath);
        var fileName = $"{_dbManager.Database.Name}.tdb";

        return File(dbBytes, "application/octet-stream", fileName);
    }

    public bool AddTable(string name) {
        if (GetTableNames().Contains(name)) {
            return false;
        }

        var table = new DatabaseManager.Table(Database.Tables.Count, name);
        table.Columns.Add(new IntColumn(0, "id"));
        Database.Tables.Add(table);
        _lastIds.Add(0);

        string query = $"CREATE TABLE {name} (id INT IDENTITY(1,1) PRIMARY KEY)";
        ExecuteSqlQuery(query, _connectionStr);

        return true;
    }

    public static string? SqlServerColumnType(string type) {
        return type switch {
            "INT" => "INT",
            "REAL" => "REAL",
            "CHAR" => "CHAR(1)",
            "STRING" => "NVARCHAR(MAX)",
            "TEXT FILE" => "NTEXT",
            "INT INTERVAL" => "VARCHAR(MAX)",
            _ => null
        };
    }
        
    public static string? ColumnType(string sqlServerType) {
        return sqlServerType switch {
            "int" => "INT",
            "real" => "REAL",
            "char" => "CHAR",
            "nvarchar" => "STRING",
            "ntext" => "TEXT FILE",
            "varchar" => "INT INTERVAL",
            _ => null
        };
    }
}
