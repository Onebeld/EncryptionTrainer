using System;
using System.Linq;

namespace EncryptionTrainer.Models;

public class KeyboardBiometricProfile
{
    public double TotalTime { get; set; }
    public double AvgInterval { get; set; }
    public double StdDevInterval { get; set; }
    public double AvgHoldTime { get; set; }
    public double StdDevHoldTime { get; set; }
    public double ErrorRate { get; set; }

    public double CompareTo(KeyboardBiometricProfile profile, double[]? weights = null)
    {
        weights ??= [1, 1, 1, 1, 1, 1];

        double distance = 0;

        distance += weights[0] * Math.Abs(TotalTime - profile.TotalTime);
        distance += weights[1] * Math.Abs(AvgInterval - profile.AvgInterval);
        distance += weights[2] * Math.Abs(StdDevInterval - profile.StdDevInterval);
        distance += weights[3] * Math.Abs(AvgHoldTime - profile.AvgHoldTime);
        distance += weights[4] * Math.Abs(StdDevHoldTime - profile.StdDevHoldTime);
        distance += weights[5] * Math.Abs(ErrorRate - profile.ErrorRate);

        distance /= weights.Sum();

        return distance;
    }
}