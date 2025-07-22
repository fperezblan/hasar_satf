using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace HASAR_SATF.Services
{
    public static class Export
    {
        public static void ToFile(string extension, string filter, string defaultName, object content)
        {
            string fileName;
            var dialog = new SaveFileDialog
            {
                FileName = defaultName,
                DefaultExt = extension,
                Filter = filter
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                fileName = dialog.FileName;
            }
            else
            {
                MessageBox.Show("Debes seleccionar una ubicación para el archivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            File.WriteAllText(fileName, content.ToString(), Encoding.UTF8);
        }
    }
}
