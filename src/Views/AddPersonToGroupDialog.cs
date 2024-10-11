using Gtk;
using System;
using System.Collections.Generic;

public class AddPersonToGroupDialog : Dialog
{
    private ComboBox personComboBox;
    private GroupController groupController;
    private int groupId;
    private ListStore personListStore;

    public AddPersonToGroupDialog(GroupController controller, int groupId, List<string> persons) : base("Asignar Persona a Grupo", null, DialogFlags.Modal)
    {
        SetDefaultSize(300, 150);

        this.groupController = controller;
        this.groupId = groupId;

        VBox vbox = new VBox(false, 2);
        ContentArea.Add(vbox);

        // Crear ListStore para almacenar los nombres de las personas
        personListStore = new ListStore(typeof(string));

        // Agregar personas a la lista
        foreach (var person in persons)
        {
            personListStore.AppendValues(person);
        }

        // Crear el ComboBox y vincularlo al modelo
        personComboBox = new ComboBox(personListStore);

        // Configurar el CellRenderer para mostrar los nombres
        CellRendererText cell = new CellRendererText();
        personComboBox.PackStart(cell, false);
        personComboBox.AddAttribute(cell, "text", 0);

        vbox.PackStart(new Label("Selecciona una persona:"), false, false, 0);
        vbox.PackStart(personComboBox, false, false, 0);

        // Botones Aceptar/Cancelar
        AddButton("Cancelar", ResponseType.Cancel);
        AddButton("Asignar", ResponseType.Ok);

        ShowAll();
    }

    // Método para obtener la persona seleccionada
    public string GetSelectedPerson()
    {
        TreeIter iter;
        if (personComboBox.GetActiveIter(out iter))
        {
            return (string)personListStore.GetValue(iter, 0);  // Obtiene el texto seleccionado
        }
        return null;  // Si no hay nada seleccionado
    }
}
