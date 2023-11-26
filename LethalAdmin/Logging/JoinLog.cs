namespace LethalAdmin.Logging;

public class JoinLog : LogBase
{
    private readonly string _username;

    public JoinLog(string username)
    {
        _username = username;
    }
    
    protected override string GetString()
    {
        return "[Join] Player \"" + _username + "\" has joined";
    }
}