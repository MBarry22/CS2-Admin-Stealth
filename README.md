# CS2-Admin-Stealth

StealthSpectator

StealthSpectator is a CounterStrikeSharp plugin that allows server administrators to go "stealth" by hiding their player model and switching them to spectator mode. When in stealth mode, the admin becomes invisible to other players while still being able to monitor the game. Once the admin exits stealth mode, the plugin restores their previous team so that they return to normal gameplay.

Note: This version handles only the stealth functionality. All ESP (glow) functionality has been removed.

Features
Stealth Mode:
Hide your admin player model from other players (using the NoDraw effect) and automatically change your team to spectator.

Restore Mode:
Exit stealth mode to show your player model and restore your previous team automatically.

Permission Checks:
Only players with the specified admin flag (default: @css/ban) can use the stealth commands.

Command Aliases:
Use either !css_hide or !css_stealth to enable stealth mode, and !css_unhide to exit stealth mode.

Requirements
CounterStrikeSharp API:
Ensure that you have the necessary dependencies installed and referenced in your project.

.NET Runtime:
Compatible with the .NET version required by your CounterStrikeSharp installation.

Server-Side Plugin Environment:
This plugin is designed to work on a server running CounterStrikeSharp (typically for games like CS:GO/CS2).

Installation
Clone the Repository:
git clone https://github.com/yourusername/StealthSpectator.git
Build the Project:

Open the solution in your preferred IDE (e.g., Visual Studio) and build it. Make sure all dependencies (CounterStrikeSharp.API, etc.) are correctly referenced.

Deploy the Plugin:

Copy the resulting plugin DLL into the appropriate plugin folder of your CounterStrikeSharp server installation. Update your server configuration to load the plugin.

Usage
Commands
!css_hide / !css_stealth

Puts the admin into stealth spectator mode. The plugin saves your current team, switches you to spectator mode, and hides your player model.

Example:

diff
Copy
!css_hide

or

diff
Copy
!css_stealth
!css_unhide

Exits stealth mode. This command removes the hidden effect and restores your previous team.

Example:

diff
Copy
!css_unhide

How it Works

When an admin with the required admin permission (flag @css/ban) uses !css_hide or !css_stealth, the plugin:

Saves their current team.

Adds their SteamID to a hidden players list.

Changes their team to Spectator.

Hides their model by setting the NoDraw flag on their entity using reflection.

When the admin uses !css_unhide, the plugin:

Removes their SteamID from the hidden players list.

Removes the NoDraw flag so that their model becomes visible.

Restores their previous team (or a default team if none is stored).

The plugin performs necessary permission checks to ensure that only authorized admins can use these commands.

Contributing
Contributions to improve the plugin are welcome! Feel free to fork the repository, open issues, or submit pull requests with enhancements or bug fixes.

Fork the repository.

Create a new feature branch (e.g., feature/your-feature-name).

Commit your changes.

Push to your feature branch.

Open a pull request against the main branch.

License
Distributed under the MIT License. See LICENSE for more information.

Contact
mason@mpdevelopments.ca
Author: MPYawn

GitHub: MBarry22

If you have any questions or feedback, please feel free to open an issue in the repository.