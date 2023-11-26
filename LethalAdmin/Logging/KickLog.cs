namespace LethalAdmin.Logging;

public class KickLog : LogBase
{
    private readonly string _username;
    
    public KickLog(string username)
    {
        _username = username;
    }

    protected override string GetString()
    {
        return "[KickLog] The user " + _username + " has been kicked";
    }
}