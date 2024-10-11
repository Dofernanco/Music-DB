using System;
using System.IO;
using TagLib;
using System.Data.SQLite;

public class Miner
{
    private Database db;  // La clase Database maneja la conexión a SQLite

    public Miner(Database database)
    {
        db = database;
    }

    public void MineMP3Files(string directory)
    {
        // Obtener todos los archivos MP3 en el directorio (y subdirectorios)
        string[] mp3Files = Directory.GetFiles(directory, "*.mp3", SearchOption.AllDirectories);

        int totalFiles = mp3Files.Length;

        for (int i = 0; i < totalFiles; i++)
        {
            string filePath = mp3Files[i];

            try
            {
                // Extraer etiquetas ID3v2.4 usando TagLib#
                var file = TagLib.File.Create(filePath);
                string title = file.Tag.Title ?? "Desconocido";
                string artist = file.Tag.FirstPerformer ?? "Desconocido";
                string album = file.Tag.Album ?? Path.GetDirectoryName(filePath) ?? "Desconocido";
                uint track = file.Tag.Track == 0 ? 1 : file.Tag.Track;
                uint year = file.Tag.Year == 0 ? (uint)DateTime.Now.Year : file.Tag.Year;
                string genre = file.Tag.FirstGenre ?? "Desconocido";

                // Insertar o actualizar los datos en la base de datos
                InsertOrUpdatePerformer(artist);
                int albumId = InsertOrUpdateAlbum(album, year, Path.GetDirectoryName(filePath));
                InsertOrUpdateSong(title, artist, albumId, filePath, track, year, genre);

                // Simular barra de progreso
                Console.WriteLine($"Procesado {i + 1}/{totalFiles} archivos");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando {filePath}: {ex.Message}");
            }
        }

        Console.WriteLine("Minería completada.");
    }

    public int InsertOrUpdatePerformer(string performerName)
    {
    // Verificar si el intérprete ya existe
    string queryCheck = "SELECT id_performer FROM performers WHERE name = @name";
    using (SQLiteCommand command = new SQLiteCommand(queryCheck, db.GetConnection()))
    {
        command.Parameters.AddWithValue("@name", performerName);
        var result = command.ExecuteScalar();

        if (result != null) // Si ya existe, devolver su ID
        {
            return Convert.ToInt32(result);
        }
    }

    // Si no existe, insertar el nuevo intérprete
    string queryInsert = "INSERT INTO performers (name, id_type) VALUES (@name, 0)";  // 0: Persona, 1: Grupo
    using (SQLiteCommand command = new SQLiteCommand(queryInsert, db.GetConnection()))
    {
        command.Parameters.AddWithValue("@name", performerName);
        command.ExecuteNonQuery();

        return (int)db.GetConnection().LastInsertRowId;  // Devolver el ID del nuevo intérprete
    }
    }


    private int InsertOrUpdateAlbum(string album, uint year, string path)
    {
        // Verificar si el álbum ya existe
        string query = "SELECT id_album FROM albums WHERE name = @name";
        SQLiteCommand command = new SQLiteCommand(query, db.GetConnection());
        command.Parameters.AddWithValue("@name", album);

        object result = command.ExecuteScalar();
        if (result == null)
        {
            // Si no existe, insertamos un nuevo álbum
            string insertQuery = "INSERT INTO albums (path, name, year) VALUES (@path, @name, @year)";
            SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, db.GetConnection());
            insertCommand.Parameters.AddWithValue("@path", path);
            insertCommand.Parameters.AddWithValue("@name", album);
            insertCommand.Parameters.AddWithValue("@year", year);
            insertCommand.ExecuteNonQuery();

            // Obtener el ID del nuevo álbum insertado
            return (int)db.GetConnection().LastInsertRowId;
        }
        else
        {
            // Retornar el ID del álbum existente
            return Convert.ToInt32(result);
        }
    }

    private void InsertOrUpdateSong(string title, string artist, int albumId, string filePath, uint track, uint year, string genre)
    {
        // Obtener el ID del intérprete
        string getPerformerIdQuery = "SELECT id_performer FROM performers WHERE name = @name";
        SQLiteCommand getPerformerIdCommand = new SQLiteCommand(getPerformerIdQuery, db.GetConnection());
        getPerformerIdCommand.Parameters.AddWithValue("@name", artist);
        int performerId = Convert.ToInt32(getPerformerIdCommand.ExecuteScalar());

        // Insertar la canción en la tabla `rolas`
        string insertQuery = @"
        INSERT INTO rolas (id_performer, id_album, path, title, track, year, genre) 
        VALUES (@id_performer, @id_album, @path, @title, @track, @year, @genre)";

        SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, db.GetConnection());
        insertCommand.Parameters.AddWithValue("@id_performer", performerId);
        insertCommand.Parameters.AddWithValue("@id_album", albumId);
        insertCommand.Parameters.AddWithValue("@path", filePath);
        insertCommand.Parameters.AddWithValue("@title", title);
        insertCommand.Parameters.AddWithValue("@track", track);
        insertCommand.Parameters.AddWithValue("@year", year);
        insertCommand.Parameters.AddWithValue("@genre", genre);
        insertCommand.ExecuteNonQuery();
    }

    public int InsertSong(string title, string artist, string album, int year, string filePath)
{
    // Verificar si el intérprete ya existe o insertarlo
    int performerId = InsertOrUpdatePerformer(artist);

    // Verificar si el álbum ya existe o insertarlo
    int albumId = InsertOrUpdateAlbum(album, (uint)year, System.IO.Path.GetDirectoryName(filePath));

    // Insertar la canción (rola) en la tabla de canciones
    string query = @"
        INSERT INTO rolas (id_performer, id_album, path, title, year, genre, track) 
        VALUES (@id_performer, @id_album, @path, @title, @year, 'Unknown', 0)";

    using (SQLiteCommand command = new SQLiteCommand(query, db.GetConnection()))
    {
        command.Parameters.AddWithValue("@id_performer", performerId);
        command.Parameters.AddWithValue("@id_album", albumId);
        command.Parameters.AddWithValue("@path", filePath);
        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@year", year);

        command.ExecuteNonQuery();

        // Devuelve el id_rola (la clave primaria de la canción insertada)
        return (int)db.GetConnection().LastInsertRowId;
    }
}

}
