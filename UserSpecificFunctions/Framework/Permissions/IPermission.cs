using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserSpecificFunctions.Framework.Permissions
{
    /// <summary>Represents a <see cref="IPermission"/>.</summary>
    public interface IPermission
    {
        /// <summary>Gets or sets the permission's name.</summary>
        string Name { get; set; }

        /// <summary>Gets the permission's negated status.</summary>
        bool Negated { get; }
    }
}
