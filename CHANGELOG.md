# v1.10.0

- Added option to do an additional kick attempt on players using steam, this will directly close the connection to the
  client, enabled by default
- Should make *all* players using steam visible in the menu, even if they are not visible in the normal game
  menu ([#30](https://github.com/gamendegamer321/Lethal-Admin/issues/30))

# v1.9.2

- Fixed furniture rotating when moved by client with building lock enabled

# v1.9.1

- Fixed building patch respawning furniture outside the ship
- Added config option to decide whether to automatically open the UI the first time

# v1.9.0

- Added a teleport button, allowing the host to teleport to another player.
- Added a kill button, allowing the host to kill players.
- Both buttons are available in the user tab

# v1.8.0

- Added a whitelist, this will allow whitelisted players to move around furniture in the ship (more will probably be
  added in the future)
- Fixed Require steam ID will not automatically disabling when in LAN-mode

# v1.7.2

- Fixed open ui button disappearing when rejoining/recreating a lobby

# v1.7.1

- Added new config option allowing the user to change where the "open UI" button appears
- **Although the preview is live, to save the changes you still need to apply the settings**

# v1.7.0

- Players that are kicked for not having a valid steam id, will not see the reason "This lobby requires steam
  authentication."
- Banned players will see the ban reason when attempting to rejoin
- Moved the "Open Admin UI" button to be above the "Resume" button

# v1.6.0

- Conflict issue with HostFixes should now be fixed ([#16](https://github.com/gamendegamer321/Lethal-Admin/issues/16))
- When playing in LAN-mode the require steam option will be
  ignored ([#17](https://github.com/gamendegamer321/Lethal-Admin/issues/17))

# v1.5.0

- Added minimization
- Added closing and reopening window

# v1.4.0

- [#10](https://github.com/gamendegamer321/Lethal-Admin/issues/10) Only host can move furniture (can be toggled in
  config)

# v1.3.0

- Updated ban system, now allows for more information to be stored (username and ban reason)
- Updated UI
- Added option to kick people without a valid steam ID by [@JobyGitGud](https://github.com/JobyGitGud)

# v1.2.0

- Add profile feature steamworks by [@Ambushfall](https://github.com/Ambushfall)

# v1.1.0

- Added version to menu label
- Added option to prevent non-hosts from using the ship lever while in orbit

# v1.0.6

- Fixed votes not always working when a client (not the host) has a minimum amount of votes set in their config

# v1.0.5

- Made players that are not connected appear in yellow
- Added minimum votes for early ship departure

# v1.0.4

- Fixed bans not working until you ban/kick that player again
- Updated the experimental vote override (triggering the auto pilot)
- Fixed bans not loading properly from the config

# v1.0.3

- Should fix disconnects
- New **EXPERIMENTAL** feature, increase the vote count to make the ship leave early

# v1.0.2

- Made bans persistent

# v1.0.1

- Fixed join and leave messages
- Added steam ID next to usernames
- Updates kick/ban system to kick everyone with the same username and steam ID instead of the first occurence
- Added walkie state as toggle to the right of the kick/ban buttons
- Added toggle ship lights button
- Added always show menu toggle (will also show menu when pause menu is not opened)

# v1.0.0

- First release
- The join and disconnect logs might not fully function yet