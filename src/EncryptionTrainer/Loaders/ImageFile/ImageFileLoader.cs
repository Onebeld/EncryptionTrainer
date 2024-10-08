using System;
using Avalonia.Media.Imaging;

namespace EncryptionTrainer.Loaders.ImageFile;

public class ImageFileLoader(string path) : IImageLoader
{
    public event EventHandler<ImageCapturedEventArgs>? ImageCaptured;

    public void Load()
    {
        Bitmap bitmap = new(path);
        OnImageCaptured(bitmap);
    }

    protected virtual void OnImageCaptured(Bitmap image)
    {
        ImageCaptured?.Invoke(this, new ImageCapturedEventArgs(image));
    }

    public void Dispose()
    {
    }
}