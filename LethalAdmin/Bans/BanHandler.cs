using System.Collections.Generic;
using System.IO;
using System.Linq;
using LethalAdmin.Logging;
using Newtonsoft.Json;

namespace LethalAdmin.Bans;

public static class BanHandler
{
    public static readonly Dictionary<ulong, BanInfo> Bans = new();

    public static void LoadBans()
    {
        var banPath = Path.Combine(Plugin.ConfigFolder, "gamendegamer.lethaladmin.json");

        if (!File.Exists(banPath)) return;

        var banList = JsonConvert.DeserializeObject<BanList>(File.ReadAllText(banPath));

        if (banList == null)
        {
            LethalLogger.AddLog(new Log("[Ban Handler] Failed to load ban list"));
            return;
        }

        foreach (var ban in banList.Bans)
        {
            Bans.Add(ban.SteamID, ban);
        }

        KickBanTools.UpdateKickedIDs();
    }

    public static void LoadDeprecated(string[] deprecatedBans)
    {
        if (deprecatedBans.Length == 0) return;

        // Add all deprecated bans to the ban list
        foreach (var id in deprecatedBans)
        {
            if (!ulong.TryParse(id, out var idValue)) continue;
            Bans.Add(idValue, new BanInfo
            {
                SteamID = idValue
            });
        }

        KickBanTools.UpdateKickedIDs();
        SaveBans();
    }

    public static bool AddBan(ulong id, string username, string reason)
    {
        if (Bans.ContainsKey(id)) return false;
        if (reason is null or "") reason = "No reason given";

        Bans.Add(id, new BanInfo
        {
            SteamID = id,
            Username = username,
            BanReason = reason
        });

        SaveBans();
        return true;
    }

    public static bool RemoveBan(ulong id)
    {
        if (!Bans.ContainsKey(id)) return false;

        Bans.Remove(id);
        SaveBans();
        return true;
    }

    private static void SaveBans()
    {
        var banPath = Path.Combine(Plugin.ConfigFolder, "gamendegamer.lethaladmin.json");
        var banList = new BanList
        {
            Bans = Bans.Values.ToList()
        };

        var json = JsonConvert.SerializeObject(banList);
        File.WriteAllText(banPath, json);
    }
}