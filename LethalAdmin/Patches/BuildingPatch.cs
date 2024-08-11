using HarmonyLib;
using LethalAdmin.Bans;
using Unity.Netcode;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(ShipBuildModeManager))]
public class BuildingPatch
{
    [HarmonyPatch("StoreObjectServerRpc")]
    [HarmonyPrefix]
    public static bool OnStoreObject(ShipBuildModeManager __instance, ref NetworkObjectReference objectRef,
        int playerWhoStored) => OnBuild(__instance, ref objectRef, playerWhoStored, true);

    [HarmonyPatch("PlaceShipObjectServerRpc")]
    [HarmonyPrefix]
    public static bool OnPlacingObject(ShipBuildModeManager __instance, ref NetworkObjectReference objectRef,
        int playerWhoMoved) => OnBuild(__instance, ref objectRef, playerWhoMoved, false);

    private static bool OnBuild(ShipBuildModeManager __instance, ref NetworkObjectReference objectRef,
        int playerId, bool movingToStorage)
    {
        if (playerId == 0 || !Plugin.Instance.FurnitureLocked || !__instance.IsServer) return true;

        // Whitelisted players are allowed to move furniture
        var players = StartOfRound.Instance.allPlayerScripts;
        if (playerId < players.Length && BanHandler.IsWhitelisted(players[playerId].playerSteamId))
        {
            return true;
        }

        if (!objectRef.TryGet(out var networkObject))
        {
            return false;
        }

        var component = networkObject.GetComponentInChildren<PlaceableShipObject>();
        if (component == null)
        {
            return false;
        }

        var item = StartOfRound.Instance.unlockablesList.unlockables[component.unlockableID];
        if (item.inStorage)
        {
            return false;
        }

        if (movingToStorage)
        {
            // Remove from storage for the client attempting to move it
            StartOfRound.Instance.ReturnUnlockableFromStorageClientRpc(component.unlockableID);
        }
        else
        {
            // Place the object back to the correct place
            var transform = component.transform;
            __instance.PlaceShipObjectClientRpc(transform.position, component.mainMesh.transform.rotation.eulerAngles,
                objectRef, 0);
        }

        return false;
    }
}