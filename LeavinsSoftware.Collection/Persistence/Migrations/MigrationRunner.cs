// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence.Migrations
{
    /// <summary>
    /// Responsible for running all SQLite database 
    /// migrations in the correct order.
    /// </summary>
    public static class MigrationRunner
    {
        /// <summary>
        /// List of migrations.
        /// </summary>
        public static IList<Migration> Migrations
        {
            get;
            private set;
        }

        static MigrationRunner()
        {
            Migrations = new List<Migration>
                    {
                        new InitialSetup(),
                        new AddListTypeColumn(),
                        new RequireIssue(),
                        new KeyValueStoreSetup()
                    };
        }

        /// <summary>
        /// Runs all migrations, in order.
        /// </summary>
        /// <param name="databaseFileName"></param>
        public static void Run(DirectoryInfo dataDir, Profile profile)
        {
            string collectionDatabasePath = Path.Combine(dataDir.FullName,
                profile.Name,
                "collection.db");
            
            string connectionString = string.Format(
                CultureInfo.InvariantCulture,
                "Data Source=|DataDirectory|{0}",
                collectionDatabasePath);

            bool outdatedProgramSchema = false;

            if (!Directory.Exists(Path.Combine(dataDir.FullName, profile.Name)))
            {
                Directory.CreateDirectory(Path.Combine(dataDir.FullName, profile.Name));
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                long version = DatabaseSchemaVersion(connection);

                if (version == 0)
                {
                    (new MigrationZero()).Run(connection, version);
                    Update(version, Migrations.Count, connection);
                }
                else if (version < MigrationsSchemaVersion)
                {
                    foreach (var migration in Migrations)
                    {
                        migration.Run(connection, version);
                    }

                    Update(version, Migrations.Count, connection);
                }
                else if (version > Migrations.Count)
                {
                    outdatedProgramSchema = true;
                }

                connection.Close();
            }

            if (outdatedProgramSchema)
            {
                throw new ApplicationException();
            }
        }

        /// <summary>
        /// Returns the schema version of the given SQLite database file.
        /// </summary>
        /// <remarks>
        /// If the schema version could not be found, the schema version
        /// table is created and a default value of 0 is returned.
        /// </remarks>
        /// <param name="databaseFileName"></param>
        /// <returns></returns>
        public static long DatabaseSchemaVersion(string databaseFileName)
        {
            long version = 0;
            string connectionString = "Data Source=|DataDirectory|" + databaseFileName;

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                version = DatabaseSchemaVersion(connection);
                connection.Close();
            }

            return version;
        }

        public static long MigrationsSchemaVersion
        {
            get
            {
                return Migrations.Max(m => m.SchemaVersion);
            }
        }

        private static long DatabaseSchemaVersion(SQLiteConnection connection)
        {
            long version = 0;

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT value FROM SchemaInfo " +
                    "WHERE key = @key;";

                cmd.Parameters.Add(new SQLiteParameter("@key", "version"));

                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string valueAsString = reader["value"] as string;
                            version = long.Parse(valueAsString, CultureInfo.InvariantCulture);
                        }
                    }
                }
                catch (SQLiteException) // no such table
                {
                    CreateSchemaInfoTable(connection);
                }
            }

            return version;
        }

        private static void CreateSchemaInfoTable(SQLiteConnection connection)
        {
            SQL.CreateTable("SchemaInfo")
                .Column("key", "text", "PRIMARY KEY")
                .Column("value", "text")
                .ExecuteWith(connection);
        }

        private static void Update(long currentSchemaVersion,
                                   long targetSchemaVersion,
                                   SQLiteConnection connection)
        {
            if (currentSchemaVersion == 0) // schema version is not yet persisted
            {
                SQL.InsertInto("SchemaInfo")
                    .Column("key", "version")
                    .Column("value", targetSchemaVersion.ToString(CultureInfo.InvariantCulture))
                    .ExecuteWith(connection);
            }
            else
            {
                SQL.Update("SchemaInfo")
                    .Set("value", targetSchemaVersion.ToString(CultureInfo.InvariantCulture))
                    .WhereEquals("key", "version")
                    .ExecuteWith(connection);
            }
        }
    }
}
