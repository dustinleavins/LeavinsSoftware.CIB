// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    public struct ImportOptions
    {
        public ImportOptions(bool merge)
        {
            Merge = merge;
        }

        public readonly bool Merge;
    }
}
