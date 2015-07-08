// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Program
{
    public sealed class FilePicker : IFilePicker
    {
        public string OpenFile(IEnumerable<string> extensions)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = GenerateFilter(extensions);
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        public string SaveFile(IEnumerable<string> extensions)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = GenerateFilter(extensions);
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private static string GenerateFilter(IEnumerable<string> extensions)
        {
            List<string> filterParts = new List<string>();
            foreach (string extension in extensions)
            {
                filterParts.Add(string.Format("{0} files (*.{0})|*.{0}", extension));
            }

            filterParts.Add("All Files (*.*)|*.*");
            return string.Join("|", filterParts);
        }
    }
}
