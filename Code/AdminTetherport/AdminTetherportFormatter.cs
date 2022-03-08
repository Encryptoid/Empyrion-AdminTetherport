using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminTetherport
{
    class AdminTetherportFormatter
    {
        public static string FormatAttpMessage(List<PlayerInfo> players)
        {
            var uiString = $"Click one of the below players from the online player list. A Tether will be created at your current " +
                $"location and you will be teleported to the player. Then type '!uattp' to return to the Tether!\n\n";

            foreach (var player in players)
            {
                uiString += FormatPlayerLocation(player);
            }

            return uiString;
        }

        public static string FormatRetieveMessage(List<PlayerInfo> players)
        {
            var uiString = $"Click one of the below players from the online player list to teleport them to your location!\n\n";

            foreach (var player in players)
            {
                uiString += FormatPlayerLocation(player);
            }

            return uiString;
        }

        public static string FormatPlayerLocation(PlayerInfo player)
        {
            return $"<link=\"{player.entityId}\"><indent=5%><line-height=150%>{player.playerName} | {player.entityId} | {player.playfield} | X:{player.pos.x} | Y:{player.pos.y} | Z:{player.pos.z}</line-height></indent></link>\n";
        }

        public static string FormatTetherportFileName(string steamId)
        {
            return Path.Combine("Tethers", $"{steamId}.tether");
        }
    }
}
