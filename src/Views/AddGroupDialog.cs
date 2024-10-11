using Gtk;
using System;

public class AddGroupDialog : Dialog
{
    private Entry nameEntry, startDateEntry, endDateEntry;

    public AddGroupDialog() : base("Agregar Grupo", null, DialogFlags.Modal)
    {
        SetDefaultSize(300, 200);

        VBox vbox = new VBox(false, 2);
        ContentArea.Add(vbox);

        // Campo para el nombre del grupo
        vbox.PackStart(new Label("Nombre del Grupo:"), false, false, 0);
        nameEntry = new Entry();
        vbox.PackStart(nameEntry, false, false, 0);

        // Campo para la fecha de inicio
        vbox.PackStart(new Label("Fecha de Inicio (YYYY-MM-DD):"), false, false, 0);
        startDateEntry = new Entry();
        vbox.PackStart(startDateEntry, false, false, 0);

        // Campo para la fecha de fin
        vbox.PackStart(new Label("Fecha de Fin (Opcional, YYYY-MM-DD):"), false, false, 0);
        endDateEntry = new Entry();
        vbox.PackStart(endDateEntry, false, false, 0);

        // Botones Aceptar/Cancelar
        AddButton("Cancelar", ResponseType.Cancel);
        AddButton("Agregar", ResponseType.Ok);

        ShowAll();
    }

    // Métodos para obtener los valores ingresados
    public string GetGroupName() => nameEntry.Text;
    public string GetStartDate() => startDateEntry.Text;
    public string GetEndDate() => endDateEntry.Text;
}
