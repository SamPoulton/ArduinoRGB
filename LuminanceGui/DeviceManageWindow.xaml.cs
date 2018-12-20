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
using LuminanceLib;

namespace ArduinoRGBGui
{
    /// <summary>
    /// Interaction logic for DeviceManageWindow.xaml
    /// </summary>
    public partial class DeviceManageWindow : Window
    {
        public DeviceManageWindow()
        {
            InitializeComponent();
            DeviceList.SetColumnHeaders("Name", "Port", "Endpoints");
            foreach (RgbDevice device in RgbDeviceManager.Devices)
            {
                DeviceList.AddItem(device.DeviceName, device.PortName, device.Endpoints.Count.ToString());
            }
        }
    }
}
