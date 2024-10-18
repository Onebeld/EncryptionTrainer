using System;
using System.Collections.Generic;
using EncryptionTrainer.Biometry.KeystrokeDynamics;

namespace EncryptionTrainer.Biometry.KeystrokeDynamics.Devices;

/// <summary>
/// Calculates the Euclidean distance between a list of template keystrokes and a list of sample keystrokes.
/// </summary>
public class EuclideanDistanceDevice : DecisionDeviceBase
{
    private readonly double _thresholdDistance;

    /// <summary>
    /// The threshold distance below which the device will return true for the <see cref="Authenticate"/> method.
    /// </summary>
    /// <param name="thresholdDistance">The threshold distance.</param>
    public EuclideanDistanceDevice(double thresholdDistance)
    {
        _thresholdDistance = thresholdDistance;
    }

    /// <summary>
    /// Calculates the Euclidean distance between a list of template keystrokes and a list of sample keystrokes.
    /// </summary>
    /// <param name="temps">A list of template keystrokes.</param>
    /// <param name="samples">A list of sample keystrokes.</param>
    /// <returns>
    ///   <c>true</c> if the distance is less than or equal to the threshold distance, <c>false</c> otherwise.
    /// </returns>
    public override bool Authenticate(List<Keystroke> temps, List<Keystroke> samples)
    {
        if (temps.Count != samples.Count)
            return false;

        double totalDistance = 0;

        for (int i = 0; i < temps.Count; i++)
        {
            totalDistance += Math.Pow(temps[i].DwellTime - samples[i].DwellTime, 2);
        }
        
        double normalizedDistance = Math.Sqrt(totalDistance);

        return normalizedDistance <= _thresholdDistance;
    }
}