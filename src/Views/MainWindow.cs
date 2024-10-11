using System;
using Gtk;
using System.IO;
using System.Threading.Tasks;
using System.Data.SQLite;

public class MainWindow : Window
{
    private ProgressBar progressBar;
    private Label progressLabel;
    private TreeView songTreeView;
    private ListStore songListStore;
    private string selectedDirectory;
    private Miner mp3Miner;
    private ProgressService progressService;
    private Entry searchEntry;
    private TreeView resultsTreeView;
    private ListStore resultsListStore;

    // Controladores
    private GroupController groupController;
    private PerformerController performerController;
    private SongController songController;

    public MainWindow() : base("Music DB")
    {
        // Inicialización de la base de datos y los controladores
        Database db = new Database("music.db");
        db.Initialize();
        groupController = new GroupController(db);
        performerController = new PerformerController(db);
        songController = new SongController(db);
	groupController = new GroupController(db);
        performerController = new PerformerController(db);

        // Configuración básica de la ventana
        SetDefaultSize(800, 600);
        SetPosition(WindowPosition.Center);

        // Crear el contenedor vertical principal
        VBox vbox = new VBox(false, 5);
        Add(vbox);

	HBox searchBox = new HBox(false, 5);
        searchEntry = new Entry();
        Button searchButton = new Button("Buscar");
        searchButton.Clicked += OnSearchButtonClicked;

        searchBox.PackStart(new Label("Buscar: "), false, false, 0);
        searchBox.PackStart(searchEntry, true, true, 0);
        searchBox.PackStart(searchButton, false, false, 0);
        vbox.PackStart(searchBox, false, false, 0);

        // Crear un botón para seleccionar el directorio
        Button selectDirectoryButton = new Button("Seleccionar Directorio");
        selectDirectoryButton.Clicked += OnSelectDirectoryButtonClicked;
        vbox.PackStart(selectDirectoryButton, false, false, 0);

        // Crear un botón para iniciar la minería
        Button mineButton = new Button("Iniciar Minería de MP3");
        mineButton.Clicked += OnMineButtonClicked;
        vbox.PackStart(mineButton, false, false, 0);

        // Botón para editar la canción seleccionada
        Button editSongButton = new Button("Editar Canción Seleccionada");
        editSongButton.Clicked += OnEditSongButtonClicked;
        vbox.PackStart(editSongButton, false, false, 0);

        // Botón para gestionar intérpretes
        Button managePerformersButton = new Button("Gestionar Intérpretes");
        managePerformersButton.Clicked += OnManagePerformersButtonClicked;
        vbox.PackStart(managePerformersButton, false, false, 0);

        // Botón para gestionar grupos
        Button manageGroupsButton = new Button("Gestionar Grupos");
        manageGroupsButton.Clicked += OnManageGroupsButtonClicked;
        vbox.PackStart(manageGroupsButton, false, false, 0);

        // Crear la barra de progreso
        progressBar = new ProgressBar();
        progressLabel = new Label("0% completado");
        vbox.PackStart(progressBar, false, false, 0);
        vbox.PackStart(progressLabel, false, false, 0);

        // Crear la tabla para mostrar las canciones
        CreateSongListView(vbox);

	CreateResultsListView(vbox);

        // Inicializar el servicio de progreso
        progressService = new ProgressService(progressBar, progressLabel);

        // Mostrar todo el contenido de la ventana
        ShowAll();
    }

    private void CreateSongListView(VBox vbox)
    {
        // Crear el almacenamiento de datos (ListStore)
        songListStore = new ListStore(typeof(int), typeof(string), typeof(string), typeof(string), typeof(string)); // id_rola, Título, Artista, Álbum, Año

        // Crear el TreeView y asignar el modelo de datos
        songTreeView = new TreeView(songListStore);

        // Crear y añadir columnas
        TreeViewColumn titleColumn = new TreeViewColumn { Title = "Título" };
        CellRendererText titleCell = new CellRendererText();
        titleColumn.PackStart(titleCell, true);
        titleColumn.AddAttribute(titleCell, "text", 1);
        songTreeView.AppendColumn(titleColumn);

        TreeViewColumn artistColumn = new TreeViewColumn { Title = "Artista" };
        CellRendererText artistCell = new CellRendererText();
        artistColumn.PackStart(artistCell, true);
        artistColumn.AddAttribute(artistCell, "text", 2);
        songTreeView.AppendColumn(artistColumn);

        TreeViewColumn albumColumn = new TreeViewColumn { Title = "Álbum" };
        CellRendererText albumCell = new CellRendererText();
        albumColumn.PackStart(albumCell, true);
        albumColumn.AddAttribute(albumCell, "text", 3);
        songTreeView.AppendColumn(albumColumn);

        TreeViewColumn yearColumn = new TreeViewColumn { Title = "Año" };
        CellRendererText yearCell = new CellRendererText();
        yearColumn.PackStart(yearCell, true);
        yearColumn.AddAttribute(yearCell, "text", 4);
        songTreeView.AppendColumn(yearColumn);

        // Crear un contenedor con scroll para la tabla
        ScrolledWindow scrolledWindow = new ScrolledWindow();
        scrolledWindow.Add(songTreeView);
        vbox.PackStart(scrolledWindow, true, true, 0);
    }

    private void OnSelectDirectoryButtonClicked(object sender, EventArgs e)
    {
        // Crear el diálogo para seleccionar el directorio
        FileChooserDialog fileChooser = new FileChooserDialog("Selecciona el directorio", this, FileChooserAction.SelectFolder, "Cancelar", ResponseType.Cancel, "Seleccionar", ResponseType.Accept);

        if (fileChooser.Run() == (int)ResponseType.Accept)
        {
            selectedDirectory = fileChooser.Filename;
            Console.WriteLine($"Directorio seleccionado: {selectedDirectory}");
        }

        fileChooser.Destroy();
    }

    private void CreateResultsListView(VBox vbox)
    {
        // Crear el almacenamiento de datos (ListStore) para los resultados
        resultsListStore = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string)); // Título, Artista, Álbum, Año

        // Crear el TreeView para mostrar los resultados
        resultsTreeView = new TreeView(resultsListStore);

        // Crear y añadir columnas
        TreeViewColumn titleColumn = new TreeViewColumn { Title = "Título" };
        CellRendererText titleCell = new CellRendererText();
        titleColumn.PackStart(titleCell, true);
        titleColumn.AddAttribute(titleCell, "text", 0);
        resultsTreeView.AppendColumn(titleColumn);

        TreeViewColumn artistColumn = new TreeViewColumn { Title = "Artista" };
        CellRendererText artistCell = new CellRendererText();
        artistColumn.PackStart(artistCell, true);
        artistColumn.AddAttribute(artistCell, "text", 1);
        resultsTreeView.AppendColumn(artistColumn);

        TreeViewColumn albumColumn = new TreeViewColumn { Title = "Álbum" };
        CellRendererText albumCell = new CellRendererText();
        albumColumn.PackStart(albumCell, true);
        albumColumn.AddAttribute(albumCell, "text", 2);
        resultsTreeView.AppendColumn(albumColumn);

        TreeViewColumn yearColumn = new TreeViewColumn { Title = "Año" };
        CellRendererText yearCell = new CellRendererText();
        yearColumn.PackStart(yearCell, true);
        yearColumn.AddAttribute(yearCell, "text", 3);
        resultsTreeView.AppendColumn(yearColumn);

        // Crear un contenedor con scroll para la tabla
        ScrolledWindow scrolledWindow = new ScrolledWindow();
        scrolledWindow.Add(resultsTreeView);
        vbox.PackStart(scrolledWindow, true, true, 0);
    }

    // Evento para manejar la búsqueda
    private void OnSearchButtonClicked(object sender, EventArgs e)
    {
        string searchTerm = searchEntry.Text.Trim();
        if (string.IsNullOrEmpty(searchTerm))
        {
            ShowErrorMessage("Por favor, introduce un término de búsqueda.");
            return;
        }

        // Limpiar los resultados anteriores
        resultsListStore.Clear();

        // Realizar la consulta
        List<Rola> songs = songController.SearchSongs(searchTerm);

        // Mostrar los resultados en el TreeView
        foreach (var song in songs)
        {
            resultsListStore.AppendValues(song.Title, song.PerformerName, song.AlbumName, song.Year.ToString());
        }
    }

    private async void OnMineButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(selectedDirectory))
        {
            ShowErrorMessage("Por favor, selecciona un directorio primero.");
            return;
        }

        // Aquí iniciamos la minería de archivos MP3
        await MineMP3FilesAsync(selectedDirectory);
    }

    private async Task MineMP3FilesAsync(string directory)
    {
        string[] mp3Files = Directory.GetFiles(directory, "*.mp3", SearchOption.AllDirectories);

        // Inicializar la barra de progreso
        progressService.Initialize(mp3Files.Length);

        int totalFiles = mp3Files.Length;

        for (int i = 0; i < totalFiles; i++)
        {
            string filePath = mp3Files[i];

            try
            {
                // Usar TagLib# para extraer la información del archivo MP3
                var file = TagLib.File.Create(filePath);

                // Obtener las etiquetas reales del archivo MP3
                string title = file.Tag.Title ?? "Desconocido";
                string artist = file.Tag.FirstPerformer ?? "Desconocido";
                string album = file.Tag.Album ?? "Desconocido";
                uint year = file.Tag.Year == 0 ? (uint)DateTime.Now.Year : file.Tag.Year;

                // Insertar la canción en la base de datos y obtener el id_rola
                int idRola = mp3Miner.InsertSong(title, artist, album, (int)year, filePath);

                // Agregar los datos reales al TreeView, incluyendo el id_rola
                songListStore.AppendValues(idRola, title, artist, album, year.ToString());

                // Actualizar la barra de progreso
                progressService.UpdateProgress(i + 1, totalFiles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar {filePath}: {ex.Message}");
            }

            await Task.Delay(100); // Simula una pequeña pausa para actualizar la UI
        }

        progressService.Complete();
        ShowInfoMessage("Minería completada.");
    }

    // Evento para editar la canción seleccionada
    private void OnEditSongButtonClicked(object sender, EventArgs e)
    {
        TreeIter iter;
        if (songTreeView.Selection.GetSelected(out iter))
        {
            string title = (string)songListStore.GetValue(iter, 1);
            string album = (string)songListStore.GetValue(iter, 3);
            string year = (string)songListStore.GetValue(iter, 4);

            EditSongDialog dialog = new EditSongDialog(title, album, year, "Género");

            if (dialog.Run() == (int)ResponseType.Ok)
            {
                // Validar el año antes de continuar
                if (!dialog.ValidateYear())
                {
                    dialog.Destroy();
                    return;
                }

                // Obtener los datos editados
                string newTitle = dialog.GetTitle();
                string newAlbum = dialog.GetAlbum();
                string newYear = dialog.GetYear();

                // Actualizar el TreeView
                songListStore.SetValue(iter, 1, newTitle);
                songListStore.SetValue(iter, 3, newAlbum);
                songListStore.SetValue(iter, 4, newYear);

                // Actualizar la base de datos usando el id_rola
                int songId = (int)songListStore.GetValue(iter, 0);
                songController.UpdateSong(songId, newTitle, newAlbum, int.Parse(newYear));

                Console.WriteLine($"Canción editada: {newTitle}, {newAlbum}, {newYear}");
            }
            dialog.Destroy();
        }
        else
        {
            ShowErrorMessage("Por favor, selecciona una canción para editar.");
        }
    }

    // Evento para gestionar intérpretes
    private void OnManagePerformersButtonClicked(object sender, EventArgs e)
    {
        ManagePerformersDialog dialog = new ManagePerformersDialog();

        // Obtener todos los intérpretes y añadirlos al diálogo
        using (SQLiteDataReader reader = performerController.GetAllPersons())
        {
            while (reader.Read())
            {
                string name = reader.GetString(0);
                string type = (reader.GetInt32(1) == 0) ? "Persona" : "Grupo";
                dialog.AddPerformer(name, type);
            }
        }

        dialog.Run();
        dialog.Destroy();
    }

    // Evento para gestionar grupos
    private void OnManageGroupsButtonClicked(object sender, EventArgs e)
    {
        ManageGroupsDialog dialog = new ManageGroupsDialog(groupController, performerController);
        dialog.LoadGroups();  // Cargar los grupos desde la base de datos
        dialog.Run();
        dialog.Destroy();
    }

    private void ShowErrorMessage(string message)
    {
        MessageDialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, message);
        dialog.Run();
        dialog.Destroy();
    }

    private void ShowInfoMessage(string message)
    {
        MessageDialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, message);
        dialog.Run();
        dialog.Destroy();
    }
}
