namespace LabWebApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Lab;

public class TableController : ControllerBase
{
    private readonly ILogger<TableController> _logger;
    private readonly DatabaseManager _dbManager;

    public TableController(ILogger<TableController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetTable")]
    public IEnumerable<TableController> Get()
    {
        return null;
    }
    
    /// <summary>
    /// Updates the row at specified index in the specified table
    /// </summary>
    /// <response code="200">_Updates the row at specified index in the specified table_</response>
    /// <response code="400">_Database is not created yet, row values' and columns' number don't match, or row values are of invalid type_</response>
    /// <response code="404">_No table with such name in the database or no row at such index in the table_</response>
    [HttpPut]
    [Route("{tableName}/Rows/{id}")]
    [ProducesResponseType(typeof(Response<Row>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 404)]
    public IActionResult Put(string tableName, int id, [FromBody] Row row) {
        if (_dbManager.Database == null) {
            return BadRequest(new { error = "Database is not created yet" });
        }

        var table = _dbManager.Database.Tables.Find(t => t.Name.Equals(tableName));
        if (table == null) {
            return NotFound(new { error = $"There is no table named {tableName} in the database" });
        }

        ;
        try {
            var oldRow = table.Rows[id];
        }
        catch {
            return NotFound(new { error = $"Theres is no row in the table named {tableName} at the specified index" });
        }

        if (row.Values.Count != table.Columns.Count) {
            return BadRequest(new { error = "Numbers of the row's values and the table's columns don't match" });
        }

        var tableId = _dbManager.Database.Tables.IndexOf(table);

        for (var i = 0; i < row.Values.Count; i++)
        {
            if (_dbManager.ChangeCellValue(row.Values[i], tableId, i, id)) continue;
            _dbManager.DeleteRow(tableId, table.Rows.Count - 1);
            return BadRequest(new { error = $"Value {row.Values[i]} is of invalid type" });
        }

        var response = new Response<Row> {
            Value = row,
            Links = new Dictionary<string, string> {
                { "updateRow", $"/Tables/{tableName}/Rows/{id}" },
                { "deleteRow", $"/Tables/{tableName}/Rows/{id}" }
            }
        };

        return Ok(response);
    }

}