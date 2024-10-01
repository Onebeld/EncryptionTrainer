using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using EncryptionTrainer.Loaders;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Biometry;

public class ImageBiometry : IDisposable
{
    private readonly IImageLoader _imageLoader;
    
    private readonly CascadeClassifier _cascadeClassifier;
    private EigenFaceRecognizer? _faceRecognizer;
    
    private Image<Gray, byte>? _detectedFace;

    public ImageBiometry(IImageLoader imageLoader, List<byte[]> faceData)
    {
        _imageLoader = imageLoader;
        
        string pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
        
        if (!File.Exists(pathToFile))
            throw new FileNotFoundException("haarcascade_frontalface_default.xml not found");

        _cascadeClassifier = new CascadeClassifier(pathToFile);

        if (faceData.Count <= 0)
            return;
        
        TrainFaceRecognizer(faceData);
    }

    public byte[]? GetFaceData()
    {
        DetectFace();
        
        if (_detectedFace is null)
            return null;

        Image<Gray, byte> detectedFace = _detectedFace.Resize(100, 100, Inter.Cubic);
        
        return detectedFace.ToJpegData();
    }
    
    public FaceRecognizer.PredictionResult PredictFace()
    {
        if (_faceRecognizer is null || _detectedFace is null)
            return default;
        
        DetectFace();
        
        FaceRecognizer.PredictionResult result = _faceRecognizer.Predict(_detectedFace.Resize(100, 100, Inter.Cubic));
        return result;
    }

    private void TrainFaceRecognizer(List<byte[]> faceData)
    {
        VectorOfMat vectorOfMat = new(faceData.Count);
        VectorOfInt vectorOfInt = new(faceData.Count);

        int i = 0;
        foreach (byte[] bytes in faceData)
        {
            Image<Gray, byte> faceImage = new(100, 100);
            faceImage.Bytes = bytes;
                
            vectorOfMat.Push(faceImage.Mat);
            vectorOfInt.Push(new []{ i++ });
        }
            
        _faceRecognizer = new EigenFaceRecognizer(faceData.Count);
        _faceRecognizer.Train(vectorOfMat, vectorOfInt);
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
        Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

        Rectangle[] faces =
            _cascadeClassifier.DetectMultiScale(grayImage, 1.2, 10, Size.Empty, Size.Empty);
        
        foreach (Rectangle face in faces)
        {
            _detectedFace = image.Copy(face).Convert<Gray, byte>();
            
            break;
        }
        
        grayImage.Dispose();
        image.Dispose();
    }

    public void Dispose()
    {
        _detectedFace?.Dispose();
        _cascadeClassifier.Dispose();
        _faceRecognizer?.Dispose();
    }
}