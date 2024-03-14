using LethalAdmin.Logging;

namespace LethalAdmin;

public static class RoundUtils
{
    private static StartOfRound StartOfRound => StartOfRound.Instance;
    private static TimeOfDay TimeOfDay => TimeOfDay.Instance;

    public static bool IsVoteOverrideAvailable()
    {
        var playerCount = StartOfRound.connectedPlayersAmount + 1;
        var livingPlayers = StartOfRound.livingPlayers;

        if (playerCount - livingPlayers == 0)
        {
            // No dead players yet
            return false;
        }

        return !TimeOfDay.shipLeavingAlertCalled;
    }

    public static void OverrideVotes()
    {
        LethalLogger.AddLog(new Log("[Voting] Vote override triggered"));
        
        var time = TimeOfDay.Instance;
        var votes = StartOfRound.Instance.connectedPlayersAmount;
        time.votesForShipToLeaveEarly = votes;
        time.SetShipLeaveEarlyClientRpc(TimeOfDay.normalizedTimeOfDay + 0.1f, votes); // Trigger the departure
    }
}