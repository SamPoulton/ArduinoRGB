using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ArduinoRGBGui;
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
        private DeviceManageWindow manageWindow = new DeviceManageWindow();

        public RgbEndpoint CurrentEndpoint
        {
            get => _endpoint;
            set
            {
                _endpoint = value;
                if (value.State is Solid) StateSelect.SelectedIndex = 0;
                if (value.State is Gradient) StateSelect.SelectedIndex = 1;
                UpdateSelected();

            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void RefreshListItems()
        {
            DeviceEntryPanel.Items.Clear();
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
                RefreshListItems();
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
            UpdateSelected();
        }
        private void UpdateSelected() { 
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

            if (((ListBoxItem)StateSelect.SelectedItem).Content.ToString() == "Solid")
            {
                if (CurrentEndpoint.State is Solid)
                {
                    EditPanelCanvas.Children.Add(new SolidEditPanel((Solid) CurrentEndpoint.State));
                    MainPanelTab.SelectedIndex = 0;
                } else if (CurrentEndpoint.State is Gradient)
                {
                    CurrentEndpoint.State = new Solid(255,255,255,CurrentEndpoint);
                    EditPanelCanvas.Children.Add(new SolidEditPanel((Solid)CurrentEndpoint.State, false));
                    MainPanelTab.SelectedIndex = 0;
                }
            } else if (((ListBoxItem)StateSelect.SelectedItem).Content.ToString() == "Gradient")
            {
                 if (CurrentEndpoint.State is Solid)
                {
                    CurrentEndpoint.State = new Gradient(0, 255, 255, 255, 10, CurrentEndpoint);
                    EditPanelCanvas.Children.Add(new GradientEditPanel((Gradient)CurrentEndpoint.State, false));
                    MainPanelTab.SelectedIndex = 0;
                }
                else if (CurrentEndpoint.State is Gradient)
                {
                    EditPanelCanvas.Children.Add(new GradientEditPanel((Gradient)CurrentEndpoint.State));
                    MainPanelTab.SelectedIndex = 0;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            manageWindow.Show();
        }
    }
}
