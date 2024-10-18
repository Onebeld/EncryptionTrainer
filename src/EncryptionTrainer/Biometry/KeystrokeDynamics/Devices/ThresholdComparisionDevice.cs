using System;
using System.Collections.Generic;
using EncryptionTrainer.Biometry.KeystrokeDynamics;

namespace EncryptionTrainer.Biometry.KeystrokeDynamics.Devices;

/// <summary>
/// Compares two keystroke sequences by comparing the dwell times of the
/// corresponding keystrokes. The sequences are considered to match if the
/// absolute difference of the dwell times is less than or equal to the
/// specified threshold in at least half of the keystrokes.
/// </summary>
public class ThresholdComparisionDevice : DecisionDeviceBase
{
    private readonly double _threshold;

    /// <summary>
    /// Creates a new ThresholdComparisionDevice with the specified threshold.
    /// </summary>
    /// <param name="threshold">The maximum allowed difference in dwell times.</param>
    public ThresholdComparisionDevice(double threshold)
    {
        _threshold = threshold;
    }
    
    /// <summary>
    /// Compares two keystroke sequences.
    /// </summary>
    /// <param name="temps">The first sequence of keystrokes.</param>
    /// <param name="samples">The second sequence of keystrokes.</param>
    /// <returns>True if the sequences are considered to match, false otherwise.</returns>
    public override bool Authenticate(List<Keystroke> temps, List<Keystroke> samples)
    {
        if (temps.Count != samples.Count)
            return false;

        int matchCount = 0;

        for (int i = 0; i < temps.Count; i++)
        {
            if (Math.Abs(temps[i].DwellTime - samples[i].DwellTime) <= _threshold)
                matchCount++;
        }
        
        return (double)matchCount / temps.Count >= 0.5;
    }
}