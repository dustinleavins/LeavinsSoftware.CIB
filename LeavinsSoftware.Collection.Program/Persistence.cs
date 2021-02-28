// Copyright (c) 2013, 2014, 2021 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
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
        private static readonly DirectoryInfo ProgramDir = new DirectoryInfo(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            InterfaceResources.CompanyName,
            InterfaceResources.ProgramName));

        public static readonly Container Container = new Container();

        public static UpdateNotifier UpdateNotifier { get; private set; }

        public static void Setup()
        {
            Profile defaultProfile = new Profile("default");
            MigrationRunner.Run(ProgramDir, defaultProfile);

            var comicBookPersistence = new ComicBookPersistence(ProgramDir, defaultProfile);
            var videoGamePersistence = new VideoGamePersistence(ProgramDir, defaultProfile);
            var productPersistence = new ProductPersistence(ProgramDir, defaultProfile);
            
            Container.RegisterInstance<ICategoryPersistence>(new ItemCategoryPersistence(ProgramDir, defaultProfile));
            Container.RegisterInstance<IProgramOptionsPersistence>(new ProgramOptionsPersistence(
                new FileInfo(Path.Combine(ProgramDir.FullName, "options.xml"))));

            Container.RegisterInstance<ISearchablePersistence<ComicBookSeries>>(comicBookPersistence);
            Container.RegisterInstance<ISearchablePersistence<VideoGame>>(videoGamePersistence);
            Container.RegisterInstance<ISearchablePersistence<Product>>(productPersistence);
            
            Container.RegisterInstance<IPersistence<ComicBookSeries>>(comicBookPersistence);
            Container.RegisterInstance<IPersistence<VideoGame>>(videoGamePersistence);
            Container.RegisterInstance<IPersistence<Product>>(productPersistence);
            
            Container.RegisterInstance<IKeyValueStore>(new KeyValueStore(ProgramDir, defaultProfile));

            UpdateNotifier = new UpdateNotifier(GetInstance<IProgramOptionsPersistence>().Retrieve(),
                Assembly.GetExecutingAssembly().GetName().Version,
                new Uri("http://api.leavins-software.com/cib/version"));
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return Container.GetInstance<TService>();
        }
    }
}
