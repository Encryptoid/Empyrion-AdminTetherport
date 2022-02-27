using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Database;
using EmpyrionModdingFramework.Teleport;
using InventoryManagement;
using ModLocator;


namespace AdminTetherport
{
    public class AdminTetherport: EmpyrionModdingFrameworkBase
    {
        private IDatabaseManager _dbManager;

        protected override void Initialize()
        {
            ModName = "AdminTetherport";

            var modLocator = new FolderLocator(Log);
            _dbManager = new CsvManager(modLocator.GetDatabaseFolder(ModName));

            CommandManager.CommandPrexix = "!";
            CommandManager.CommandList.Add(new ChatCommand("admintetherport", AdminTetherporter, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand("attp", AdminTetherporter, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand("uattp", Untether, PlayerPermission.Admin));
        }

        private async Task AdminTetherporter(MessageData messageData)
        {
            PlayerInfo adminPlayer = await QueryPlayerInfo(messageData.SenderEntityId);
            if (adminPlayer == null) return;

            var playerIdList = await QueryPlayerList();

            List<PlayerInfo> allPlayers = new List<PlayerInfo>();
            foreach(var playerId in playerIdList)
            {
                allPlayers.Add(await QueryPlayerInfo(playerId));
            }

            ShowLinkedDialog(adminPlayer.entityId, FormatPlayerList(allPlayers), "Admin Tetherporter!", TetherportToPlayer);

            return;
        }

        private async Task Untether(MessageData messageData)
        {
            PlayerInfo player = await QueryPlayerInfo(messageData.SenderEntityId);
            if (player == null) return;

            var tetherporterRecord = _dbManager.LoadRecords<PlayerLocationRecord>(FormatTetherportFileName(player.steamId))?.FirstOrDefault();

            if (tetherporterRecord == null)
            {
                Log($"Entity {player.entityId}/{player.playerName} requester Untether but no tether was found.");
                await MessagePlayer(player.entityId, $"No tether was found.", 5, MessagerPriority.Red);
                return;
            }

            await TeleportPlayer(player.entityId, tetherporterRecord.Playfield,
                tetherporterRecord.PosX, tetherporterRecord.PosY, tetherporterRecord.PosZ,
                tetherporterRecord.RotX, tetherporterRecord.RotY, tetherporterRecord.RotZ);
        }

        private async void TetherportToPlayer(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            Log("Link id was " + linkId + " & button was " + buttonIdx);
            if (string.IsNullOrWhiteSpace(linkId))
                return;

            var targetPlayerId = int.Parse(linkId);

            var adminPlayer = await QueryPlayerInfo(playerId);

            //Save new admin tether record
            _dbManager.SaveRecord(FormatTetherportFileName(adminPlayer.steamId), adminPlayer.ToPlayerLocationRecord(), 
                clearExisting: true);

            await TeleportPlayerToPlayer(adminPlayer.entityId, targetPlayerId);
            await MessagePlayer(adminPlayer.entityId, $"Created Admin Tetherporter tether! Telported to PlayerId:{targetPlayerId}.", 5);

        }

        private string FormatPlayerList(List<PlayerInfo> players)
        {
            var uiString = $"Click one of the below players from the online player list. A Tether will be created at your current " +
                $"location and you will be teleported to the player. Then type '!uattp' to return to the Tether!\n\n";

            foreach (var player in players) 
            {
                uiString += FormatPlayerLocation(player);
            }

            return uiString;
        }

        private string FormatPlayerLocation(PlayerInfo player)
        {
            return $"<link=\"{player.entityId}\"><indent=5%><line-height=150%>{player.playerName} | {player.entityId} | {player.playfield} | X:{player.pos.x} | Y:{player.pos.y} | Z:{player.pos.z}</line-height></indent></link>\n";
        }

        public static string FormatTetherportFileName(string steamId)
        {
            return Path.Combine("Tethers", $"{steamId}.tether");
        }
    }
}
