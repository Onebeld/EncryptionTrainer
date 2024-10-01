using EncryptionTrainer.Models;

namespace EncryptionTrainer.Loaders.Camera;

public class CameraLoader : IImageLoader
{
    private byte[] _data;
    private int _width;
    private int _height;

    public void Load(int width, int height, byte[] data)
    {
        _data = data;

        _width = width;
        _height = height;
    }
    
    public ImageData Load()
    {
        return new ImageData(_width, _height, _data);
    }
}