using Gtk;

public class ManagePerformersDialog : Dialog
{
    private TreeView performerTreeView;
    private ListStore performerListStore;

    public ManagePerformersDialog() : base("Gestionar Intérpretes", null, DialogFlags.Modal)
    {
        SetDefaultSize(400, 300);

        VBox vbox = new VBox(false, 2);
        ContentArea.Add(vbox);

        // Crear la tabla para mostrar los intérpretes
        performerListStore = new ListStore(typeof(string), typeof(string)); // Nombre, Tipo (Persona o Grupo)
        performerTreeView = new TreeView(performerListStore);

        // Crear y añadir columnas
        TreeViewColumn nameColumn = new TreeViewColumn { Title = "Nombre" };
        CellRendererText nameCell = new CellRendererText();
        nameColumn.PackStart(nameCell, true);
        nameColumn.AddAttribute(nameCell, "text", 0);
        performerTreeView.AppendColumn(nameColumn);

        TreeViewColumn typeColumn = new TreeViewColumn { Title = "Tipo" };
        CellRendererText typeCell = new CellRendererText();
        typeColumn.PackStart(typeCell, true);
        typeColumn.AddAttribute(typeCell, "text", 1);
        performerTreeView.AppendColumn(typeColumn);

        ScrolledWindow scrolledWindow = new ScrolledWindow();
        scrolledWindow.Add(performerTreeView);
        vbox.PackStart(scrolledWindow, true, true, 0);

        // Botones Aceptar/Cancelar
        AddButton("Cerrar", ResponseType.Close);

        ShowAll();
    }

    // Método para agregar un intérprete a la lista
    public void AddPerformer(string name, string type)
    {
        performerListStore.AppendValues(name, type);
    }
}
