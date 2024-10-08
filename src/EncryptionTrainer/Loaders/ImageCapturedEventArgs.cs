using System;
using System.IO;
using Avalonia.Media.Imaging;
using EncryptionTrainer.Models;
using SkiaSharp;

namespace EncryptionTrainer.Loaders;

public class ImageCapturedEventArgs : EventArgs
{
    public ImageData ImageData { get; }

    public ImageCapturedEventArgs(Bitmap bitmap)
    {
        using MemoryStream ms = new();
        bitmap.Save(ms);

        ImageData = new ImageData((int)bitmap.Size.Width, (int)bitmap.Size.Height, ms.ToArray());
    }

    public ImageCapturedEventArgs(byte[] bytes)
    {
        using SKBitmap skBitmap = SKBitmap.Decode(bytes);
        ImageData = new ImageData(skBitmap.Width, skBitmap.Height, bytes);
    }
}