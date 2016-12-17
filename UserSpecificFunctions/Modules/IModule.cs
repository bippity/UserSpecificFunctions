using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserSpecificFunctions.Modules
{
    /// <summary>Describes the basic structure of a module.</summary>
    internal interface IModule
    {
        /// <summary>Registers the module.</summary>
        void Register();

        /// <summary>Deregisters the module.</summary>
        void Deregister();
    }
}
