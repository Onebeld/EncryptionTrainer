using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Loaders.ImageFile;

public class ImageFileLoader : IImageProvider
{
    private readonly string _path;
    private readonly List<IImageListener> _listeners = new();

    public ImageFileLoader(string path)
    {
        _path = path;
    }

    public void Attach(IImageListener listener)
    {
        _listeners.Add(listener);
    }

    public void Detach(IImageListener listener)
    {
        _listeners.Remove(listener);
    }

    public void Load()
    {
        using Bitmap bitmap = new(_path);
        using MemoryStream memoryStream = new();
        
        bitmap.Save(memoryStream);
        
        ImageData imageData = new((int)bitmap.Size.Width, (int)bitmap.Size.Height, memoryStream.ToArray());
        
        NotifyListeners(imageData);
    }

    private void NotifyListeners(ImageData imageData)
    {
        foreach (IImageListener imageListener in _listeners)
        {
            imageListener.OnImageCaptured(imageData);
        }
    }

    public void Dispose() { }
}