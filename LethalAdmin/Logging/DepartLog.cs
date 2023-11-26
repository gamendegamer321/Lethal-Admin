namespace LethalAdmin.Logging;

public class DepartLog : LogBase
{
    private readonly string _username;

    public DepartLog(string username)
    {
        _username = username;
    }
    
    protected override string GetString()
    {
        return "[Departure] Player \"" + _username + "\" has started the ship";
    }
}