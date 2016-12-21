﻿#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.Hooks;
using UserSpecificFunctions.Extensions;

namespace UserSpecificFunctions.Modules
{
    /// <summary>Provides command functionality.</summary>
    public class CommandModule : IModule
    {
        private readonly UserSpecificFunctionsPlugin _plugin;

        /// <summary>Initializes a new instance of the <see cref="CommandModule"/> class.</summary>
        /// <param name="plugin">The <see cref="UserSpecificFunctionsPlugin"/> instance.</param>
        public CommandModule(UserSpecificFunctionsPlugin plugin)
        {
            _plugin = plugin;
        }

        /// <inheritdoc/>
        public void Register()
        {
#if DEBUG
            Console.WriteLine("CommandModule Initialized...");
#endif
            Commands.ChatCommands.Add(new Command(UserSpecificFunctionsCommand, "us"));
        }

        /// <inheritdoc/>
        public void Deregister()
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == UserSpecificFunctionsCommand);
        }

        private static void RunCommand(Command command, string message, TSPlayer tsplayer, List<string> args)
        {
            try
            {
                CommandDelegate commandD = command.CommandDelegate;
                commandD.Invoke(new CommandArgs(message, tsplayer, args));
            }
            catch (Exception ex)
            {
                tsplayer.SendErrorMessage("Command failed, check logs for more details.");
                TShock.Log.Error(ex.ToString());
            }
        }

        public bool ExecuteCommand(TSPlayer player, string text)
        {
            string cmdText = text.Remove(0, 1);
            string cmdPrefix = text[0].ToString();
            bool silent = false;

            if (cmdPrefix == TShock.Config.CommandSilentSpecifier)
            {
                silent = true;
            }

            List<string> args = typeof(Commands).InvokePrivateMethod<List<string>>("ParseParameters", cmdText);
            if (args.Count < 1)
            {
                return false;
            }

            string cmdName = args[0].ToLower();
            args.RemoveAt(0);

            IEnumerable<Command> cmds = Commands.ChatCommands.FindAll(c => c.HasAlias(cmdName));

            if (PlayerHooks.OnPlayerCommand(player, cmdName, cmdText, args, ref cmds, cmdPrefix))
                return true;

            if (cmds.Count() == 0)
            {
                if (player.AwaitingResponse.ContainsKey(cmdName))
                {
                    Action<CommandArgs> call = player.AwaitingResponse[cmdName];
                    player.AwaitingResponse.Remove(cmdName);
                    call(new CommandArgs(cmdText, player, args));
                    return true;
                }
                player.SendErrorMessage("Invalid command entered. Type {0}help for a list of valid commands.", TShock.Config.CommandSpecifier);
                return true;
            }
            foreach (Command command in cmds)
            {
                //if (!command.AllowServer && !player.RealPlayer)
                //{
                //    player.SendErrorMessage("You must use this command in-game.");
                //}
                //else if ((!command.CanRun(player) && !player.GetPlayerInfo().HasPermission(command.Permissions.Any() ? command.Permissions[0] : null)) || (command.CanRun(player) && player.GetPlayerInfo().Permissions.Negated(command.Permissions.Any() ? command.Permissions[0] : null)))
                //{
                //    TShock.Utils.SendLogs(string.Format("{0} tried to execute {1}{2}.", player.Name, TShock.Config.CommandSpecifier, cmdText), Color.PaleVioletRed, player);
                //    player.SendErrorMessage("You do not have access to this command.");
                //}
                //else
                //{
                //    if (command.DoLog)
                //        TShock.Utils.SendLogs(string.Format("{0} executed: {1}{2}.", player.Name, silent ? TShock.Config.CommandSilentSpecifier : TShock.Config.CommandSpecifier, cmdText), Color.PaleVioletRed, player);
                //    RunCommand(command, cmdText, player, args);
                //}
                if (!command.AllowServer && !player.RealPlayer)
                {
                    player.SendErrorMessage("You must use this command in-game.");
                }
                else if ((!command.CanRun(player) && !player.GetPlayerInfo().Permissions.HasPermission(command.Permissions.Any() ? command.Permissions[0] : null)
                    || (command.CanRun(player) && !player.GetPlayerInfo().Permissions.HasPermission(command.Permissions.Any() ? command.Permissions[0] : null))))
                {
                    TShock.Utils.SendLogs(string.Format("{0} tried to execute {1}{2}.", player.Name, TShock.Config.CommandSpecifier, cmdText), Color.PaleVioletRed, player);
                    player.SendErrorMessage("You do not have access to this command.");
                }
                else
                {
                    if (command.DoLog)
                        TShock.Utils.SendLogs(string.Format("{0} executed: {1}{2}.", player.Name, silent ? TShock.Config.CommandSilentSpecifier : TShock.Config.CommandSpecifier, cmdText), Color.PaleVioletRed, player);
                    RunCommand(command, cmdText, player, args);
                }
            }

            return true;
        }

        private static void UserSpecificFunctionsCommand(CommandArgs e)
        {
            if (e.Parameters.Count < 1 || e.Parameters.Count > 3 || e.Parameters[0].ToLower() == "help")
            {
                //e.Player.SendErrorMessage("Invalid syntax! Proper syntax:");
                //e.Player.SendErrorMessage($"{Commands.Specifier}us prefix <player name> <prefix>");
                //e.Player.SendErrorMessage($"{Commands.Specifier}us suffix <player name> <suffix>");
                //e.Player.SendErrorMessage($"{Commands.Specifier}us color <player name> <color>");
                //return;
                SendInvalidSyntax(e);
                return;
            }
        }

        private static void SendInvalidSyntax(CommandArgs e)
        {
            Dictionary<string, string> help = new Dictionary<string, string>()
            {
                { "prefix", "Sets the player's chat prefix" },
                { "suffix", "Sets the player's chat suffix" },
                { "color", "Sets the player's chat color" },
                { "remove", "Removes the player's (pre/suf)fix or chat color" },
                { "reset", "Resets the player's chat information" },
                { "purge", "Removes all empty entries from the database" },
                { "read", "Outputs the player's chat information" }
            };

            int pageNum;
            if (!PaginationTools.TryParsePageNumber(e.Parameters, 1, e.Player, out pageNum))
            {
                if (!help.ContainsKey(e.Parameters[1]))
                {
                    e.Player.SendErrorMessage("Invalid command name or page provided.");
                    return;
                }

                e.Player.SendInfoMessage($"Sub-command: {e.Parameters[1]}");
                e.Player.SendInfoMessage($"Help: {help[e.Parameters[1]]}");
            }
            else
            {
                PaginationTools.SendPage(e.Player, pageNum, help.Keys.Select(p => $"{p} - {help[p]}"), help.Count,
                    new PaginationTools.Settings()
                    {
                        HeaderFormat = $"UserSpecificFunctions Help({{0}}/{{1}})",
                        FooterFormat = $"Type {Commands.Specifier}us help {{0}} for more."
                    });
            }
        }
    }
}