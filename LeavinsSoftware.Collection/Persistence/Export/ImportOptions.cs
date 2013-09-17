// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    /// <summary>
    /// Import options
    /// </summary>
    public struct ImportOptions : IEquatable<ImportOptions>
    {
        public ImportOptions(bool merge)
        {
            Merge = merge;
        }

        public readonly bool Merge;
        
        public override bool Equals(object obj)
        {
            return (obj is ImportOptions) && Equals((ImportOptions)obj);
        }
        
        public bool Equals(ImportOptions other)
        {
            return this.Merge == other.Merge;
        }
        
        public override int GetHashCode()
        {
            int hashCode = 19;
            
            unchecked
            {
                hashCode += 23 * Merge.GetHashCode();
            }
            
            return hashCode;
        }
        
        public static bool operator ==(ImportOptions lhs, ImportOptions rhs)
        {
            return lhs.Equals(rhs);
        }
        
        public static bool operator !=(ImportOptions lhs, ImportOptions rhs)
        {
            return !(lhs == rhs);
        }

    }
}
