// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.Collections.Generic;

namespace LeavinsSoftware.Collection.Program
{
    public interface IFilePicker
    {
        string OpenFile(IEnumerable<string> extensions);

        string SaveFile(IEnumerable<string> extensions);
    }
}
