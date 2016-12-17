using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;
using UserSpecificFunctions.Framework.Permissions;

namespace UserSpecificFunctions.Framework
{
    /// <summary>Represents a <see cref="PlayerInfo"/> instance attached to a <see cref="TSPlayer"/> object.</summary>
    public class PlayerInfo
    {
        public const string Key = "UsfData_Key";

        /// <summary>Gets or sets the user's ID.</summary>
        public int UserID { get; set; }

        /// <summary>Gets or sets the prefix.</summary>
        public string Prefix { get; set; }

        /// <summary>Gets or sets the suffix.</summary>
        public string Suffix { get; set; }

        /// <summary>Gets or sets the chat color.</summary>
        public string Color { get; set; }

        /// <summary>Gets or sets the permissions.</summary>
        public PermissionList Permissions { get; set; }

        public PlayerInfo Parse(QueryResult reader)
        {
            //UserID = reader.Get<int>("UserID");
            //Prefix = reader.Get<string>("Prefix");
            //Suffix = reader.Get<string>("Suffix");
            //Color = reader.Get<string>("Color");
            //Permissions = 

            return new PlayerInfo()
            {
                UserID = reader.Get<int>("UserID"),
                Prefix = reader.Get<string>("Prefix"),
                Suffix = reader.Get<string>("Suffix"),
                Color = reader.Get<string>("Color"),
                Permissions = new PermissionList().ParsePermissions(reader.Get<string>("Permissions"))
            };
        }

        /// <summary>Initializes a new instance of the <see cref="PlayerInfo"/> class.</summary>
        public PlayerInfo()
        {
            UserID = -1;
            Prefix = null;
            Suffix = null;
            Color = null;
            Permissions = new PermissionList();
        }
    }
}
