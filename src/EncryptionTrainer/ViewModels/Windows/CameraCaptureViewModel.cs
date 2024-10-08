using System;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using EncryptionTrainer.Biometry;
using EncryptionTrainer.Loaders;
using EncryptionTrainer.Loaders.Camera;
using EncryptionTrainer.Windows;
using SkiaSharp;

namespace EncryptionTrainer.ViewModels.Windows;

public class CameraCaptureViewModel : ObservableObject
{
    private readonly CameraCaptureWindow _window;
    
    private readonly FaceBiometric.Mode _mode;
    private readonly FaceBiometric _faceBiometric;
    private readonly CameraLoader _cameraLoader;

    private SKBitmap? _cameraBitmap;
    
    public SKBitmap? CameraBitmap
    {
        get => _cameraBitmap;
        set => SetProperty(ref _cameraBitmap, value);
    }

    public CameraCaptureViewModel(CameraCaptureWindow window, FaceBiometric.Mode mode, byte[]? referenceFaceData = null)
    {
        _window = window;
        _mode = mode;

        if (Design.IsDesignMode)
            return;

        _cameraLoader = new CameraLoader();

        _faceBiometric = new FaceBiometric(_cameraLoader, referenceFaceData);
        
        _cameraLoader.ImageCaptured += CameraLoaderOnImageCaptured;
        _cameraLoader.Initialized += CameraLoaderOnInitialized;
    }

    private void CameraLoaderOnInitialized(object? sender, EventArgs e)
    {
        _cameraLoader.Load();
    }

    private void CameraLoaderOnImageCaptured(object? sender, ImageCapturedEventArgs e)
    {
        CameraBitmap?.Dispose();
        CameraBitmap = SKBitmap.Decode(e.ImageData.Data);

        if (_mode == FaceBiometric.Mode.Determination && _faceBiometric.FaceIdentified) 
            ReceiveFaceDataAsync(_faceBiometric.DetectedFaceData!);
        else if (_mode == FaceBiometric.Mode.Comparison && _faceBiometric.FaceMatches is not null)
            ReceivePredictionResultAsync(_faceBiometric.FaceMatches.Value);
    }

    public void CancelAsync()
    {
        Dispose();
        Dispatcher.UIThread.Invoke(() =>
        {
            _window.Close(null);
        });
    }

    public void ReceiveFaceDataAsync(byte[] faceData)
    {
        Dispose();
        
        Dispatcher.UIThread.Invoke(() =>
        {
            _window.Close(faceData);
        });
    }

    public void ReceivePredictionResultAsync(bool result)
    {
        Dispose();
        
        Dispatcher.UIThread.Invoke(() =>
        {
            _window.Close(result);
        });
    }

    private void Dispose()
    {
        _cameraLoader.ImageCaptured -= CameraLoaderOnImageCaptured;
        _cameraLoader.Initialized -= CameraLoaderOnInitialized;
        
        _faceBiometric.Dispose();
        _cameraLoader.Dispose();
        CameraBitmap?.Dispose();
    }
}