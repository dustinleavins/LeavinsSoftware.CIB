// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Persistence.Migrations;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.Update;
using System;
using System.IO;
using System.Reflection;

namespace LeavinsSoftware.Collection.Program
{
    public static class Persistence
    {
        public static IComicBookPersistence ComicPersistence { get; private set; }

        public static IVideoGamePersistence GamePersistence { get; private set; }

        public static IProductPersistence ProductPersistence { get; private set; }

        public static ICategoryPersistence CategoryPersistence { get; private set; }

        public static IProgramOptionsPersistence ProgramOptionsPersistence { get; private set; }

        public static UpdateNotifier UpdateNotifier { get; private set; }

        public static void Setup()
        {
            MigrationRunner.Run(DataDirectory, "default");
            ComicPersistence = new ComicBookPersistence(DataDirectory, "default");
            GamePersistence = new VideoGamePersistence(DataDirectory, "default");
            ProductPersistence = new ProductPersistence(DataDirectory, "default");
            CategoryPersistence = new ItemCategoryPersistence(DataDirectory, "default");
            ProgramOptionsPersistence = new ProgramOptionsPersistence(Path.Combine(DataDirectory, "options.xml"));

            UpdateNotifier = new UpdateNotifier(ProgramOptionsPersistence.Retrieve(),
                Assembly.GetExecutingAssembly().GetName().Version,
                new Uri("http://api.leavins-software.com/cib/version"));
        }

        public static string DataDirectory
        {
            get
            {
                return dataDirectory.Value;
            }
        }

        private static Lazy<string> dataDirectory = new Lazy<string>(() =>
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    InterfaceResources.CompanyName,
                    InterfaceResources.ProgramName);
            });
    }
}
