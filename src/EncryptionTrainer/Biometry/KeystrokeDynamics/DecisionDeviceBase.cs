using System.Collections.Generic;

namespace EncryptionTrainer.Biometry.KeystrokeDynamics;

/// <summary>
/// Abstract base class for decision devices.
/// </summary>
public abstract class DecisionDeviceBase
{
    /// <summary>
    /// Authenticates a user using the given templates and samples.
    /// </summary>
    /// <param name="temps">User's keystroke templates.</param>
    /// <param name="samples">User's keystroke samples to compare against templates.</param>
    /// <returns>True if user is authenticated, false otherwise.</returns>
    public abstract bool Authenticate(List<Keystroke> temps, List<Keystroke> samples);

    /// <summary>
    /// Calculates the error rate of the decision device using the given templates and samples.
    /// </summary>
    /// <param name="temps">User's keystroke templates.</param>
    /// <param name="samples">User's keystroke samples to compare against templates.</param>
    /// <returns>Error rate of the decision device.</returns>
    public double CalculateErrorRate(List<List<Keystroke>> temps, List<List<Keystroke>> samples)
    {
        int totalAttempts = temps.Count;
        int successfulAttempts = 0;

        for (int i = 0; i < totalAttempts; i++)
        {
            if (Authenticate(temps[i], samples[i]))
                successfulAttempts++;
        }

        return (double)(totalAttempts - successfulAttempts) / totalAttempts;
    }
}