#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using TShockAPI;
using TShockAPI.DB;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using UserSpecificFunctions.Framework;
using UserSpecificFunctions.Extensions;

namespace UserSpecificFunctions.Modules
{
    public enum DatabaseUpdate
    {
        Prefix, Suffix, ChatColor, Permissions
    }

    /// <summary>Provides database functionality.</summary>
    public class DatabaseModule : IModule
    {
        private IDbConnection db;
        private readonly UserSpecificFunctionsPlugin _plugin;

        /// <summary>Initializes a new instance of the <see cref="DatabaseModule"/> class.</summary>
        /// <param name="plugin">The <see cref="UserSpecificFunctionsPlugin"/> instance.</param>
        public DatabaseModule(UserSpecificFunctionsPlugin plugin)
        {
            _plugin = plugin;
        }

        /// <summary>Sets up the database.</summary>
        private void DBConnect()
        {
            switch (TShock.Config.StorageType.ToLower())
            {
                case "mysql":
                    string[] dbHost = TShock.Config.MySqlHost.Split(':');
                    db = new MySqlConnection()
                    {
                        ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                            dbHost[0],
                            dbHost.Length == 1 ? "3306" : dbHost[1],
                            TShock.Config.MySqlDbName,
                            TShock.Config.MySqlUsername,
                            TShock.Config.MySqlPassword)

                    };
                    break;

                case "sqlite":
                    string sql = Path.Combine(TShock.SavePath, "tshock.sqlite");
                    db = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
                    break;
            }

            SqlTableCreator sqlcreator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());

            sqlcreator.EnsureTableStructure(new SqlTable("UserSpecificFunctions",
                new SqlColumn("UserID", MySqlDbType.Int32),
                new SqlColumn("Prefix", MySqlDbType.Text),
                new SqlColumn("Suffix", MySqlDbType.Text),
                new SqlColumn("Color", MySqlDbType.Text),
                new SqlColumn("Permissions", MySqlDbType.Text)));
        }

        /// <inheritdoc/>
        public void Register()
        {
#if DEBUG
            Console.WriteLine("DatabaseModule Initialized...");
#endif
            DBConnect();
        }

        /// <inheritdoc/>
        public void Deregister()
        {

        }

        /// <summary>Returns a <see cref="PlayerInfo"/> object matching the player ID.</summary>
        /// <param name="playerId">The player's ID.</param>
        /// <returns>A <see cref="PlayerInfo"/> object.</returns>
        public PlayerInfo GetPlayerById(int playerId)
        {
            using (QueryResult reader = db.QueryReader("SELECT * FROM UserSpecificFunctions WHERE UserID = @0;", playerId.ToString()))
            {
                if (reader.Read())
                {
                    return new PlayerInfo().Parse(reader);
                }

                return null;
            }
        }

        /// <summary>Returns a <see cref="PlayerInfo"/> object matching the player ID as an asynchronous operation.</summary>
        /// <param name="playerId">The player's ID.</param>
        /// <returns>A task for this operation.</returns>
        public Task<PlayerInfo> GetPlayerByIdAsync(int playerId)
        {
            return Task.Run(() => GetPlayerById(playerId));
        }

        /// <summary>Inserts a player into the database as an asynchronous operation.</summary>
        /// <param name="player">The <see cref="PlayerInfo"/> object.</param>
        /// <returns>A task for this operation.</returns>
        public Task AddPlayerAsync(PlayerInfo player)
        {
            return Task.Run(() =>
            {

                string query = "INSERT INTO UserSpecificFunctions (UserID, Prefix, Suffix, Color, Permissions) VALUES (@0, @1, @2, @3, @4);";
                db.Query(query, player.UserID, player.Prefix, player.Suffix, player.Color,
                    player.Permissions.GetPermissions().Any() ? player.Permissions.GetPermissions().Join(",") : null);

            }).LogExceptions();
        }

        /// <summary>Updates a player's prefix as an asynchronous operation.</summary>
        /// <param name="playerId">The player's ID.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>A task for this operation.</returns>
        public Task SetPrefixAsync(int playerId, string prefix)
        {
            PlayerInfo playerInfo;

            return Task.Run(async () =>
            {

                TSPlayer tsplr = TShock.Players.FirstOrDefault(p => p?.User?.ID == playerId);
                if (tsplr != null)
                {
                    playerInfo = tsplr.GetPlayerInfo();
                }
                else
                {
                    playerInfo = await GetPlayerByIdAsync(playerId);
                }

                if (playerInfo == null)
                {
                    await AddPlayerAsync(new PlayerInfo() { UserID = playerId, Prefix = prefix });
                }
                else
                {
                    playerInfo.Prefix = prefix;
                    db.Query("UPDATE UserSpecificFunctions SET Prefix = @0 WHERE UserID = @1;", prefix, playerId.ToString());
                }

            }).LogExceptions();
        }

        public Task SetSuffixAsync(int playerId, string suffix)
        {
            PlayerInfo playerInfo;

            return Task.Run(async () =>
            {

                TSPlayer tsplr = TShock.Players.FirstOrDefault(p => p?.User?.ID == playerId);
                if (tsplr != null)
                {
                    playerInfo = tsplr.GetPlayerInfo();
                }
                else
                {
                    playerInfo = await GetPlayerByIdAsync(playerId);
                }

                if (playerInfo == null)
                {
                    await AddPlayerAsync(new PlayerInfo() { UserID = playerId, Suffix = suffix });
                }
                else
                {
                    playerInfo.Suffix = suffix;
                    db.Query("UPDATE UserSpecificFunctions SET Suffix = @0 WHERE UserID = @1;", suffix, playerId.ToString());
                }

            }).LogExceptions();
        }

        public Task SetColorAsync(int playerId, string color)
        {
            PlayerInfo playerInfo;

            return Task.Run(async () => 
            {

                TSPlayer tsplr = TShock.Players.FirstOrDefault(p => p?.User?.ID == playerId);
                if (tsplr != null)
                {
                    playerInfo = tsplr.GetPlayerInfo();
                }
                else
                {
                    playerInfo = await GetPlayerByIdAsync(playerId);
                }

                if (playerInfo == null)
                {
                    await AddPlayerAsync(new PlayerInfo() { UserID = playerId, Color = color });
                }
                else
                {
                    playerInfo.Color = color;
                    db.Query("UPDATE UserSpecificFunctions SET Color = @0 WHERE UserID = @1;", color, playerId.ToString());
                }

            }).LogExceptions();
        }

        public Task UpdatePlayer(PlayerInfo playerInfo, DatabaseUpdate updateType)
        {
            return Task.Run(() => 
            {

                //TSPlayer tsplr = TShock.Players.First(p => p?.User?.ID == playerId);
                //if (tsplr == null)
                //{
                //    playerInfo = tsplr.GetPlayerInfo();
                //}
                //else
                //{
                //    playerInfo = await GetPlayerByIdAsync(playerId);
                //}

                //switch (updateType)
                //{
                //    case DatabaseUpdate.Prefix:
                //        {

                //        }
                //        break;

                //    case DatabaseUpdate.Suffix:
                //        {

                //        }
                //        break;

                //    case DatabaseUpdate.ChatColor:
                //        {

                //        }
                //        break;

                //    case DatabaseUpdate.Permissions:
                //        {

                //        }
                //        break;
                //}
                if (updateType == 0)
                {
                    return;
                }

                StringBuilder _stringBuilder = new StringBuilder();
                if ((updateType & DatabaseUpdate.Prefix) == DatabaseUpdate.Prefix)
                {
                    _stringBuilder.Append($"Prefix = {playerInfo.Prefix}");
                }
                if ((updateType & DatabaseUpdate.Suffix) == DatabaseUpdate.Suffix)
                {
                    _stringBuilder.Append($"Suffix = {playerInfo.Suffix}");
                }
                if ((updateType & DatabaseUpdate.ChatColor) == DatabaseUpdate.ChatColor)
                {
                    _stringBuilder.Append($"Color = {playerInfo.Color}");
                }
                if ((updateType & DatabaseUpdate.Permissions) == DatabaseUpdate.Permissions)
                {
                    _stringBuilder.Append($"Permissions = {playerInfo.Permissions.GetPermissions().Join(",")}");
                }

                db.Query($"UPDATE UserSpecificFunctions SET {string.Join(", ", _stringBuilder.ToString())} WHERE UserID = {playerInfo.UserID}");

            }).LogExceptions();
        }
    }
}
