using System;
using Gtk;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Inicializar GTK#
        Application.Init();

        // Crear la ventana principal y ejecutar la aplicación
        MainWindow win = new MainWindow();
        win.Show();

        // Iniciar el ciclo principal de GTK
        Application.Run();
    }
}

