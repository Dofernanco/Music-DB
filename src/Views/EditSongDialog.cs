using Gtk;

public class EditSongDialog : Dialog
{
    private Entry titleEntry, albumEntry, yearEntry, genreEntry;

    public EditSongDialog(string title, string album, string year, string genre) : base("Editar Canción", null, DialogFlags.Modal)
    {
        SetDefaultSize(300, 200);

        VBox vbox = new VBox(false, 2);
        ContentArea.Add(vbox);

        // Campos para editar la canción
        vbox.PackStart(new Label("Título:"), false, false, 0);
        titleEntry = new Entry { Text = title };
        vbox.PackStart(titleEntry, false, false, 0);

        vbox.PackStart(new Label("Álbum:"), false, false, 0);
        albumEntry = new Entry { Text = album };
        vbox.PackStart(albumEntry, false, false, 0);

        vbox.PackStart(new Label("Año:"), false, false, 0);
        yearEntry = new Entry { Text = year };
        vbox.PackStart(yearEntry, false, false, 0);

        vbox.PackStart(new Label("Género:"), false, false, 0);
        genreEntry = new Entry { Text = genre };
        vbox.PackStart(genreEntry, false, false, 0);

        // Botones Aceptar/Cancelar
        AddButton("Cancelar", ResponseType.Cancel);
        AddButton("Guardar", ResponseType.Ok);

        ShowAll();
    }

    // Validación del año (debe ser un número)
    public bool ValidateYear()
    {
        int year;
        if (!int.TryParse(yearEntry.Text, out year))
        {
            MessageDialog errorDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "El año debe ser un número.");
            errorDialog.Run();
            errorDialog.Destroy();
            return false;
        }
        return true;
    }

    // Métodos para obtener los valores modificados
    public string GetTitle() => titleEntry.Text;
    public string GetAlbum() => albumEntry.Text;
    public string GetYear() => yearEntry.Text;
    public string GetGenre() => genreEntry.Text;
}
