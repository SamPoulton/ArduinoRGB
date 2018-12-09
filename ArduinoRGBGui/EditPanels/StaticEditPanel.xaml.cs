using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LuminanceLib;
using LuminanceLib.States;

namespace LuminanceGui
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StaticEditPanel : UserControl
    {
        private LuminanceLib.States.Solid _state;
        public LuminanceLib.States.Solid State
        {
            get { return _state; }
            set
            {
                _state = value;
                UpdateState();
            }
        }

        public StaticEditPanel()
        {
            InitializeComponent();
        }

        public SolidColorBrush CurrentColour =>
            new SolidColorBrush(Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value));

        public StaticEditPanel(Solid state)
        {
            InitializeComponent();
            _state = state;
            RefreshState();
        }
        private void UpdateState()
        {
            State.Red = (byte)RedSlider.Value;
            State.Green = (byte)GreenSlider.Value;
            State.Blue = (byte)BlueSlider.Value;
        }

        private void RefreshState()
        {
            RedSlider.Value = State.Red;
            GreenSlider.Value = State.Green;
            BlueSlider.Value = State.Blue;
        }
        private void UpdateState(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((Slider)e.Source == RedSlider) State.Red = (byte)e.NewValue;
            else if ((Slider)e.Source == GreenSlider) State.Green = (byte)e.NewValue;
            else if ((Slider)e.Source == BlueSlider) State.Blue = (byte)e.NewValue;
            ColourPreview.Fill = new SolidColorBrush(Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value));
        }
    }
}
