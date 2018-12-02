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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArduinoRGBLib;

namespace ArduinoRGBGui
{
    /// <summary>
    /// Interaction logic for ListViewEntry.xaml
    /// </summary>
    public partial class ListViewEntry : UserControl
    {
        public readonly RgbEndpoint Endpoint;

        public ListViewEntry(RgbEndpoint endpoint)
        {
            Endpoint = endpoint;
            InitializeComponent();
            DeviceName.Content = endpoint.Name;
            ParentName.Content = endpoint.Parent.DeviceName;
            IndexName.Content = "Index " + endpoint.Index;
        }
    }
}
