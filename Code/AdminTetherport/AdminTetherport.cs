using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;
using EmpyrionModdingFramework;
using EmpyrionModdingFramework.Database;
using InventoryManagement;
using ModLocator;
using Tetherporter;

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
            CommandManager.CommandList.Add(new ChatCommand("admintetherport", AdminTetherporter, PlayerPermission.Admin, paramCount: 1));
            CommandManager.CommandList.Add(new ChatCommand("attp", AdminTetherporter, PlayerPermission.Admin, 1));
            CommandManager.CommandList.Add(new ChatCommand("uattp", Untether, PlayerPermission.Admin));
        }

        private async Task AdminTetherporter(MessageData messageData, object[] parameters)
        {
            PlayerInfo player = await QueryPlayerInfo(messageData.SenderEntityId);
            if (player == null) return;

            // Create Tetherport record
            var record = new TetherporterRecord(player.steamId, player.entityId, player.playfield, player.pos, player.rot);
            _dbManager.SaveRecord(FormatRecordId(player.steamId), record);

            // Parse Target EntityId
            if(!ChatCommand.ParseIntParam(parameters, 0, out var targetEntityId))
            {
                await MessagePlayer(player.entityId, $"Could not parse integer EntityId from parameter.", 5);
                return;
            }

            // Get Target player info
            PlayerInfo target = await QueryPlayerInfo(targetEntityId);
            if (target == null) return;

            // Teleport Admin to Player & inform them
            await TeleportPlayer(player.entityId, target.playfield, target.pos.x, target.pos.y, target.pos.z, target.rot.x, target.rot.y, target.rot.z);
            await MessagePlayer(player.entityId, $"Created Admin Tetherporter tether! Telported to {target.playerName}::{targetEntityId}.", 5);
        }

        private async Task Untether(MessageData messageData)
        {
            PlayerInfo player = await QueryPlayerInfo(messageData.SenderEntityId);
            if (player == null) return;

            if (!_dbManager.LoadRecord<TetherporterRecord>(FormatRecordId(player.steamId), out var tetherporterRecord))
            {
                Log($"Entity {player.entityId}/{player.playerName} requester Untether but no tether was found.");
                await MessagePlayer(player.entityId, $"No tether was found.", 5, MessagerPriority.Red);
                return;
            }

            await TeleportPlayer(player.entityId, tetherporterRecord.Playfield,
                tetherporterRecord.PosX, tetherporterRecord.PosY, tetherporterRecord.PosZ,
                tetherporterRecord.RotX, tetherporterRecord.RotY, tetherporterRecord.RotZ);
        }

        private string FormatRecordId(string steamId)
        {
            return $"{steamId}.tether";
        }
    }
}
