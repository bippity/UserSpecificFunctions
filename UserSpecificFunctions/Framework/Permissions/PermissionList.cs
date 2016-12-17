using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserSpecificFunctions.Framework.Permissions
{
    public class PermissionList
    {
        private static List<IPermission> _permissions;

        /// <summary>Initializes a new instance of the <see cref="PermissionList"/> class.</summary>
        public PermissionList()
        {
            _permissions = new List<IPermission>();
        }

        /// <summary>Initializes a new instance of the <see cref="PermissionList"/> class.</summary>
        /// <param name="permissions">The permission list.</param>
        public PermissionList(List<IPermission> permissions)
        {
            _permissions = permissions;
        }

        /// <summary>Parses a <see cref="Permission"/> from the given permission string.</summary>
        /// <param name="permissionString">The permission string.</param>
        /// <returns>A <see cref="PermissionList"/> object.</returns>
        public PermissionList ParsePermissions(string permissionString)
        {
            //_permissions = new PermissionList().Parse("");
            //return new PermissionList(_permissions);
            return new PermissionList(Parse(permissionString));
        }

        /// <summary>Adds a permission to the list.</summary>
        /// <param name="permission">The <see cref="Permission"/> instance.</param>
        public void AddPermission(IPermission permission)
        {
            if (string.IsNullOrWhiteSpace(permission.Name))
            {
                return;
            }

            if (_permissions.Any(p => p == permission))
            {
                return;
            }

            _permissions.Add(permission);
        }

        /// <summary>Removes a permission from the list.</summary>
        /// <param name="permission">The <see cref="Permission"/> instance.</param>
        public void RemovePermission(IPermission permission)
        {
            if (string.IsNullOrWhiteSpace(permission.Name))
            {
                return;
            }

            if (_permissions.Any(p => p == permission))
            {
                _permissions.Remove(permission);
            }
        }

        /// <summary>Checks whether the permission list contains a specific permission.</summary>
        /// <param name="permission">The permission.</param>
        /// <returns>True or false.</returns>
        public bool HasPermission(IPermission permission)
        {
            if (permission.Negated)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(permission.Name))
            {
                return true;
            }

            if (_permissions.Any(p => p == permission))
            {
                return true;
            }

            string[] nodes = permission.Name.Split('.');
            for (int i = nodes.Length - 1; i >= 0; i--)
            {
                nodes[i] = "*";
                if (_permissions.Any(p => p.Name == string.Join(".", nodes, 0, i + 1)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Returns a list of permissions.</summary>
        /// <returns>A list of permissions.</returns>
        public List<IPermission> GetPermissions()
        {
            return _permissions;
        }

        /// <summary>Adds a permission to the list.</summary>
        /// <param name="permission">The permission string.</param>
        public void AddPermission(string permission)
        {
            //if (_permissions.Any(p => p.Name == permission))
            //{
            //    return;
            //}

            //_permissions.Add(new Permission(permission));

            AddPermission(new Permission(permission));
        }

        /// <summary>Removes a permission from the list.</summary>
        /// <param name="permission">The permission string.</param>
        public void RemovePermission(string permission)
        {
            //if (_permissions.Any(p => p.Name == permission))
            //{
            //    _permissions.Remove(new Permission(permission));
            //}

            RemovePermission(new Permission(permission));
        }

        /// <summary>Checks whether the permission list contains</summary>
        /// <param name="permission">The permission string.</param>
        /// <returns>True or false.</returns>
        public bool HasPermission(string permission)
        {
            return HasPermission(new Permission(permission));
        }

        /// <summary>Parses the permission list into an <see cref="IPermission"/> list.</summary>
        /// <param name="permissions">The permission string.</param>
        private List<IPermission> Parse(string permissions)
        {
            if (string.IsNullOrWhiteSpace(permissions))
            {
                return new List<IPermission>();
            }

            _permissions = new List<IPermission>();
            string[] _permissionArray = string.Join(",", permissions).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string permission in _permissionArray)
            {
                AddPermission(permission);
            }

            return _permissions;
        }
    }
}
