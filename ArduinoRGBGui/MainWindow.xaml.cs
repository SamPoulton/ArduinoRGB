using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArduinoRGBLib;
using ArduinoRGBLib.Exceptions;

namespace ArduinoRGBGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void AddItemsToList()
        {
            try
            {
                foreach (RgbDevice device in RgbDeviceManager.Devices)
                {
                    foreach (RgbEndpoint endpoint in device.Endpoints)
                    {
                        DeviceEntryPanel.Items.Add(new ListViewEntry(endpoint));
                    }
                }
            }
            catch (TypeInitializationException e) when (e.InnerException is NoConfigException)
            {
                throw new NoConfigException();
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AddItemsToList();
            }
            catch (NoConfigException)
            {
                MessageBox.Show(this, "Your configuration file could not be found. The program now must close.", "", MessageBoxButton.OK);
            }
        }
    }
}
