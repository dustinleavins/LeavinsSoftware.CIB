using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Represents a user profile.
    /// </summary>
    public struct Profile
    {
        public Profile(string name)
        {
            Name = name;
        }

        public readonly string Name;
    }
}
