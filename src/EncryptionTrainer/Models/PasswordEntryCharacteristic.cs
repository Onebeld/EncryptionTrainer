using System;
using System.Collections.Generic;
using System.Linq;

namespace EncryptionTrainer.Models;

public class PasswordEntryCharacteristic
{
    public List<long> KeyDownTimes { get; }
    public List<long> KeyUpTimes { get; }
    public List<long> InterKeyTimes { get; }

    public PasswordEntryCharacteristic()
    {
        KeyDownTimes = new List<long>();
        KeyUpTimes = new List<long>();
        InterKeyTimes = new List<long>();
    }
    
    public PasswordEntryCharacteristic(List<long> keyDownTimes, List<long> keyUpTimes, List<long> interKeyTimes)
    {
        KeyDownTimes = keyDownTimes;
        KeyUpTimes = keyUpTimes;
        InterKeyTimes = interKeyTimes;
    }
    
    public void AddInterKeyTime()
    {
        if (KeyDownTimes.Count < 2)
            return;
        
        InterKeyTimes.Add(KeyDownTimes.Last() - KeyDownTimes[^2]);
    }

    public List<long> GetInterKeyTimes()
    {
        return InterKeyTimes;
    }

    public List<long> GetHoldTimes()
    {
        return KeyDownTimes.Select((t, i) => KeyUpTimes[i] - t).ToList();
    }

    public (double, double) CompareCharacteristic(PasswordEntryCharacteristic other)
    {
        List<long> holdTimes = GetHoldTimes();
        List<long> otherHoldTimes = other.GetHoldTimes();

        List<long> otherInterKeyTimes = other.GetInterKeyTimes();
        
        List<double> holdTimeDiffs = new();
        List<double> interKeyTimeDiffs = new();

        for (int i = 0; i < holdTimes.Count; i++)
        {
            holdTimeDiffs.Add(CalculatePercentageDifference(holdTimes[i], otherHoldTimes[i]));
        }

        for (int i = 0; i < InterKeyTimes.Count; i++)
        {
            interKeyTimeDiffs.Add(CalculatePercentageDifference(InterKeyTimes[i], otherInterKeyTimes[i]));
        }

        double avgHoldTimeDiff = holdTimeDiffs.Average();
        double avgInterkeyTimeDiff = interKeyTimeDiffs.Average();

        return (avgHoldTimeDiff, avgInterkeyTimeDiff);
    }

    public void ClearAllTimes()
    {
        KeyDownTimes.Clear();
        KeyUpTimes.Clear();
        InterKeyTimes.Clear();
    }

    private double CalculatePercentageDifference(double currentValue, double baseValue)
    {
        return Math.Abs((currentValue - baseValue) / baseValue);
    }
}