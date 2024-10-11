using System;
using System.Data.SQLite;

public class SongController
{
    private Database db;

    public SongController(Database database)
    {
        db = database;
    }

    // Método para actualizar una canción en la base de datos
    public void UpdateSong(int songId, string title, string album, int year)
    {
        string query = "UPDATE rolas SET title = @title, album = @album, year = @year WHERE id_rola = @id";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@album", album);
        command.Parameters.AddWithValue("@year", year);
        command.Parameters.AddWithValue("@id", songId);

        command.ExecuteNonQuery();
        Console.WriteLine("Canción actualizada en la base de datos.");
    }

    public void DeleteSong(int songId)
    {
        string query = "DELETE FROM rolas WHERE id_rola = @id";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@id", songId);
        command.ExecuteNonQuery();
    }


    // Método para obtener la información de una canción
    public (string title, string album, int year) GetSongById(int songId)
    {
        string query = "SELECT title, album, year FROM rolas WHERE id_rola = @id";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@id", songId);

        using (SQLiteDataReader reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                string title = reader.GetString(0);
                string album = reader.GetString(1);
                int year = reader.GetInt32(2);
                return (title, album, year);
            }
        }

        return (null, null, 0);
    }

    public List<Rola> SearchSongs(string searchTerm)
    {
        List<Rola> songs = new List<Rola>();

        string query = @"
            SELECT r.title, p.name AS performer, a.name AS album, r.year
            FROM rolas r
            JOIN performers p ON r.id_performer = p.id_performer
            JOIN albums a ON r.id_album = a.id_album
            WHERE r.title LIKE @searchTerm
            OR p.name LIKE @searchTerm
            OR a.name LIKE @searchTerm";

        using (SQLiteCommand command = new SQLiteCommand(query, db.GetConnection()))
        {
            command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string title = reader.GetString(0);
                    string performer = reader.GetString(1);
                    string album = reader.GetString(2);
                    int year = reader.GetInt32(3);

                    Rola song = new Rola(0, 0, 0, "", title, 0, year, "");
                    song.PerformerName = performer;
                    song.AlbumName = album;
                    songs.Add(song);
                }
            }
        }

        return songs;
    }
}
