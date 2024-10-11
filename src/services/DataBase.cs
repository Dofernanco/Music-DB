using System.Data.SQLite;

public class Database
{
    private SQLiteConnection connection;

    public Database(string dbPath)
    {
        connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
        connection.Open();
    }

    public SQLiteConnection GetConnection()
    {
        return connection;
    }

    public void Initialize()
    {
        string createTables = @"
        CREATE TABLE IF NOT EXISTS performers (
            id_performer INTEGER PRIMARY KEY,
            id_type INTEGER,
            name TEXT
        );

        CREATE TABLE IF NOT EXISTS albums (
            id_album INTEGER PRIMARY KEY,
            path TEXT,
            name TEXT,
            year INTEGER
        );

        CREATE TABLE IF NOT EXISTS rolas (
            id_rola INTEGER PRIMARY KEY,
            id_performer INTEGER,
            id_album INTEGER,
            path TEXT,
            title TEXT,
            track INTEGER,
            year INTEGER,
            genre TEXT,
            FOREIGN KEY (id_performer) REFERENCES performers(id_performer),
            FOREIGN KEY (id_album) REFERENCES albums(id_album)
        );";

        SQLiteCommand command = new SQLiteCommand(createTables, connection);
        command.ExecuteNonQuery();
    }
}
