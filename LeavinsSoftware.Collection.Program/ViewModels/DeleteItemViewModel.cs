// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.Globalization;
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Persistence.Export;
using System;
using System.Windows.Input;
using LeavinsSoftware.Collection.Program.Resources;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="DeleteItemPage"/>.
    /// </summary>
    public sealed class DeleteItemViewModel : ViewModelBase
    {
        private DeleteItemViewModel(IAppNavigationService nav)
        {
            Nav = nav;
            
            Cancel = new CustomCommand(
                (x) => Nav.GoBack());
        }
        
        public static DeleteItemViewModel New<T>(T item, IAppNavigationService nav) where T : Item
        {
            if (Object.Equals(item, null))
            {
                throw new ArgumentNullException("item");
            }

            var viewModel = new DeleteItemViewModel(nav);
            
            viewModel.Delete = new CustomCommand(
                (x) =>
                {
                    Persistence.GetInstance<IPersistence<T>>().Delete(item);
                    BasicMessenger.Default.Send(MessageIds.App_ItemDeleted);
                });
            
            viewModel.DeleteConfirmMessage = string.Format(
                CultureInfo.InvariantCulture,
                InterfaceResources.DeleteItem_ConfirmMessageFormat,
                item.Name);
            
            return viewModel;
        }
        
        public IAppNavigationService Nav { get; private set; }
        
        public ICommand Delete { get; private set; }
        
        public ICommand Cancel { get; private set; }
        
        public string DeleteConfirmMessage
        {
            get;
            private set;
        }
    }
}