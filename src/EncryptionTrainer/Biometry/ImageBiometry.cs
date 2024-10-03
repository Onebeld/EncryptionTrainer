using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using EncryptionTrainer.Loaders;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Biometry;

public class ImageBiometry : IDisposable
{
    private const int DetectedFaceWidth = 250;
    private const int DetectedFaceHeight = 250;
    
    private readonly IImageLoader _imageLoader;
    
    private readonly CascadeClassifier _cascadeClassifier;
    
    private Image<Bgr, byte>? _detectedFace;

    public ImageBiometry(IImageLoader imageLoader, List<byte[]> faceData)
    {
        _imageLoader = imageLoader;
        
        string pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
        
        if (!File.Exists(pathToFile))
            throw new FileNotFoundException("haarcascade_frontalface_default.xml not found");

        _cascadeClassifier = new CascadeClassifier(pathToFile);
    }

    public byte[]? GetFaceData()
    {
        DetectFace();
        
        if (_detectedFace is null)
            return null;

        Image<Bgr, byte> detectedFace = _detectedFace.Resize(DetectedFaceWidth, DetectedFaceHeight, Inter.Cubic);

        return CvInvoke.Imencode(".png", detectedFace);
    }
    
    public bool? CompareFaces(byte[] referenceFaceData)
    {
        DetectFace();
        
        if (_detectedFace is null)
            return null;
        
        Image<Bgr, byte> faceImage = new(DetectedFaceWidth, DetectedFaceHeight);
        CvInvoke.Imdecode(referenceFaceData, ImreadModes.AnyColor, faceImage.Mat);
        
        Image<Bgr, byte> detectedFace = _detectedFace.Resize(DetectedFaceWidth, DetectedFaceHeight, Inter.Cubic);

        double threshold = 0.45;
        double distance = CvInvoke.Norm(faceImage.Mat, detectedFace.Mat, NormType.RelativeL2);
        
        return distance < threshold;
    }

    private void DetectFace()
    {
        ImageData imageData = _imageLoader.Load();
        Image<Bgr, byte> image = new(imageData.Width, imageData.Height);
        
        CvInvoke.Imdecode(imageData.Data, ImreadModes.AnyColor, image.Mat);
        
        DetectFace(image);
    }

    private void DetectFace(Image<Bgr, byte> image)
    {
        Image<Bgr, byte> grayImage = image.Convert<Bgr, byte>();

        Rectangle[] faces =
            _cascadeClassifier.DetectMultiScale(grayImage, 1.2, 10, Size.Empty, Size.Empty);
        
        foreach (Rectangle face in faces)
        {
            _detectedFace = image.Copy(face);
            
            break;
        }
        
        grayImage.Dispose();
        image.Dispose();
    }

    public void Dispose()
    {
        _detectedFace?.Dispose();
        _cascadeClassifier.Dispose();
    }
}