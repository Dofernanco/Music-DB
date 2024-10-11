using Gtk;
using System;

public class ProgressService
{
    private ProgressBar progressBar;
    private Label progressLabel;

    public ProgressService(ProgressBar progressBar, Label progressLabel = null)
    {
        this.progressBar = progressBar;
        this.progressLabel = progressLabel;
    }

    public void Initialize(int totalTasks)
    {
        progressBar.Fraction = 0;
        if (progressLabel != null)
        {
            progressLabel.Text = "0% completado";
        }
    }

    public void UpdateProgress(int currentTask, int totalTasks)
    {
        double progressFraction = (double)currentTask / totalTasks;
        progressBar.Fraction = progressFraction;

        if (progressLabel != null)
        {
            progressLabel.Text = $"{(int)(progressFraction * 100)}% completado";
        }
    }

    public void Complete()
    {
        progressBar.Fraction = 1.0;
        if (progressLabel != null)
        {
            progressLabel.Text = "Proceso completado.";
        }
    }
}
