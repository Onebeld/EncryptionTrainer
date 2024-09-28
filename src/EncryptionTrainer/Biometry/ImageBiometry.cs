using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace EncryptionTrainer.Biometry;

public class ImageBiometry
{
    private Image<Gray, byte>? _detectedFace = null;

    private CascadeClassifier _cascadeClassifier;
    private EigenFaceRecognizer _faceRecognizer;

    public ImageBiometry()
    {
        string pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
        
        if (!File.Exists(pathToFile))
            throw new FileNotFoundException("haarcascade_frontalface_default.xml not found");

        _cascadeClassifier = new CascadeClassifier(pathToFile);
    }

    public byte[]? GetFace(Bitmap avaloniaBitmap)
    {
        DetectFace(avaloniaBitmap);
        
        if (_detectedFace is null)
            return null;

        Image<Gray, byte> detectedFace = _detectedFace.Resize(100, 100, Inter.Cubic);
        return detectedFace.ToJpegData();
    }

    public void DetectFace(Bitmap avaloniaBitmap)
    {
        System.Drawing.Bitmap bitmap = ConvertToSystemDrawingBitmap(avaloniaBitmap);

        Image<Bgr, byte> image = bitmap.ToImage<Bgr, byte>();
        DetectFace(image);
    }

    private void DetectFace(Image<Bgr, byte> image)
    {
        Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

        Rectangle[] faces =
            _cascadeClassifier.DetectMultiScale(grayImage, 1.2, 10, new Size(50, 50), new Size(200, 200));

        foreach (Rectangle face in faces)
        {
            _detectedFace = image.Copy(face).Convert<Gray, byte>();
            
            break;
        }
    }

    private void FaceRecognition()
    {
        
    }

    private System.Drawing.Bitmap ConvertToSystemDrawingBitmap(Bitmap bitmap)
    {
        using MemoryStream memoryStream = new();
        
        bitmap.Save(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        
        return new System.Drawing.Bitmap(memoryStream);
    }
}