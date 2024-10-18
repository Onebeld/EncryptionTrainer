using Emgu.CV;
using EncryptionTrainer.Biometry;
using EncryptionTrainer.UnitTest.Helpers;
using EncryptionTrainer.UnitTest.Loaders;

namespace EncryptionTrainer.UnitTest;

public class FaceBiometricTests
{
    private TestImageLoader _imageLoader;
    private FaceBiometric _faceBiometric;
    
    private byte[] _referenceFaceData;
    
    [SetUp]
    public void Setup()
    {
        CvInvoke.UseOpenCL = false;
        
        _referenceFaceData = ImageLoading.LoadImage("referenceFace.png");
        
        _imageLoader = new TestImageLoader();
        _faceBiometric = new FaceBiometric(_imageLoader);
    }
    
    [TearDown]
    public void TearDown()
    {
        _faceBiometric.Dispose();
        _imageLoader.Dispose();
    }
    
    [Test]
    public void DetermineFace_FaceDetected_ReturnsByteArray()
    {
        _imageLoader.LoadFile("face1.png");
        
        byte[]? faceData1 = _faceBiometric.GetFaceData();
        Assert.That(faceData1, Is.Not.Null);
        
        _imageLoader.LoadFile("face2.png");
        
        byte[]? faceData2 = _faceBiometric.GetFaceData();
        Assert.That(faceData2, Is.Not.Null);
    }

    [Test]
    public void DetermineFace_FaceNotDetected_ReturnsNull()
    {
        _imageLoader.LoadFile("empty.png");
        
        byte[]? faceData = _faceBiometric.GetFaceData();
        Assert.That(faceData, Is.Null);
    }

    [Test]
    public void CompareFaces_FacesMatch_ReturnsTrue()
    {
        _imageLoader.LoadFile("face2.png");

        (bool? result, double? distance) = _faceBiometric.CompareFace(_referenceFaceData);
        
        Assert.That(result, Is.True);
        
        TestContext.Out.WriteLine("Distance: " + distance);
    }
    
    [Test]
    public void CompareFaces_FacesNotMatch_ReturnsFalse()
    {
        _imageLoader.LoadFile("fakeFace.jpg");
        
        (bool? result, double? distance) = _faceBiometric.CompareFace(_referenceFaceData);
        
        Assert.That(result, Is.False);
        
        TestContext.Out.WriteLine("Distance: " + distance);
    }
}