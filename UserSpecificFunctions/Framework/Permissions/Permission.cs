using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserSpecificFunctions.Framework.Permissions
{
    /// <summary>Provides implementation for the <see cref="IPermission"/> interface.</summary>
    public class Permission : IPermission
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public bool Negated => Name.StartsWith("!");

        /// <summary>Initializes a new instance of the <see cref="Permission"/> class.</summary>
        /// <param name="name">The permission's name.</param>
        /// <param name="negated">The permission's negated status.</param>
        public Permission(string name/*, bool negated*/)
        {
            Name = name;
            //Negated = negated;
        }

        /// <summary>Checks to see whether two <see cref="Permission"/> match each other.</summary>
        /// <param name="x">The first permission.</param>
        /// <param name="y">The second permission.</param>
        /// <returns>True or false.</returns>
        public static bool operator ==(Permission x, Permission y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if ((object)x == null || (object)y == null)
            {
                return false;
            }

            return x.Name == y.Name && x.Negated == y.Negated;
        }

        /// <summary>Checks to see whether two <see cref="Permission"/> match each other.</summary>
        /// <param name="x">The first permission.</param>
        /// <param name="y">The second permission.</param>
        /// <returns>True or false.</returns>
        public static bool operator !=(Permission x, Permission y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Permission;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
