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

        // Inicializar el minero MP3
        mp3Miner = new Miner(db);

        groupController = new GroupController(db);
        performerController = new PerformerController(db);
        songController = new SongController(db);

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
        songListStore = new ListStore(typeof(int), typeof(string), typeof(string), typeof(string), typeof(string)); // id_rola, Título, Artista, Álbum, Año

        songTreeView = new TreeView(songListStore);

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

        ScrolledWindow scrolledWindow = new ScrolledWindow();
        scrolledWindow.Add(songTreeView);
        vbox.PackStart(scrolledWindow, true, true, 0);
    }

    private void OnSelectDirectoryButtonClicked(object sender, EventArgs e)
    {
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
        resultsListStore = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string)); // Título, Artista, Álbum, Año

        resultsTreeView = new TreeView(resultsListStore);

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

        ScrolledWindow scrolledWindow = new ScrolledWindow();
        scrolledWindow.Add(resultsTreeView);
        vbox.PackStart(scrolledWindow, true, true, 0);
    }

    private async void OnMineButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(selectedDirectory))
        {
            ShowErrorMessage("Por favor, selecciona un directorio primero.");
            return;
        }

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
            // Llamar al método ProcessSong de Miner para procesar el archivo MP3
            mp3Miner.ProcessSong(filePath);

            // Extraer los datos que quieras mostrar en el TreeView (puedes también obtener los datos de la base de datos si lo prefieres)
            var file = TagLib.File.Create(filePath);
            string title = file.Tag.Title ?? "Desconocido";
            string artist = file.Tag.FirstPerformer ?? "Desconocido";
            string album = file.Tag.Album ?? "Desconocido";
            uint year = file.Tag.Year == 0 ? (uint)System.IO.File.GetCreationTime(filePath).Year : file.Tag.Year;

            // Actualizar el ListStore con la canción procesada
            songListStore.AppendValues(i + 1, title, artist, album, year.ToString());

            Console.WriteLine($"Canción agregada al ListStore: {title}, Artista: {artist}, Álbum: {album}, Año: {year}");

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



    private void OnSearchButtonClicked(object sender, EventArgs e)
    {
        string searchTerm = searchEntry.Text.Trim();
        if (string.IsNullOrEmpty(searchTerm))
        {
            ShowErrorMessage("Por favor, introduce un término de búsqueda.");
            return;
        }

        resultsListStore.Clear();

        List<Rola> songs = songController.SearchSongs(searchTerm);

        foreach (var song in songs)
        {
            resultsListStore.AppendValues(song.Title, song.PerformerName, song.AlbumName, song.Year.ToString());
        }
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
