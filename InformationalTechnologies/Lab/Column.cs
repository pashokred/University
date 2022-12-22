namespace Lab;

public abstract class Column {
    public string Name { get; set; }
    public string Type { get; } = "";

    public Column(string name) {
        Name = name;
    }

    public abstract bool Validate(string value);
}

public class IntColumn : Column {
    public new string Type { get; } = "INT";
    public IntColumn(string name) : base(name) { }

    public override bool Validate(string value) => int.TryParse(value, out _);
}

public class RealColumn : Column {
    public new string Type { get; }  = "REAL";
    public RealColumn(string name) : base(name) { }

    public override bool Validate(string value) => double.TryParse(value, out _);
}

public class CharColumn : Column {
    public new string Type { get; } = "CHAR";
    public CharColumn(string name) : base(name) { }

    public override bool Validate(string value) => char.TryParse(value, out _);
}

public class StringColumn : Column {
    public new string Type { get; } = "STRING";
    public StringColumn(string name) : base(name) { }

    public override bool Validate(string value) => true;
}

public class TextFileColumn : Column {
    public new string Type { get; }  = "TEXT FILE";
    public TextFileColumn(string name) : base(name) { }

    public override bool Validate(string value) => value.ToLower().EndsWith(".txt") &&
                                                   File.Exists(value);
}

public class IntIntervalColumn : Column {
    public new string Type { get; } = "INT INTERVAL";
    public IntIntervalColumn(string name) : base(name) { }

    public override bool Validate(string value) {
        string[] buf = value.Replace(" ", "").Split(',');

        return buf.Length == 2 && int.TryParse(buf[0], out int a) &&
               int.TryParse(buf[1], out int b) && a < b;
    }
}

public class Row {
    public List<string> Values { get; set; } = new();

    public string this[int i] {
        get => Values[i];
        set => Values[i] = value;
    }
}

public abstract class DatabaseManager {
    private static DatabaseManager? _instance;
    public DB Database { get; set; }

    public static DatabaseManager? Instance => _instance ??= new DatabaseManager();
    private DatabaseManager() { }

    public bool ChangeCellValue(string value, int tableId, int i, int id) => true;
    public abstract void DeleteRow(int tableId, int length);
    
    public class DB
    {
        public List<Table> Tables { get; set; }
    }
    
    public class Table
    {
        public string Name { get; set; }
        public List<Row> Rows { get; set; }
        public List<Column> Columns { get; set; }
    }
}


