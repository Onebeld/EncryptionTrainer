using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using EncryptionTrainer.General.GenerationContexts;
using PleasantUI;
using PleasantUI.Core.Constants;

namespace EncryptionTrainer.General;

public class AppSettings : ViewModelBase
{
    private const string FileName = "AppSettings.json";
    
    private string _language = null!;
    private int _numberOfAttempts = 3;

    public string Language
    {
        get => _language;
        set => RaiseAndSet(ref _language, value);
    }

    public int NumberOfAttempts
    {
        get => _numberOfAttempts;
        set => RaiseAndSet(ref _numberOfAttempts, value);
    }

    public static readonly AppSettings Instance;

    static AppSettings()
    {
        if (!Directory.Exists(PleasantDirectories.Settings))
            Directory.CreateDirectory(PleasantDirectories.Settings);

        string settings = Path.Combine(PleasantDirectories.Settings, FileName);
        
        if (File.Exists(settings))
        {
            try
            {
                using FileStream fileStream = File.OpenRead(Path.Combine(PleasantDirectories.Settings, settings));
                Instance = JsonSerializer.Deserialize(fileStream, AppSettingsGenerationContext.Default.AppSettings) ?? throw new NullReferenceException();
            }
            catch
            {
                Instance = new AppSettings
                {
                    Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName
                };
            }
        }
        else
        {
            Instance = new AppSettings
            {
                Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName
            };
        }
    }

    public static void Save()
    {
        using FileStream fileStream = File.Create(Path.Combine(PleasantDirectories.Settings, FileName));
        JsonSerializer.Serialize(fileStream, Instance, AppSettingsGenerationContext.Default.AppSettings);
    }
}