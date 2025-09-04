[![GitHub release](https://flat.badgen.net/github/release/gamendegamer321/Lethal-Admin/)](https://github.com/gamendegamer321/Lethal-Admin/releases/latest)
[![Game Version](https://flat.badgen.net/static/Game%20Version/v47+)](https://github.com/gamendegamer321/Lethal-Admin/)
[![License](https://flat.badgen.net/github/license/gamendegamer321/Lethal-Admin/)](https://github.com/gamendegamer321/Lethal-Admin/blob/master/LICENSE)

# About Lethal Admin

An admin tool for lethal company. Intended to help with moderating your lobbies and prevent the people that have been
banned from joining, even after restarting the lobby.

# Installation

Download the latest release from [GitHub](https://github.com/gamendegamer321/Lethal-Admin/releases/latest)
or the [Thunderstore](https://thunderstore.io/c/lethal-company/p/gamendegamer/Lethal_Admin/) and paste the dll
into `path_to_lethal_company/BepInEx/Plugins`

# Usage

When hosting a game the lethal admin UI will be available from your pause menu.
You can minimize or even hide this UI using the buttons at the bottom of the menu.

## Kicking and banning

The mod overrides the kick/ban system.

Under the "Users" tab you can see all users currently in your lobby.
To view more information about this user you can open their profile by click the view info button next to their name.
From here you can open their steam profile by clicking the profile button, kick them using the kick button,
or ban them using the ban button. If you ban someone you can optionally enter a reason for the ban in the textbox to the
left of the button.

Because we have overwriten the ingame kick/ban system, a kicked player is able to rejoin, but a banned player is not.

You can unban a player in the Bans tab by clicking the View ban info button for that player and then using the unban
button.
When viewing a players ban you are also able to open their steam profile and view the reason for their ban.

Your bans will be saved to your drive, meaning that even after a reboot of the game, the players you have banned will
remain banned.

While the player is still being approved by the game, they will already show up in your user list as "Player without
script". If, after being approved, the player is not visible on the menu provided by the game, they should remain
visible in the admin menu as "Player without script" allowing you to kick the player. **You can only kick the player
after their connection is approved, not while their approval is pending!**

### Require steam

When having require steam enabled, players with their steam id set to 0 will not be allowed to join.
Instead, they will be kicked immediately after loading in.

### New ban system

With the 1.3.0 update the mod is using a new ban system.
With this ban system it is possible to store a reason for a ban and the username of the banned user.
The goal is to give the lobby host more information about previous bans they have given,
and possibly in the future to show the ban reason when a player attempts to rejoin a lobby they have been banned from.

Any previous bans will automatically be transferred to the new ban system,
these players will still have their username set as "UNKNOWN" and will be given the default ban reason ("No reason
given").

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
Disabling this config option will only take effect the next time you go into orbit.

## Furniture lock (Host can only move furniture)

When you have the furniture lock enabled, only the host will be able to move furnitre (like the bunkbeds and filing
cabinets)
around and place them into storage. However, it is still possible for other players to take items back out of storage.

**Whitelisted players will still be able to move furniture, even if the furniture lock is enabled!**

## Actions

You can toggle the lights switch of the ship using a button in the admin panel.

Departure button, when pressed the votes by the spectators will be overriden and the autopilot triggered (this will act
as if all spectators have voted)