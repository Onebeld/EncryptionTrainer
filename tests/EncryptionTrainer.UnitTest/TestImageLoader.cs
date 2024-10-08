using EncryptionTrainer.Loaders;

namespace EncryptionTrainer.UnitTest;

public class TestImageLoader : IImageLoader
{
    private byte[]? _imageData;
    
    public event EventHandler<ImageCapturedEventArgs>? ImageCaptured;

    public TestImageLoader()
    {
    }
    
    public TestImageLoader(string fileName)
    {
        _imageData = LoadImage(fileName);
    }

    public void Load(string fileName)
    {
        _imageData = LoadImage(fileName);
        Load();
    }
    
    public void Load()
    {
        if (_imageData is null)
            throw new NullReferenceException(nameof(_imageData));
        
        ImageCaptured?.Invoke(this, new ImageCapturedEventArgs(_imageData));
    }

    public void Dispose()
    {
    }
    
    public static byte[] LoadImage(string fileName)
    {
        string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Faces", fileName);
        
        if (!File.Exists(path))
            throw new FileNotFoundException();

        return File.ReadAllBytes(path);
    }
}