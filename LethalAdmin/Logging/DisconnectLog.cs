namespace LethalAdmin.Logging;

public class DisconnectLog : LogBase
{
    private readonly string _username;

    public DisconnectLog(string username)
    {
        _username = username;
    }
    
    protected override string GetString()
    {
        return "[Join] Player \"" + _username + "\" has disconnected";
    }
}