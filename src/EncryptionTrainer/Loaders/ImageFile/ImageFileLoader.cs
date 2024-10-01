using System.IO;
using Avalonia.Media.Imaging;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Loaders.ImageFile;

public class ImageFileLoader(string path) : IImageLoader
{
    public ImageData Load()
    {
        Bitmap bitmap = new(path);

        MemoryStream ms = new();
        bitmap.Save(ms);
        ms.Position = 0;
        
        return new ImageData((int)bitmap.Size.Width, (int)bitmap.Size.Height, ms.ToArray());
    }
}