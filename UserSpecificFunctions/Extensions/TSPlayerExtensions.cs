using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using UserSpecificFunctions.Framework;

namespace UserSpecificFunctions.Extensions
{
    /// <summary>Provides extension methods for the <see cref="TSPlayer"/> type.</summary>
    public static class TSPlayerExtensions
    {
        /// <summary>Gets the <see cref="PlayerInfo"/> object attached to the <see cref="TSPlayer"/>.</summary>
        /// <param name="tsplayer">The <see cref="TSPlayer"/> object.</param>
        /// <returns>A <see cref="PlayerInfo"/> object.</returns>
        public static PlayerInfo GetPlayerInfo(this TSPlayer tsplayer)
        {
            if (!tsplayer.ContainsData(PlayerInfo.Key))
            {
                tsplayer.SetData(PlayerInfo.Key, new PlayerInfo());
            }

            return tsplayer.GetData<PlayerInfo>(PlayerInfo.Key);
        }
    }
}
