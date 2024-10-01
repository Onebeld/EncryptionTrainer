using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.General;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Structures;
using PleasantUI;
using PleasantUI.Core;
using PleasantUI.Core.Localization;
using PleasantUI.Core.Models;
using PleasantUI.ToolKit;

namespace EncryptionTrainer.ViewModels;

public class SettingsViewModel : ObservableObject
{
    public Language SelectedLanguage
    {
        get => App.Languages.First(language => language.Key == AppSettings.Instance.Language);
        set
        {
            AppSettings.Instance.Language = value.Key;
            Localizer.Instance.ChangeLanguage(value.Key);
        }
    }

    public Theme? SelectedTheme
    {
        get => PleasantTheme.Themes.FirstOrDefault(theme => theme.Name == PleasantSettings.Instance.Theme);
        set => PleasantSettings.Instance.Theme = value?.Name ?? "System";
    }

    public bool UseAccentColor
    {
        get => !PleasantSettings.Instance.PreferUserAccentColor;
        set
        {
            PleasantSettings.Instance.PreferUserAccentColor = !value;
            
            if (PleasantSettings.Instance.PreferUserAccentColor)
                return;

            Color accent = Application.Current!.PlatformSettings!.GetColorValues().AccentColor1;

            PleasantSettings.Instance.NumericalAccentColor = accent.ToUInt32();
        }
    }

    public async Task ChangeAccentColor()
    {
        Color? newColor =
            await ColorPickerWindow.SelectColor(App.MainWindow, PleasantSettings.Instance.NumericalAccentColor);

        if (newColor is not null)
            PleasantSettings.Instance.NumericalAccentColor = newColor.Value.ToUInt32();
    }

    public async Task PasteAccentColor()
    {
        string? data = await App.MainWindow.Clipboard?.GetTextAsync()!;

        if (uint.TryParse(data, out uint uintColor))
            PleasantSettings.Instance.NumericalAccentColor = uintColor;
        else if (Color.TryParse(data, out Color color))
            PleasantSettings.Instance.NumericalAccentColor = color.ToUInt32();
    }

    public async Task CopyAccentColor()
    {
        await App.MainWindow.Clipboard?.SetTextAsync(
            $"#{PleasantSettings.Instance.NumericalAccentColor.ToString("x8").ToUpper()}")!;
        
        
    }
    
    public void GoBack()
    {
        WeakReferenceMessenger.Default.Send(new GoBackMessage());
    }
}