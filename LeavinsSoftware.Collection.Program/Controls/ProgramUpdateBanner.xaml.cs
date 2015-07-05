// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Program.Controls.ViewModels;

namespace LeavinsSoftware.Collection.Program.Controls
{
    /// <summary>
    /// Interaction logic for ProgramUpdateBanner.xaml
    /// </summary>
    public partial class ProgramUpdateBanner : Banner
    {
        public ProgramUpdateBanner()
        {
            InitializeComponent();
            DataContext = new ProgramUpdateBannerViewModel(this);
        }
    }
}
