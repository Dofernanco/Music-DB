using System;
using System.Data.SQLite;

public class PerformerController
{
    private Database db;

    public PerformerController(Database database)
    {
        db = database;
    }

    // Obtener todas las personas (no grupos) desde la base de datos
    public SQLiteDataReader GetAllPersons()
    {
        string query = "SELECT id_performer, name FROM performers WHERE id_type = 0"; // 0 es Persona
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        return command.ExecuteReader();
    }

    // Obtener el ID de una persona por nombre
    public int GetPersonIdByName(string name)
    {
        string query = "SELECT id_performer FROM performers WHERE name = @name AND id_type = 0";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@name", name);

        using (SQLiteDataReader reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }
        }

        return -1; // Si no se encuentra la persona
    }
}
