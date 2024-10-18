using System;

namespace EncryptionTrainer.Biometry.KeystrokeDynamics;

public class Keystroke
{
    public TimeSpan PressTime { get; set; }
    
    public TimeSpan ReleaseTime { get; set; }

    public double DwellTime => (ReleaseTime - PressTime).TotalMilliseconds;
    
    public Keystroke(TimeSpan pressTime, TimeSpan releaseTime)
    {
        PressTime = pressTime;
        ReleaseTime = releaseTime;
    }
}