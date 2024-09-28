using Avalonia;
using System;

namespace EncryptionTrainer;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        AppBuilder appBuilder = AppBuilder.Configure<App>();
        appBuilder.UsePlatformDetect();

        appBuilder
            .With(new Win32PlatformOptions
            {
                OverlayPopups = true
            })
            .With(new X11PlatformOptions
            {
                OverlayPopups = true
            });

        appBuilder.LogToTrace();

        return appBuilder;
    }
}
