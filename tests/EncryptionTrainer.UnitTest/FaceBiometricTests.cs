using Emgu.CV;
using EncryptionTrainer.Biometry;

namespace EncryptionTrainer.UnitTest;

public class FaceBiometricTests
{
    private TestImageLoader _imageLoader;
    private FaceBiometric _faceBiometric;
    
    [SetUp]
    public void Setup()
    {
        CvInvoke.UseOpenCL = false;
        
        byte[] referenceFaceData = TestImageLoader.LoadImage("referenceFace.png");
        
        _imageLoader = new TestImageLoader();
        _faceBiometric = new FaceBiometric(_imageLoader, referenceFaceData);
    }
    
    [TearDown]
    public void TearDown()
    {
        _faceBiometric.Dispose();
        _imageLoader.Dispose();
    }
    
    [Test]
    public void DetermineFace_FaceDetected_ReturnsTrue()
    {
        _imageLoader.Load("face1.png");
        Assert.That(_faceBiometric.FaceIdentified, Is.True);
        
        _imageLoader.Load("face2.png");
        Assert.That(_faceBiometric.FaceIdentified, Is.True);
    }

    [Test]
    public void DetermineFace_FaceNotDetected_ReturnsFalse()
    {
        _imageLoader.Load("empty.png");
        
        Assert.That(_faceBiometric.FaceIdentified, Is.False);
    }

    [Test]
    public void CompareFaces_FacesMatch_ReturnsTrue()
    {
        _imageLoader.Load("face2.png");
        
        Assert.That(_faceBiometric.FaceMatches, Is.True);
        
        TestContext.Out.WriteLine("Distance: " + _faceBiometric.FaceMatchDistance);
    }
    
    [Test]
    public void CompareFaces_FacesNotMatch_ReturnsFalse()
    {
        _imageLoader.Load("fakeFace.jpg");
        
        Assert.That(_faceBiometric.FaceMatches, Is.False);
        
        TestContext.Out.WriteLine("Distance: " + _faceBiometric.FaceMatchDistance);
    }
}