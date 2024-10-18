using System;
using CommunityToolkit.Mvvm.ComponentModel;
using EncryptionTrainer.Models;
using SkiaSharp;

namespace EncryptionTrainer.Loaders;

public class CameraListener : ObservableObject, IImageListener, IDisposable
{
    private SKBitmap? _bitmap;

    public SKBitmap? Bitmap
    {
        get => _bitmap;
        set => SetProperty(ref _bitmap, value);
    }

    public void OnImageCaptured(ImageData imageData)
    {
        Bitmap = SKBitmap.Decode(imageData.Data);
    }
    
    public void Dispose() { }
}