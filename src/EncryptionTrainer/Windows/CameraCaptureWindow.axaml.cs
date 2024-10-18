using Avalonia.Controls;
using Avalonia.Interactivity;
using EncryptionTrainer.Biometry;
using EncryptionTrainer.ViewModels.Windows;
using PleasantUI.Controls;

namespace EncryptionTrainer.Windows;

public partial class CameraCaptureWindow : ContentDialog
{
    private readonly CameraCaptureViewModel _cameraCaptureViewModel;
    
    public CameraCaptureWindow()
    {
        InitializeComponent();
        
        _cameraCaptureViewModel = new CameraCaptureViewModel(this, FaceBiometric.Mode.Determination);

        DataContext = _cameraCaptureViewModel;
    }

    public CameraCaptureWindow(FaceBiometric.Mode mode, byte[]? referenceFaceData = null)
    {
        InitializeComponent();
        
        _cameraCaptureViewModel = new CameraCaptureViewModel(this, mode, referenceFaceData);

        DataContext = _cameraCaptureViewModel;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        if (Design.IsDesignMode)
            return;

        _ = _cameraCaptureViewModel.CaptureCamera();
    }
}