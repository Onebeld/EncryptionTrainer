using System.Collections.Generic;
using System.Linq;
using EncryptionTrainer.General;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Biometry.KeystrokeDynamics;

public class AuthenticationManager
{
    private const double MatchThreshold = 40;
    
    private readonly List<DecisionDeviceBase> _decisionDevices;
    private readonly UserDatabase _userDatabase;

    public AuthenticationManager(List<DecisionDeviceBase> decisionDevices, UserDatabase userDatabase)
    {
        _decisionDevices = decisionDevices;
        _userDatabase = userDatabase;
    }

    public bool Register(string username, string password, byte[]? faceData, List<List<Keystroke>> templates)
    {
        if (_userDatabase.Users.Any(u => u.Username == username))
            return false;

        User user = new(username, password, templates, faceData);
        _userDatabase.Users.Add(user);

        return true;
    }

    public (bool isAuthenticated, double matchPercentage) Authenticate(string username, string password, List<List<Keystroke>> samples)
    {
        User? user = _userDatabase.Users.FirstOrDefault(u => u.Username == username);

        if (user is null)
            return (false, -1);

        if (user.Password != password)
            return (false, -1);

        double matchScore = 0;

        foreach (DecisionDeviceBase device in _decisionDevices)
            matchScore += device.CalculateErrorRate(user.KeystrokeTemplates, samples);
        
        matchScore /= _decisionDevices.Count;
        
        double matchPercentage = matchScore * 100;

        return (matchPercentage > MatchThreshold, matchPercentage);
    }
}