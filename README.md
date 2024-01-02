# Lethal Admin
An admin tool for lethal company.

## Manual Installation
Download the ``LethalAdmin.dll`` and paste the dll into ``path_to_lethal_company/BepInEx/Plugins``

## Kicking and banning
The mod overrides the kick/ban system.
When kick the player is able to rejoin, but when banning the player is unable to rejoin.
You can unban a player in the Bans tab by clicking the unban button for that player.
Your bans will be saved to the config, meaning that after a reboot of the game, the players you have banned will remain banned.
After a reboot you can still unban the player, _but_ only their steamID is stored, meaning their username will now be displayed as "UNKNOWN".

You can also open someones steam profile from within the admin window by clicking the "Profile" button.
This even works with mods that allow for bigger lobbies, which might break the default profile button.

## Radio
To prevent radio spam, you can view who is talking on the radio with the toggle to the right of the kick/ban button ("Talking through walkie") and you can view who has their radio enabled with the toggle "Walkie on".

## Minimum votes
Ever had that 1 person that dies in the first minute and then activates the autopilot?
Well no longer, you can now add a minimum to the amount of votes that are required before the autopilot will trigger.
To disable the feature set the minimum to 1, as that is the default of the game.

## Host only ship lever
WIth the host only ship lever it is possible to prevent people who are not the host from using the ship lever, 
this *only works in orbit* to ensure people can still leave a planet.
This can be enabled or disabled using the "leverLock" config option, or in the admin menu.
Disabling this config option will only take affect the next time you go into orbit.

## Actions
You can toggle the lights switch of the ship using a button in the admin panel.

Departure button, when pressed the votes by the spectators will be overriden and the autopilot triggered (this will act as if all spectators have voted)