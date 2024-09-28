using System.Resources;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EncryptionTrainer.General;
using EncryptionTrainer.Properties;
using EncryptionTrainer.Structures;
using PleasantUI.Core.Localization;

namespace EncryptionTrainer;

public partial class App : Application
{
    public static MainWindow MainWindow { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Localizer.Instance.AddResourceManager(new ResourceManager(typeof(Local)));
        Localizer.Instance.EditLanguage(AppSettings.Instance.Language);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            desktop.MainWindow = MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static readonly Language[] Languages =
    [
        new("English (English)", "en"),
        new("Русский (Russian)", "ru")
    ];
}