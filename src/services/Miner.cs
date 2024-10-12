using System;
using System.IO;
using TagLib;
using System.Data.SQLite;

public class Miner
{
    private Database db;

    public Miner(Database database)
    {
        db = database;
    }

    public void MineMP3Files(string directory)
    {
        string[] mp3Files = System.IO.Directory.GetFiles(directory, "*.mp3", SearchOption.AllDirectories);
        int totalFiles = mp3Files.Length;

        for (int i = 0; i < totalFiles; i++)
        {
            string filePath = mp3Files[i];

            try
            {
                // Usar TagLib.File explícitamente para evitar ambigüedad
                TagLib.File file = TagLib.File.Create(filePath);
                
                string title = file.Tag.Title ?? "Unknown"; // TIT2
                string artist = file.Tag.FirstPerformer ?? "Unknown"; // TPE1
                string album = file.Tag.Album ?? Path.GetDirectoryName(filePath) ?? "Unknown"; // TALB
                uint track = file.Tag.Track == 0 ? 1 : file.Tag.Track; // TRCK
                uint year = file.Tag.Year == 0 ? (uint)System.IO.File.GetCreationTime(filePath).Year : file.Tag.Year; // TDRC
                string genre = file.Tag.FirstGenre ?? "Unknown"; // TCON

                // Insertar o actualizar el intérprete
                InsertOrUpdatePerformer(artist);

                // Insertar o actualizar el álbum
                int albumId = InsertOrUpdateAlbum(album, year, Path.GetDirectoryName(filePath));

                // Insertar o actualizar la canción, ahora pasando el nombre del álbum también
                InsertOrUpdateSong(title, artist, albumId, filePath, track, year, genre, album);

                // Actualizar progreso en consola
                if ((i + 1) % 10 == 0 || i == totalFiles - 1)
                {
                    Console.WriteLine($"Procesado {i + 1}/{totalFiles} archivos");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando {filePath}: {ex.Message}");
            }
        }

        Console.WriteLine("Minería completada.");
    }

    private int InsertOrUpdatePerformer(string performerName)
    {
        string queryCheck = "SELECT id_performer FROM performers WHERE name = @name";
        using (SQLiteCommand command = new SQLiteCommand(queryCheck, db.GetConnection()))
        {
            command.Parameters.AddWithValue("@name", performerName);
            var result = command.ExecuteScalar();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }
        }

        string queryInsert = "INSERT INTO performers (name, id_type) VALUES (@name, 2)";
        using (SQLiteCommand command = new SQLiteCommand(queryInsert, db.GetConnection()))
        {
            command.Parameters.AddWithValue("@name", performerName);
            command.ExecuteNonQuery();

            return (int)db.GetConnection().LastInsertRowId;
        }
    }

    private int InsertOrUpdateAlbum(string album, uint year, string path)
    {
        string query = "SELECT id_album FROM albums WHERE name = @name AND year = @year";
        using (SQLiteCommand command = new SQLiteCommand(query, db.GetConnection()))
        {
            command.Parameters.AddWithValue("@name", album);
            command.Parameters.AddWithValue("@year", year);
            var result = command.ExecuteScalar();

            if (result == null)
            {
                string insertQuery = "INSERT INTO albums (path, name, year) VALUES (@path, @name, @year)";
                using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, db.GetConnection()))
                {
                    insertCommand.Parameters.AddWithValue("@path", path);
                    insertCommand.Parameters.AddWithValue("@name", album);
                    insertCommand.Parameters.AddWithValue("@year", year);
                    insertCommand.ExecuteNonQuery();

                    return (int)db.GetConnection().LastInsertRowId;
                }
            }
            else
            {
                return Convert.ToInt32(result);
            }
        }
    }

    // Corregido para incluir el parámetro 'album'
    private void InsertOrUpdateSong(string title, string artist, int albumId, string filePath, uint track, uint year, string genre, string album)
    {
        // Obtener el ID del intérprete
        string getPerformerIdQuery = "SELECT id_performer FROM performers WHERE name = @name";
        using (SQLiteCommand getPerformerIdCommand = new SQLiteCommand(getPerformerIdQuery, db.GetConnection()))
        {
            getPerformerIdCommand.Parameters.AddWithValue("@name", artist);
            int performerId = Convert.ToInt32(getPerformerIdCommand.ExecuteScalar());

            // Insertar la canción en la tabla `rolas`
            string insertQuery = @"
            INSERT INTO rolas (id_performer, id_album, path, title, track, year, genre) 
            VALUES (@id_performer, @id_album, @path, @title, @track, @year, @genre)";

            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, db.GetConnection()))
            {
                insertCommand.Parameters.AddWithValue("@id_performer", performerId);
                insertCommand.Parameters.AddWithValue("@id_album", albumId);
                insertCommand.Parameters.AddWithValue("@path", filePath);
                insertCommand.Parameters.AddWithValue("@title", title);
                insertCommand.Parameters.AddWithValue("@track", track);
                insertCommand.Parameters.AddWithValue("@year", year);
                insertCommand.Parameters.AddWithValue("@genre", genre);
                insertCommand.ExecuteNonQuery();

                // Ahora puedes acceder al nombre del álbum y mostrarlo en la consola
                Console.WriteLine($"Canción insertada: {title}, Artista: {artist}, Álbum: {album}, Año: {year}");
            }
        }
    }

    public void ProcessSong(string filePath)
    {
        try
        {
            // Usar TagLib para extraer la información del archivo MP3
            var file = TagLib.File.Create(filePath);

            // Obtener las etiquetas reales del archivo MP3
            string title = file.Tag.Title ?? "Desconocido";
            string artist = file.Tag.FirstPerformer ?? "Desconocido";
            string album = file.Tag.Album ?? "Desconocido";  // Aquí definimos el álbum
            uint track = file.Tag.Track == 0 ? 1 : file.Tag.Track;
            uint year = file.Tag.Year == 0 ? (uint)System.IO.File.GetCreationTime(filePath).Year : file.Tag.Year;
            string genre = file.Tag.FirstGenre ?? "Desconocido";

            // Insertar o actualizar el intérprete en la base de datos
            int performerId = InsertOrUpdatePerformer(artist);

            // Insertar o actualizar el álbum en la base de datos
            int albumId = InsertOrUpdateAlbum(album, year, System.IO.Path.GetDirectoryName(filePath));

            // Mostrar la canción en la consola ANTES de llamar a InsertOrUpdateSong
            Console.WriteLine($"Canción procesada: {title}, Artista: {artist}, Álbum: {album}, Año: {year}");

            // Insertar o actualizar la canción en la base de datos, pasando el nombre del álbum también
            InsertOrUpdateSong(title, artist, albumId, filePath, track, year, genre, album);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error procesando {filePath}: {ex.Message}");
        }
    }
}
