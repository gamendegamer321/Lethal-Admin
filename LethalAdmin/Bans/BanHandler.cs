using System.Collections.Generic;
using System.IO;
using System.Linq;
using LethalAdmin.Logging;
using Newtonsoft.Json;

namespace LethalAdmin.Bans;

public static class BanHandler
{
    private static readonly Dictionary<ulong, BanInfo> Bans = new();
    private static readonly Dictionary<ulong, WhitelistInfo> Whitelist = new();

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

        foreach (var whitelist in banList.Whitelist)
        {
            Whitelist.Add(whitelist.SteamID, whitelist);
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

    public static bool AddWhitelist(ulong id, string username)
    {
        if (Whitelist.ContainsKey(id)) return false;
        
        Whitelist.Add(id, new WhitelistInfo
        {
            SteamID = id,
            Username = username
        });
        
        SaveBans();
        return true;
    }

    public static bool RemoveWhitelist(ulong id)
    {
        if (!Whitelist.ContainsKey(id)) return false;

        Whitelist.Remove(id);
        SaveBans();
        return true;
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
    
    public static IEnumerable<BanInfo> GetBans() => Bans.Values;
    public static IEnumerable<WhitelistInfo> GetWhitelist() => Whitelist.Values;

    public static bool TryGetBan(ulong id, out BanInfo banInfo)
    {
        return Bans.TryGetValue(id, out banInfo);
    }

    public static bool IsWhitelisted(ulong id)
    {
        return Whitelist.Keys.Contains(id);
    }

    private static void SaveBans()
    {
        var banPath = Path.Combine(Plugin.ConfigFolder, "gamendegamer.lethaladmin.json");
        var banList = new BanList
        {
            Bans = Bans.Values.ToList(),
            Whitelist = Whitelist.Values.ToList()
        };

        var json = JsonConvert.SerializeObject(banList);
        File.WriteAllText(banPath, json);
    }
}