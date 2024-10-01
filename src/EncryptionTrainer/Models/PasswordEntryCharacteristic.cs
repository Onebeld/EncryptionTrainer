using System.Collections.Generic;
using System.Linq;

namespace EncryptionTrainer.Models;

public class PasswordEntryCharacteristic
{
    private readonly List<long> _keyDownTimes = new();
    private readonly List<long> _keyUpTimes = new();
    private readonly List<long> _interKeyTimes = new();

    public void AddKeyDownTime(long time)
    {
        _keyDownTimes.Add(time);
    }
    
    public void AddKeyUpTime(long time)
    {
        _keyUpTimes.Add(time);
    }
    
    public void AddInterKeyTime()
    {
        if (_keyDownTimes.Count < 2)
            return;
        
        _interKeyTimes.Add(_keyDownTimes.Last() - _keyDownTimes[^2]);
    }

    public List<long> GetInterKeyTimes()
    {
        return _interKeyTimes;
    }

    public List<long> GetHoldTimes()
    {
        return _keyDownTimes.Select((t, i) => _keyUpTimes[i] - t).ToList();
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

        for (int i = 0; i < _interKeyTimes.Count; i++)
        {
            interKeyTimeDiffs.Add(CalculatePercentageDifference(_interKeyTimes[i], otherInterKeyTimes[i]));
        }

        double avgHoldTimeDiff = holdTimeDiffs.Average();
        double avgInterkeyTimeDiff = interKeyTimeDiffs.Average();

        return (avgHoldTimeDiff, avgInterkeyTimeDiff);
    }

    public void ClearAllTimes()
    {
        _keyDownTimes.Clear();
        _keyUpTimes.Clear();
        _interKeyTimes.Clear();
    }

    private double CalculatePercentageDifference(double currentValue, double baseValue)
    {
        return (currentValue - baseValue) / baseValue * 100;
    }
}