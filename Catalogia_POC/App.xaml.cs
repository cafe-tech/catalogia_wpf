using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Catalogia_POC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Say you wanted to implement the MVVM programmatically
        /*
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LibraryDemo w = new Museum_POC.LibraryDemo();
            MuseumDemoViewModel mvvm = new MuseumDemoViewModel();
            w.DataContext = mvvm;
            w.Show();
        }
        */
    }
}
