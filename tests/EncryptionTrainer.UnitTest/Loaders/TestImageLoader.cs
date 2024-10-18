using EncryptionTrainer.Loaders;
using EncryptionTrainer.Models;
using SkiaSharp;

namespace EncryptionTrainer.UnitTest.Loaders;

public class TestImageLoader : IImageProvider
{
    private ImageData _imageData;
    private readonly List<IImageListener> _listeners = new();

    public void LoadFile(string fileName)
    {
        _imageData = LoadImage(fileName);
        Load();
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
        if (_imageData is null)
            throw new NullReferenceException(nameof(_imageData));
        
        NotifyListeners(_imageData);
    }

    public void Dispose()
    {
        
    }
    
    private void NotifyListeners(ImageData imageData)
    {
        foreach (IImageListener imageListener in _listeners)
        {
            imageListener.OnImageCaptured(imageData);
        }
    }

    private static ImageData LoadImage(string fileName)
    {
        string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Faces", fileName);
        
        if (!File.Exists(path))
            throw new FileNotFoundException();

        byte[] bytes = File.ReadAllBytes(path);

        using SKBitmap bitmap = SKBitmap.Decode(bytes);

        return new ImageData(bitmap.Width, bitmap.Height, bytes);
    }
}