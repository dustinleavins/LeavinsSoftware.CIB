// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence
{
    public interface IProgramOptionsPersistence
    {
        void Update(ProgramOptions options);

        ProgramOptions Retrieve();
    }
}
