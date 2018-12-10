using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LuminanceLib;
using LuminanceLib.States;
using LuminanceLib.Exceptions;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace LuminanceGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RgbEndpoint _endpoint;

        public RgbEndpoint CurrentEndpoint
        {
            get => _endpoint;
            set
            {
                _endpoint = value;
                if (value.State is Solid) StateSelect.SelectedIndex = 0;
                if (value.State is Gradient) StateSelect.SelectedIndex = 1;
            }
        }

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

        private void DeviceEntryPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEndpoint = ((ListViewEntry) e.AddedItems[0]).Endpoint;
        }

        private void StateSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // remove all children
            while (true)
            {
                try
                {
                    EditPanelCanvas.Children.RemoveAt(0);
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
            if (((ListBoxItem) e.AddedItems[0]).Content.ToString() == "Static")
            {
                if (CurrentEndpoint.State is Solid)
                {
                    EditPanelCanvas.Children.Add(new StaticEditPanel((Solid) CurrentEndpoint.State));
                    MainPanelTab.SelectedIndex = 0;
                } else if (CurrentEndpoint.State is Gradient)
                {
                    EditPanelCanvas.Children.Add(new StaticEditPanel(new Solid(255, 255, 255, CurrentEndpoint), false)); //TODO
                    MainPanelTab.SelectedIndex = 0;
                }
            } else if (((ListBoxItem) e.AddedItems[0]).Content.ToString() == "Gradient")
            {
                 if (CurrentEndpoint.State is Solid)
                {
                    EditPanelCanvas.Children.Add(new GradientEditPanel(new Gradient(0, 255, 255, 255, 10, CurrentEndpoint), false));
                    MainPanelTab.SelectedIndex = 0;
                }
                else if (CurrentEndpoint.State is Gradient)
                {
                    EditPanelCanvas.Children.Add(new GradientEditPanel((Gradient)CurrentEndpoint.State));
                    MainPanelTab.SelectedIndex = 0;
                }
            }
        }
    }
}
