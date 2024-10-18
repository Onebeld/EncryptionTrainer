using System;
using System.Collections.Generic;

namespace EncryptionTrainer.Biometry.KeystrokeDynamics.Devices;

/// <summary>
/// Device that uses Manhattan distance to compare keystroke templates.
/// </summary>
public class ManhattanDistanceDevice : DecisionDeviceBase
{
    private readonly double _threshold;

    /// <summary>
    /// Initializes the device with a given threshold.
    /// </summary>
    /// <param name="threshold">The maximum allowed Manhattan distance between a template and a sample to consider them a match.</param>
    public ManhattanDistanceDevice(double threshold)
    {
        _threshold = threshold;
    }
    
    /// <summary>
    /// Compares a list of keystroke templates with a list of keystroke samples.
    /// </summary>
    /// <param name="temps">The list of keystroke templates.</param>
    /// <param name="samples">The list of keystroke samples.</param>
    /// <returns>True if the total Manhattan distance between the templates and samples is within the allowed threshold, false otherwise.</returns>
    public override bool Authenticate(List<Keystroke> temps, List<Keystroke> samples)
    {
        if (temps.Count != samples.Count)
            return false;
        
        double totalDistance = 0;

        for (int i = 0; i < temps.Count; i++)
        {
            totalDistance += Math.Abs(temps[i].DwellTime - samples[i].DwellTime);
        }
        
        return totalDistance <= _threshold;
    }
}