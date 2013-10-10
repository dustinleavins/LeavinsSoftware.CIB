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
        private static readonly DirectoryInfo ProgramDir = new DirectoryInfo(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            InterfaceResources.CompanyName,
            InterfaceResources.ProgramName));

        private static readonly Container Container = new Container();

        public static UpdateNotifier UpdateNotifier { get; private set; }

        public static void Setup()
        {
            Profile defaultProfile = new Profile("default");
            MigrationRunner.Run(ProgramDir, defaultProfile);

            Container.RegisterSingle<IComicBookPersistence>(new ComicBookPersistence(ProgramDir, defaultProfile));
            Container.RegisterSingle<IVideoGamePersistence>(new VideoGamePersistence(ProgramDir, defaultProfile));
            Container.RegisterSingle<IProductPersistence>(new ProductPersistence(ProgramDir, defaultProfile));
            Container.RegisterSingle<ICategoryPersistence>(new ItemCategoryPersistence(ProgramDir, defaultProfile));
            Container.RegisterSingle<IProgramOptionsPersistence>(new ProgramOptionsPersistence(
                new FileInfo(Path.Combine(ProgramDir.FullName, "options.xml"))));

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
