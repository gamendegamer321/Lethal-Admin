# Lethal Admin

An admin tool for lethal company.

## Manual Installation

Download the ``LethalAdmin.dll`` and paste the dll into ``path_to_lethal_company/BepInEx/Plugins``

## Kicking and banning

The mod overrides the kick/ban system.

Under the "Users" tab you can see all users currently in your lobby.
To view more information about this user you can open their profile by click the view info button next to their name.
From here you can open their steam profile by clicking the profile button, kick them using the kick button,
or ban them using the ban button. If you ban someone you can optionally enter a reason for the ban in the textbox to the
left of the button.

Because we have overwriten the ingame kick/ban system, a kicked player is able to rejoin, but a banned player is not.

You can unban a player in the Bans tab by clicking the View ban info button for that player and then using the unban button.
When viewing a players ban you are also able to open their steam profile and view the reason for their ban.

Your bans will be saved to your drive, meaning that even after a reboot of the game, the players you have banned will
remain banned.

### Require steam

When having require steam enabled, players with their steam id set to 0 will not be allowed to join. 
Instead they will be kicked immediately after loading in.

**Keep in mind that, when using LAN-mode, you need to disable this option in order for other players to join!**

### New ban system

With the 1.3.0 update the mod is using a new ban system.
With this ban system it is possible to store a reason for a ban and the username of the banned user.
The goal is to give the lobby host more information about previous bans they have given,
and possibly in the future to show the ban reason when a player attempts to rejoin a lobby they have been banned from.

Any previous bans will automatically be transferred to the new ban system,
these players will still have their username set as "UNKNOWN" and will be given the default ban reason ("No reason given").

## Radio

To prevent radio spam, you can view who is talking on the radio with the toggle to the right of the kick/ban button ("
Talking through walkie") and you can view who has their radio enabled with the toggle "Walkie on".

## Minimum votes

Ever had that 1 person that dies in the first minute and then activates the autopilot?
Well no longer, you can now add a minimum to the amount of votes that are required before the autopilot will trigger.
To disable the feature set the minimum to 1, as that is the default of the game.

## Host only ship lever

With the host only ship lever it is possible to prevent people who are not the host from using the ship lever,
this *only works in orbit* to ensure people can still leave a planet.
This can be enabled or disabled using the "leverLock" config option, or in the admin menu.
Disabling this config option will only take affect the next time you go into orbit.

## Furniture lock (Host can only move furniture)

When you have the furniture lock enabled, only the host will be able to move furnitre (like the bunkbeds and filing cabinets) 
around and place them into storage. However it is still possible for other players to take items back out of storage.

## Actions

You can toggle the lights switch of the ship using a button in the admin panel.

Departure button, when pressed the votes by the spectators will be overriden and the autopilot triggered (this will act
as if all spectators have voted)