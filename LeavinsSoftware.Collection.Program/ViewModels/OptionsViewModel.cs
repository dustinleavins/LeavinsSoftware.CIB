// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class OptionsViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="formValidator">Func that returns true if the form is valid</param>
        public OptionsViewModel(IAppNavigationService nav, Func<bool> formValidator)
        {
            FormValidator = formValidator;

            Nav = nav;

            Options = Persistence.ProgramOptionsPersistence.Retrieve();

            CategoryEdit = new BasicCommand(
                () =>
                {
                    Nav.Navigate(() => new OptionsCategoryNamesPage());
                });

            Finish = new BasicCommand(
                // Execute
                () =>
                {
                    if (Options.IsValid() && formValidator.Invoke())
                    {
                        Persistence.ProgramOptionsPersistence.Update(Options);
                        Nav.GoBack();
                    }
                },

                // CanExecute
                () =>
                {
                    return Options.IsValid() && formValidator.Invoke();
                });
        }

        public IAppNavigationService Nav { get; private set; }

        public ProgramOptions Options { get; private set; }

        public ICommand CategoryEdit { get; private set; }

        public ICommand Finish { get; private set; }

        public Func<bool> FormValidator { get; private set; }
    }
}
