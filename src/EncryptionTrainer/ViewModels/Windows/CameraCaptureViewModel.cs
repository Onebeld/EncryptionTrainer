using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Emgu.CV;
using EncryptionTrainer.Biometry;
using EncryptionTrainer.Loaders;
using EncryptionTrainer.Loaders.Camera;
using EncryptionTrainer.Windows;

namespace EncryptionTrainer.ViewModels.Windows;

public class CameraCaptureViewModel : ObservableObject
{
    private readonly CameraCaptureWindow _window;
    
    private readonly FaceBiometric.Mode _mode;
    private readonly FaceBiometric _faceBiometric;
    private readonly CameraLoader _cameraLoader;
    
    private readonly byte[]? _referenceFaceData;

    private bool _canceled;
    
    public CameraListener CameraListener { get; }

    public CameraCaptureViewModel(CameraCaptureWindow window, FaceBiometric.Mode mode, byte[]? referenceFaceData = null)
    {
        _window = window;
        _mode = mode;

        if (Design.IsDesignMode)
            return;

        _referenceFaceData = referenceFaceData;
        
        _cameraLoader = new CameraLoader();
        _faceBiometric = new FaceBiometric(_cameraLoader);
        
        CameraListener = new CameraListener();
        _cameraLoader.Attach(CameraListener);
    }

    public async Task CaptureCamera()
    {
        while (CvInvoke.WaitKey(1) == -1)
        {
            if (_canceled)
                return;
            
            _cameraLoader.Load();

            switch (_mode)
            {
                case FaceBiometric.Mode.Determination:
                {
                    byte[]? faceData = _faceBiometric.GetFaceData();
                
                    if (faceData is null)
                        continue;
                
                    ReceiveFaceDataAsync(faceData);
                    return;
                }
                case FaceBiometric.Mode.Comparison when _referenceFaceData is not null:
                {
                    (bool? result, _) = _faceBiometric.CompareFace(_referenceFaceData);
                
                    if (result is null)
                        continue;
                
                    ReceivePredictionResultAsync(result.Value);
                    return;
                }
                default:
                    // ~ 30 FPS
                    await Task.Delay(32);
                    break;
            }
        }
    }

    public void CancelAsync()
    {
        _canceled = true;
        
        Dispose();

        _window.Close(null);
    }

    public void ReceiveFaceDataAsync(byte[] faceData)
    {
        Dispose();
        
        _window.Close(faceData);
    }

    public void ReceivePredictionResultAsync(bool result)
    {
        Dispose();
        
        _window.Close(result);
    }

    private void Dispose()
    {
        CameraListener.Dispose();
        _cameraLoader.Dispose();
        
        _faceBiometric.Dispose();
    }
}