using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using StealthSpectator.Extensions;  // Contains our SetNoDraw extension.

namespace StealthSpectator
{
    // This plugin allows admins to go "stealth" by hiding their player model (using the NoDraw flag)
    // and switching them to spectator mode. Other players will not see the admin in the game.
    public class StealthSpectator : BasePlugin
    {
        public override string ModuleName => "Stealth Spectator";
        public override string ModuleAuthor => "MPYawn";
        public override string ModuleVersion => "1.2.2";  // Updated version

        // Store hidden admin players (by SteamID) for stealth mode.
        private readonly HashSet<ulong> _hiddenPlayers = new HashSet<ulong>();
        // Save each admin’s previous team so that it can be restored when they exit stealth mode.
        private readonly Dictionary<ulong, CsTeam> _previousTeams = new Dictionary<ulong, CsTeam>();

        // Required admin permission flag.
        private const string RequiredFlag = "@css/ban";

        public override void Load(bool hotReload)
        {
            // Optionally log load events or do additional initialization.
        }

        // Command: !css_hide (and alias !css_stealth)
        // Puts an admin into stealth spectator mode.
        [ConsoleCommand("css_hide", "Enter stealth spectator mode")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public void OnCommandHide(CCSPlayerController? player, CommandInfo command)
        {
            if (!PermissionCheck(player))
                return;
            if (AlreadyHiddenCheck(player))
                return;
            if (AlreadySpectatorCheck(player))
                return;

            EnterStealthMode(player);
        }

        // Alias for !css_hide.
        [ConsoleCommand("css_stealth", "Alias for !css_hide")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public void OnCommandStealth(CCSPlayerController? player, CommandInfo command)
            => OnCommandHide(player, command);

        private void EnterStealthMode(CCSPlayerController player)
        {
            ulong steamId = player.SteamID;
            _previousTeams[steamId] = player.Team;
            _hiddenPlayers.Add(steamId);

            // Switch the admin to spectator mode.
            player.ChangeTeam(CsTeam.Spectator);
            // Hide the admin's model by setting the NoDraw flag.
            player.Entity.SetNoDraw(true);

            SendMessage(player, "You are now in stealth spectator mode. Use !css_unhide to exit stealth.");
        }

        // Command: !css_unhide
        // Exits stealth mode and restores the admin's previous team.
        [ConsoleCommand("css_unhide", "Exit stealth spectator mode and return to your previous team")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public void OnCommandUnhide(CCSPlayerController? player, CommandInfo command)
        {
            if (!PermissionCheck(player))
                return;
            if (!IsHiddenCheck(player))
                return;

            ulong steamId = player.SteamID;
            _hiddenPlayers.Remove(steamId);

            // Remove the NoDraw flag so that the admin's model becomes visible.
            player.Entity.SetNoDraw(false);

            // Restore the previous team if available.
            if (_previousTeams.TryGetValue(steamId, out var previousTeam))
            {
                player.ChangeTeam(previousTeam);
                _previousTeams.Remove(steamId);
                SendMessage(player, "You have exited stealth spectator mode.");
            }
            else
            {
                player.ChangeTeam(CsTeam.Terrorist); // Adjust to your default team if needed.
                SendMessage(player, "You have exited stealth spectator mode (previous team not found, moved to Terrorist).");
            }
        }

        // Helper: Check that the player is valid, not a bot, and has the required admin permissions.
        private bool PermissionCheck(CCSPlayerController? player)
        {
            if (player == null || !player.IsValid || player.IsBot)
                return false;
            if (!AdminManager.PlayerHasPermissions(player, RequiredFlag))
            {
                SendMessage(player, "You do not have the required permissions to use this command.");
                return false;
            }
            return true;
        }

        private bool AlreadyHiddenCheck(CCSPlayerController? player)
        {
            if (player != null && _hiddenPlayers.Contains(player.SteamID))
            {
                SendMessage(player, "You are already in stealth spectator mode.");
                return true;
            }
            return false;
        }

        private bool AlreadySpectatorCheck(CCSPlayerController? player)
        {
            if (player != null && player.Team == CsTeam.Spectator && !_hiddenPlayers.Contains(player.SteamID))
            {
                SendMessage(player, "You are already a spectator. Use !css_unhide to exit stealth.");
                return true;
            }
            return false;
        }

        private bool IsHiddenCheck(CCSPlayerController? player)
        {
            if (player != null && !_hiddenPlayers.Contains(player.SteamID))
            {
                SendMessage(player, "You are not in stealth spectator mode.");
                return false;
            }
            return true;
        }

        // Helper: Send a chat message to a player.
        private void SendMessage(CCSPlayerController player, string message)
        {
            player.PrintToChat($"[StealthSpectator] {message}");
        }
    }
}
