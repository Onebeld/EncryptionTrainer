using System;
using EncryptionTrainer.General;
using PleasantUI.Controls;

namespace EncryptionTrainer;

public partial class MainWindow : PleasantWindow
{
    public MainWindow()
    {
        InitializeComponent();

        Closed += OnClosed;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        AppSettings.Save();
    }
}