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
            CommandManager.CommandList.Add(new ChatCommand("admintetherport", AdminTetherportDialog, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand("attp", AdminTetherportDialog, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand("uattp", AdminUntether, PlayerPermission.Admin));
            CommandManager.CommandList.Add(new ChatCommand("retrieve", RetrievePlayerDialog, PlayerPermission.Admin));
        }

        private async Task AdminTetherportDialog(MessageData messageData)
        {
            PlayerInfo adminPlayer = await QueryPlayerInfo(messageData.SenderEntityId);
            if (adminPlayer == null) return;

            var playerIdList = await QueryPlayerList();

            List<PlayerInfo> allPlayers = new List<PlayerInfo>();
            foreach(var playerId in playerIdList)
            {
                allPlayers.Add(await QueryPlayerInfo(playerId));
            }

            ShowLinkedDialog(adminPlayer.entityId, AdminTetherportFormatter.FormatAttpMessage(allPlayers), "Admin Tetherporter!", TetherportToPlayer);

            return;
        }

        private async void TetherportToPlayer(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            if (string.IsNullOrWhiteSpace(linkId))
                return;

            var targetPlayerId = int.Parse(linkId);

            var adminPlayer = await QueryPlayerInfo(playerId);

            //Save new admin tether record
            _dbManager.SaveRecord(AdminTetherportFormatter.FormatTetherportFileName(adminPlayer.steamId), adminPlayer.ToPlayerLocationRecord(),
                clearExisting: true);

            await TeleportPlayerToPlayer(adminPlayer.entityId, targetPlayerId);
            await MessagePlayer(adminPlayer.entityId, $"Created Admin Tetherporter tether! Telported to PlayerId:{targetPlayerId}.", 5);
        }

        private async Task AdminUntether(MessageData messageData)
        {
            PlayerInfo player = await QueryPlayerInfo(messageData.SenderEntityId);
            if (player == null) return;

            var tetherporterRecord = _dbManager.LoadRecords<PlayerLocationRecord>(AdminTetherportFormatter.FormatTetherportFileName(player.steamId))?.FirstOrDefault();

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

        private async Task RetrievePlayerDialog(MessageData messageData)
        {
            PlayerInfo adminPlayer = await QueryPlayerInfo(messageData.SenderEntityId);
            if (adminPlayer == null) return;

            var playerIdList = await QueryPlayerList();

            List<PlayerInfo> allPlayers = new List<PlayerInfo>();
            foreach (var playerId in playerIdList)
            {
                allPlayers.Add(await QueryPlayerInfo(playerId));
            }

            ShowLinkedDialog(adminPlayer.entityId, AdminTetherportFormatter.FormatRetieveMessage(allPlayers), "Retrieve Player!", RetrievePlayer);
        }

        private async void RetrievePlayer(int buttonIdx, string linkId, string inputContent, int playerId, int customValue)
        {
            if (string.IsNullOrWhiteSpace(linkId))
                return;

            var targetPlayerId = int.Parse(linkId);
            var adminPlayer = await QueryPlayerInfo(playerId);

            await TeleportPlayerToPlayer(targetPlayerId, adminPlayer.entityId);
            await MessagePlayer(adminPlayer.entityId, $"Teleported player to your location. EntityId: {targetPlayerId}.", 5);
        }
    }
}
