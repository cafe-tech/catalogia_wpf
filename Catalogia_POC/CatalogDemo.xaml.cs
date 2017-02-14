/*
    Catalog WPF 0.7
    Copyright © 2016-2017, Michael Francisco / FCS. All rights reserved.
  
    Catalog WPF is licensed under the terms of the GPLv2
    <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>.

    This program is free software; you can redistribute it and/or modify 
    it under the terms of the GNU General Public License as published 
    by the Free Software Foundation; version 2 of the License.

    This script is distributed in the hope that it will be useful, but 
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
    or FITNESS FOR A PARTICULAR BUSINESS MODEL. See the GNU General Public License 
    for more details. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace Catalogia_POC
{
    /// <summary>
    /// Interaction logic for LibraryDemo.xaml
    /// </summary>
    public partial class LibraryDemo : Window
    {
        // MuseumData md;
        // ObjectsInCollection _currentRecord;

        CatalogDemoViewModel mvvm;
        public LibraryDemo()
        {
            InitializeComponent();

            mvvm = new CatalogDemoViewModel();

            this.DataContext = mvvm;
            mvvm.Run("Reset");
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            btnAdd.IsEnabled = false;
            btnSave.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = false;
            btnAdd.IsEnabled = true;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCategory.SelectedValue != null)
            {
                mvvm.CurrentCategoryId = cmbCategory.SelectedValue.ToString();
            }
        }
    }
}
