// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence.Migrations
{
    /// <summary>
    /// Base class for SQLite database migrations.
    /// </summary>
    public abstract class Migration
    {
        /// <summary>
        /// Runs this migration if the given schema version is below
        /// this schema version of the migration.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="schemaVersion"></param>
        public void Run(SQLiteConnection connection, long schemaVersion)
        {
            if (schemaVersion >= SchemaVersion)
            {
                return;
            }
            else
            {
                Up(connection);
            }
        }

        /// <summary>
        /// Perform the migration; there is currently no Down/Reverse method.
        /// </summary>
        /// <param name="connection"></param>
        protected abstract void Up(SQLiteConnection connection);

        /// <summary>
        /// Schema version represented by this migration.
        /// </summary>
        public abstract long SchemaVersion { get; }
    }
}
