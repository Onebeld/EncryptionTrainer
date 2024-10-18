using EncryptionTrainer.Biometry.KeystrokeDynamics;
using EncryptionTrainer.Biometry.KeystrokeDynamics.Devices;

namespace EncryptionTrainer.UnitTest;

public class DecisionDevicesTests
{
    private List<Keystroke> _templateTrue;
    private List<Keystroke> _templateFalse;
    private List<Keystroke> _sample;
    
    [SetUp]
    public void Setup()
    {
        _templateTrue =
        [
            new(TimeSpan.FromMilliseconds(30), TimeSpan.FromMilliseconds(168)),
            new(TimeSpan.FromMilliseconds(308), TimeSpan.FromMilliseconds(390)),
            new(TimeSpan.FromMilliseconds(798), TimeSpan.FromMilliseconds(884))
        ];
        
        _templateFalse =
        [
            new(TimeSpan.FromMilliseconds(30), TimeSpan.FromMilliseconds(112)),
            new(TimeSpan.FromMilliseconds(242), TimeSpan.FromMilliseconds(340)),
            new(TimeSpan.FromMilliseconds(682), TimeSpan.FromMilliseconds(762))
        ];

        _sample =
        [
            new(TimeSpan.FromMilliseconds(30), TimeSpan.FromMilliseconds(162)),
            new(TimeSpan.FromMilliseconds(302), TimeSpan.FromMilliseconds(385)),
            new(TimeSpan.FromMilliseconds(782), TimeSpan.FromMilliseconds(862))
        ];
    }
    
    [Test]
    public void EuclideanDistanceDevice_Authenticate_Success()
    {
        var device = new EuclideanDistanceDevice(40);
        
        Assert.That(device.Authenticate(_templateTrue, _sample), Is.True);
    }

    [Test]
    public void EuclideanDistanceDevice_Authenticate_Fail()
    {
        var device = new EuclideanDistanceDevice(40);
        
        Assert.That(device.Authenticate(_templateFalse, _sample), Is.False);
    }

    [Test]
    public void ManhattanDistanceDevice_Authenticate_Success()
    {
        var device = new ManhattanDistanceDevice(30);
        
        Assert.That(device.Authenticate(_templateTrue, _sample), Is.True);
    }
    
    [Test]
    public void ManhattanDistanceDevice_Authenticate_Fail()
    {
        var device = new ManhattanDistanceDevice(30);
        
        Assert.That(device.Authenticate(_templateFalse, _sample), Is.False);
    }
    
    [Test]
    public void ThresholdComparisonDevice_Authenticate_Success()
    {
        var device = new ThresholdComparisionDevice(10);
        
        Assert.That(device.Authenticate(_templateTrue, _sample), Is.True);
    }
    
    [Test]
    public void ThresholdComparisonDevice_Authenticate_Fail()
    {
        var device = new ThresholdComparisionDevice(10);
        
        Assert.That(device.Authenticate(_templateFalse, _sample), Is.False);
    }
}