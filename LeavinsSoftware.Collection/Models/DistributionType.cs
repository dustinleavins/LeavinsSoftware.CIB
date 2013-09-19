// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Method of distribution.
    /// </summary>
    public enum DistributionType
    {
        /// <summary>
        /// Physical good - this is a physical comic book issue,
        /// a game cartridge, a disc, or anything that was not downloaded.
        /// </summary>
        Physical = 0,

        /// <summary>
        /// Digital download good.
        /// </summary>
        Digital = 1
    }
}
