using Gtk;
using System;
using System.Collections.Generic;

public class ManageGroupsDialog : Dialog
{
    private TreeView groupTreeView;
    private ListStore groupListStore;
    private GroupController groupController;
    private PerformerController performerController;

    public ManageGroupsDialog(GroupController controller, PerformerController performerCtrl) : base("Gestionar Grupos", null, DialogFlags.Modal)
    {
        SetDefaultSize(400, 300);

        groupController = controller;
        performerController = performerCtrl;

        VBox vbox = new VBox(false, 2);
        ContentArea.Add(vbox);

        // Crear el ListStore y TreeView para mostrar los grupos
        groupListStore = new ListStore(typeof(int), typeof(string), typeof(string), typeof(string)); // id, Nombre, Fecha inicio, Fecha fin
        groupTreeView = new TreeView(groupListStore);

        // Crear las columnas del TreeView
        TreeViewColumn nameColumn = new TreeViewColumn { Title = "Nombre del Grupo" };
        CellRendererText nameCell = new CellRendererText();
        nameColumn.PackStart(nameCell, true);
        nameColumn.AddAttribute(nameCell, "text", 1);
        groupTreeView.AppendColumn(nameColumn);

        TreeViewColumn startDateColumn = new TreeViewColumn { Title = "Fecha de Inicio" };
        CellRendererText startDateCell = new CellRendererText();
        startDateColumn.PackStart(startDateCell, true);
        startDateColumn.AddAttribute(startDateCell, "text", 2);
        groupTreeView.AppendColumn(startDateColumn);

        TreeViewColumn endDateColumn = new TreeViewColumn { Title = "Fecha de Fin" };
        CellRendererText endDateCell = new CellRendererText();
        endDateColumn.PackStart(endDateCell, true);
        endDateColumn.AddAttribute(endDateCell, "text", 3);
        groupTreeView.AppendColumn(endDateColumn);

        ScrolledWindow scrolledWindow = new ScrolledWindow();
        scrolledWindow.Add(groupTreeView);
        vbox.PackStart(scrolledWindow, true, true, 0);

        // Botones para agregar, eliminar grupos y asignar personas
        Button addGroupButton = new Button("Agregar Grupo");
        addGroupButton.Clicked += OnAddGroupButtonClicked;
        vbox.PackStart(addGroupButton, false, false, 0);

        Button deleteGroupButton = new Button("Eliminar Grupo");
        deleteGroupButton.Clicked += OnDeleteGroupButtonClicked;
        vbox.PackStart(deleteGroupButton, false, false, 0);

        Button assignPersonButton = new Button("Asignar Persona a Grupo");
        assignPersonButton.Clicked += OnAssignPersonButtonClicked;
        vbox.PackStart(assignPersonButton, false, false, 0);

        ShowAll();
    }

    // Cargar todos los grupos desde la base de datos
    public void LoadGroups()
    {
        groupListStore.Clear();

        using (var reader = groupController.GetAllGroups())
        {
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string startDate = reader.GetString(2);
                string endDate = reader.IsDBNull(3) ? "Presente" : reader.GetString(3);
                groupListStore.AppendValues(id, name, startDate, endDate);
            }
        }
    }

    // Evento para agregar un grupo
    private void OnAddGroupButtonClicked(object sender, EventArgs e)
    {
        AddGroupDialog addGroupDialog = new AddGroupDialog();

        if (addGroupDialog.Run() == (int)ResponseType.Ok)
        {
            string name = addGroupDialog.GetGroupName();
            string startDate = addGroupDialog.GetStartDate();
            string endDate = addGroupDialog.GetEndDate();

            groupController.AddGroup(name, startDate, endDate);

            // Recargar los grupos en el TreeView
            LoadGroups();
        }

        addGroupDialog.Destroy();
    }

    // Evento para eliminar un grupo
    private void OnDeleteGroupButtonClicked(object sender, EventArgs e)
    {
        TreeIter iter;
        if (groupTreeView.Selection.GetSelected(out iter))
        {
            int groupId = (int)groupListStore.GetValue(iter, 0);

            MessageDialog confirmDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, "¿Estás seguro de que deseas eliminar este grupo?");
            if (confirmDialog.Run() == (int)ResponseType.Yes)
            {
                groupController.DeleteGroup(groupId);
                groupListStore.Remove(ref iter);
            }
            confirmDialog.Destroy();
        }
        else
        {
            MessageDialog errorDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Por favor, selecciona un grupo para eliminar.");
            errorDialog.Run();
            errorDialog.Destroy();
        }
    }

    // Evento para asignar una persona a un grupo
    private void OnAssignPersonButtonClicked(object sender, EventArgs e)
    {
        TreeIter iter;
        if (groupTreeView.Selection.GetSelected(out iter))
        {
            int groupId = (int)groupListStore.GetValue(iter, 0);

            // Obtener la lista de personas disponibles
            List<string> persons = new List<string>();
            using (var reader = performerController.GetAllPersons())
            {
                while (reader.Read())
                {
                    string personName = reader.GetString(1);
                    persons.Add(personName);
                }
            }

            if (persons.Count > 0)
            {
                // Abrir el diálogo para asignar una persona al grupo
                AssignPersonToGroupDialog dialog = new AssignPersonToGroupDialog(groupController, groupId, persons);

                if (dialog.Run() == (int)ResponseType.Ok)
                {
                    string selectedPerson = dialog.GetSelectedPerson();
                    int personId = performerController.GetPersonIdByName(selectedPerson);

                    // Asignar la persona al grupo
                    groupController.AddPersonToGroup(personId, groupId);
                    Console.WriteLine($"Persona {selectedPerson} asignada al grupo {groupId}.");
                }

                dialog.Destroy();
            }
            else
            {
                MessageDialog noPersonsDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "No hay personas disponibles para asignar.");
                noPersonsDialog.Run();
                noPersonsDialog.Destroy();
            }
        }
        else
        {
            MessageDialog errorDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Por favor, selecciona un grupo.");
            errorDialog.Run();
            errorDialog.Destroy();
        }
    }
}
