namespace LethalAdmin.Logging;

public class BanLog : LogBase
{
    private readonly string _username;
    private readonly ulong _steamID;
    
    public BanLog(string username, ulong steamID)
    {
        _username = username;
        _steamID = steamID;
    }

    protected override string GetString()
    {
        return "[BanLog] The user " + _username + "@" + _steamID + " has been banned";
    }
}