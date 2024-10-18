using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using EncryptionTrainer.Loaders;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Biometry;

public class FaceBiometric : IImageListener, IDisposable
{
    public enum Mode
    {
        Determination,
        Comparison
    }
    
    private const double Threshold = 0.4;
    private const int DetectedFaceWidth = 250;
    private const int DetectedFaceHeight = 250;
    
    private readonly IImageProvider _imageLoader;
    private readonly CascadeClassifier _cascadeClassifier;
    
    private Image<Gray, byte>? _detectedFace;

    public FaceBiometric(IImageProvider imageLoader)
    {
        _imageLoader = imageLoader;
        _imageLoader.Attach(this);
        
        string pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
        
        if (!File.Exists(pathToFile))
            throw new FileNotFoundException("haarcascade_frontalface_default.xml not found");

        _cascadeClassifier = new CascadeClassifier(pathToFile);
    }


    public void OnImageCaptured(ImageData imageData)
    {
        DetectFace(imageData);
    }
    
    public byte[]? GetFaceData()
    {
        return _detectedFace is null ? null : CvInvoke.Imencode(".png", _detectedFace);
    }

    public (bool?, double?) CompareFace(byte[] referenceFaceData)
    {
        if (_detectedFace is null)
            return (null, null);
        
        Image<Gray, byte> faceImage = new(DetectedFaceWidth, DetectedFaceHeight);
        CvInvoke.Imdecode(referenceFaceData, ImreadModes.AnyColor, faceImage.Mat);
        
        Image<Gray, byte> detectedFace = _detectedFace.Resize(DetectedFaceWidth, DetectedFaceHeight, Inter.Cubic);

        double distance = CvInvoke.Norm(faceImage.Mat, detectedFace.Mat, NormType.RelativeL2);
        
        return (distance < Threshold, distance);
    }

    private bool DetectFace(ImageData imageData)
    {
        _detectedFace?.Dispose();
        _detectedFace = null;
        
        using Image<Bgr, byte> image = new(imageData.Width, imageData.Height);
        
        CvInvoke.Imdecode(imageData.Data, ImreadModes.AnyColor, image.Mat);
        
        return DetectFace(image);
    }

    private bool DetectFace(Image<Bgr, byte> image)
    {
        Rectangle[] faces =
            _cascadeClassifier.DetectMultiScale(image, 1.2, 10, Size.Empty, Size.Empty);

        if (faces.Length > 0)
            _detectedFace = image.Copy(faces[0]).Convert<Gray, byte>().Resize(DetectedFaceWidth, DetectedFaceHeight, Inter.Cubic);

        return _detectedFace is not null;
    }

    public void Dispose()
    {
        _imageLoader.Detach(this);
        
        _cascadeClassifier.Dispose();
        _detectedFace?.Dispose();
    }
}