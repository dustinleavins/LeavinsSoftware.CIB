// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Persistence.Migrations;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.Update;
using SimpleInjector;
using System;
using System.IO;
using System.Reflection;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Contains instances of every class responsible for persistence.
    /// </summary>
    public static class Persistence
    {
        public static UpdateNotifier UpdateNotifier { get; private set; }

        public static void Setup()
        {
            MigrationRunner.Run(DataDirectory, "default");

            Container = new Container();
            Container.RegisterSingle<IComicBookPersistence>(new ComicBookPersistence(DataDirectory, "default"));
            Container.RegisterSingle<IVideoGamePersistence>(new VideoGamePersistence(DataDirectory, "default"));
            Container.RegisterSingle<IProductPersistence>(new ProductPersistence(DataDirectory, "default"));
            Container.RegisterSingle<ICategoryPersistence>(new ItemCategoryPersistence(DataDirectory, "default"));
            Container.RegisterSingle<IProgramOptionsPersistence>(new ProgramOptionsPersistence(Path.Combine(DataDirectory, "options.xml")));

            UpdateNotifier = new UpdateNotifier(GetInstance<IProgramOptionsPersistence>().Retrieve(),
                Assembly.GetExecutingAssembly().GetName().Version,
                new Uri("http://api.leavins-software.com/cib/version"));
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return Container.GetInstance<TService>();
        }

        public static string DataDirectory
        {
            get
            {
                return dataDirectory.Value;
            }
        }

        public static Container Container
        {
            get;
            private set;
        }

        private static Lazy<string> dataDirectory = new Lazy<string>(() =>
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    InterfaceResources.CompanyName,
                    InterfaceResources.ProgramName);
            });
    }
}
