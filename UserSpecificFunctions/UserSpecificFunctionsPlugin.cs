using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using UserSpecificFunctions.Modules;
using UserSpecificFunctions.Framework;
using UserSpecificFunctions.Extensions;

namespace UserSpecificFunctions
{
    /// <summary>Represents the UserSpecificFunctions plugin.</summary>
    [ApiVersion(1, 25)]
    public class UserSpecificFunctionsPlugin : TerrariaPlugin
    {
        private static string _configPath => Path.Combine(TShock.SavePath, "UserSpecificFunctions.json");

        /// <summary>The <see cref="Modules.CommandModule"/> instance.</summary>
        public CommandModule CommandModule { get; private set; }

        /// <summary>The <see cref="Modules.DatabaseModule"/> instance.</summary>
        public DatabaseModule DatabaseModule { get; private set; }

        /// <summary>The configuration file instance.</summary>
        public Configuration Configuration { get; private set; }

        /// <summary>Gets the author of this plugin.</summary>
        public override string Author => "Flash Slothmore";

        /// <summary>Gets the name of this plugin.</summary>
        public override string Name => "User Specific Functions";

        /// <summary>Gets the description of this plugin.</summary>
        public override string Description => "N/A";

        /// <summary>Gets the version of this plugin.</summary>
        public override Version Version => new Version(1, 0, 0, 0);

        /// <summary>Initializes a new instance of the <see cref="UserSpecificFunctionsPlugin"/> class.</summary>
        /// <param name="game">The <see cref="Main"/> instance.</param>
        public UserSpecificFunctionsPlugin(Main game) : base(game)
        {
            CommandModule = new CommandModule(this);
            DatabaseModule = new DatabaseModule(this);
        }

        #region Initialize/Dispose
        /// <summary>Initializes the plugin.</summary>
        public override void Initialize()
        {
            LoadConfig();

            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            TShockAPI.Hooks.GeneralHooks.ReloadEvent += OnReload;

            //CommandModule.Register();
            //DatabaseModule.Register();
            RegisterModules();
        }

        /// <summary>Disposes the plugin.</summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
                TShockAPI.Hooks.GeneralHooks.ReloadEvent -= OnReload;

                //CommandModule.Deregister();
                //DatabaseModule.Deregister();
                DeregisterModules();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Hooks
        private void OnChat(ServerChatEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            TSPlayer tsplr = TShock.Players[e.Who];
            if (tsplr == null || tsplr.GetPlayerInfo() == null)
            {
                return;
            }

            if (!e.Text.StartsWith(TShock.Config.CommandSilentSpecifier) && !e.Text.StartsWith(TShock.Config.CommandSilentSpecifier))
            {
                string prefix = tsplr.GetPlayerInfo().Prefix?.ToString() ?? tsplr.Group.Prefix;
                string suffix = tsplr.GetPlayerInfo().Suffix?.ToString() ?? tsplr.Group.Suffix;
                Color chatColor = tsplr.GetPlayerInfo().Color?.ToColor() ?? tsplr.Group.ChatColor.ToColor();

                if (!TShock.Config.EnableChatAboveHeads)
                {
                    string message = string.Format(TShock.Config.ChatFormat, tsplr.Group.Name, prefix, tsplr.Name, suffix, e.Text);
                    TSPlayer.All.SendMessage(message, chatColor);
                    TSPlayer.Server.SendMessage(message, chatColor);
                    TShock.Log.Info($"Broadcast: {message}");

                    e.Handled = true;
                }
                else
                {
                    Player player = Main.player[e.Who];
                    string name = player.name;
                    player.name = string.Format(TShock.Config.ChatAboveHeadsFormat, tsplr.Group.Name, prefix, tsplr.Name, suffix);
                    NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, player.name, e.Who, 0, 0, 0, 0);
                    player.name = name;
                    var text = e.Text;
                    NetMessage.SendData((int)PacketTypes.ChatText, -1, e.Who, text, e.Who, chatColor.R, chatColor.G, chatColor.B);
                    NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, name, e.Who, 0, 0, 0, 0);

                    string message = string.Format("<{0}> {1}", string.Format(TShock.Config.ChatAboveHeadsFormat, tsplr.Group.Name, prefix, tsplr.Name, suffix), text);
                    tsplr.SendMessage(message, chatColor);
                    TSPlayer.Server.SendMessage(message, chatColor);
                    TShock.Log.Info("Broadcast: {0}", message);

                    e.Handled = true;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(e.Text.Substring(1)))
                {
                    try
                    {
                        e.Handled = CommandModule.ExecuteCommand(tsplr, e.Text);
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.ConsoleError("An exception occured executing a command.");
                        TShock.Log.Error(ex.ToString());
                    }
                }
            }
        }

        private void OnReload(TShockAPI.Hooks.ReloadEventArgs e)
        {
            LoadConfig();
        }
        #endregion

        #region Module Registration
        private void RegisterModules()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule))))
            {
                type.GetMethod("Register").Invoke(Activator.CreateInstance(type, this), null);
            }
        }

        private void DeregisterModules()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule))))
            {
                type.GetMethod("Deregister").Invoke(Activator.CreateInstance(type, this), null);
            }
        }
        #endregion

        #region LoadConfig
        private void LoadConfig()
        {
            Configuration = Configuration.ReadOrWrite(_configPath);
        }
        #endregion
    }
}
