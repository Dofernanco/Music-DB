using System;
using System.Data.SQLite;

public class GroupController
{
    private Database db;

    public GroupController(Database database)
    {
        db = database;
    }

    // Método para agregar un grupo a la base de datos
    public void AddGroup(string groupName, string startDate, string endDate)
    {
        string query = "INSERT INTO groups (name, start_date, end_date) VALUES (@name, @startDate, @endDate)";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@name", groupName);
        command.Parameters.AddWithValue("@startDate", startDate);
        command.Parameters.AddWithValue("@endDate", string.IsNullOrEmpty(endDate) ? (object)DBNull.Value : endDate);
        command.ExecuteNonQuery();

        Console.WriteLine("Grupo agregado a la base de datos.");
    }

    // Método para obtener todos los grupos
    public SQLiteDataReader GetAllGroups()
    {
        string query = "SELECT id_group, name, start_date, end_date FROM groups";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        return command.ExecuteReader();
    }

    // Método para agregar una persona a un grupo (Tabla in_group)
    public void AddPersonToGroup(int personId, int groupId)
    {
        string query = "INSERT INTO in_group (id_person, id_group) VALUES (@personId, @groupId)";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@personId", personId);
        command.Parameters.AddWithValue("@groupId", groupId);
        command.ExecuteNonQuery();

        Console.WriteLine("Persona agregada al grupo en la base de datos.");
    }

    // Método para obtener todas las personas de un grupo
    public SQLiteDataReader GetPeopleInGroup(int groupId)
    {
        string query = @"
        SELECT p.stage_name, p.real_name, p.birth_date, p.death_date 
        FROM persons p
        JOIN in_group ig ON p.id_person = ig.id_person
        WHERE ig.id_group = @groupId";

        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@groupId", groupId);
        return command.ExecuteReader();
    }

    // Método para eliminar una persona de un grupo
    public void RemovePersonFromGroup(int personId, int groupId)
    {
        string query = "DELETE FROM in_group WHERE id_person = @personId AND id_group = @groupId";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@personId", personId);
        command.Parameters.AddWithValue("@groupId", groupId);
        command.ExecuteNonQuery();

        Console.WriteLine("Persona eliminada del grupo en la base de datos.");
    }

    // Método para eliminar un grupo
    public void DeleteGroup(int groupId)
    {
        string query = "DELETE FROM groups WHERE id_group = @id";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@id", groupId);
        command.ExecuteNonQuery();

        // También eliminar las relaciones de in_group asociadas
        string deleteInGroupQuery = "DELETE FROM in_group WHERE id_group = @id";
        SQLiteCommand deleteInGroupCommand = new SQLiteCommand(deleteInGroupQuery, db.GetConnection());
        deleteInGroupCommand.Parameters.AddWithValue("@id", groupId);
        deleteInGroupCommand.ExecuteNonQuery();

        Console.WriteLine("Grupo eliminado de la base de datos.");
    }
}
