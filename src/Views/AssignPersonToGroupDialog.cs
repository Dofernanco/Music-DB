using Gtk;
using System;
using System.Collections.Generic;

public class AssignPersonToGroupDialog : Dialog
{
    private ComboBoxText personComboBox;
    private GroupController groupController;
    private int groupId;

    public AssignPersonToGroupDialog(GroupController controller, int groupId, List<string> persons) : base("Asignar Persona a Grupo", null, DialogFlags.Modal)
    {
        SetDefaultSize(300, 150);

        this.groupController = controller;
        this.groupId = groupId;

        VBox vbox = new VBox(false, 2);
        ContentArea.Add(vbox);

        // ComboBox para seleccionar una persona
        vbox.PackStart(new Label("Selecciona una persona:"), false, false, 0);
        personComboBox = new ComboBoxText();

        // Agregar personas a la lista desplegable
        foreach (var person in persons)
        {
            personComboBox.AppendText(person);
        }

        vbox.PackStart(personComboBox, false, false, 0);

        // Botones Aceptar/Cancelar
        AddButton("Cancelar", ResponseType.Cancel);
        AddButton("Asignar", ResponseType.Ok);

        ShowAll();
    }

    // Método para obtener la persona seleccionada
    public string GetSelectedPerson() => personComboBox.ActiveText;
}
