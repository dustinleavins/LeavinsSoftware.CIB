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
            Profile defaultProfile = new Profile("default");
            MigrationRunner.Run(DataDirectory, defaultProfile);
            
            Container = new Container();
            Container.RegisterSingle<IComicBookPersistence>(new ComicBookPersistence(DataDirectory, defaultProfile));
            Container.RegisterSingle<IVideoGamePersistence>(new VideoGamePersistence(DataDirectory, defaultProfile));
            Container.RegisterSingle<IProductPersistence>(new ProductPersistence(DataDirectory, defaultProfile));
            Container.RegisterSingle<ICategoryPersistence>(new ItemCategoryPersistence(DataDirectory, defaultProfile));
            Container.RegisterSingle<IProgramOptionsPersistence>(new ProgramOptionsPersistence(
                new FileInfo(Path.Combine(DataDirectory.FullName, "options.xml"))));

            UpdateNotifier = new UpdateNotifier(GetInstance<IProgramOptionsPersistence>().Retrieve(),
                Assembly.GetExecutingAssembly().GetName().Version,
                new Uri("http://api.leavins-software.com/cib/version"));
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return Container.GetInstance<TService>();
        }

        public static DirectoryInfo DataDirectory
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

        private static Lazy<DirectoryInfo> dataDirectory = new Lazy<DirectoryInfo>(() =>
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    InterfaceResources.CompanyName,
                    InterfaceResources.ProgramName);

                return new DirectoryInfo(path);
            });
    }
}
