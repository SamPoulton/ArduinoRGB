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
    public static class HsvConverter
    {
        public static Color HsvToColour(double h, double S, double V)
        {
            int r, g, b;
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
            return Color.FromRgb((byte) r, (byte) g, (byte) b);
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GradientEditPanel : UserControl
    {
        private bool ignoreUpdate = false;
        private LuminanceLib.States.Gradient _state;
        public LuminanceLib.States.Gradient State
        {
            get { return _state; }
            set
            {
                _state = value;
                UpdateState();
            }
        }

        public GradientEditPanel()
        {
            InitializeComponent();
        }

        public GradientEditPanel(Gradient state)
        {
            InitializeComponent();
            _state = state;
            ignoreUpdate = true;
            RefreshState();
            ignoreUpdate = false;
        }

        public GradientEditPanel(Gradient state, bool isNewState)
        {
            InitializeComponent();
            _state = state;
            ignoreUpdate = isNewState;
            RefreshState();
            UpdateState();
            ignoreUpdate = false;
        }

        private void UpdateState()
        {
            State.Hue1 = (byte)Hue1Slider.Value;
            State.Hue2 = (byte)Hue2Slider.Value;
            State.Saturation = (byte)SaturationSlider.Value;
            State.Value = (byte)ValueSlider.Value;
            State.Speed = (byte)SpeedSlider.Value;
        }

        private void RefreshState()
        { 
            Hue1Slider.Value = State.Hue1;
            Hue2Slider.Value = State.Hue2;
            SaturationSlider.Value = State.Saturation;
            ValueSlider.Value = State.Value;
            SpeedSlider.Value = State.Speed;
        }
        private void UpdateState(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!ignoreUpdate)
            {
                if ((Slider) e.Source == Hue1Slider) State.Hue1 = (byte) e.NewValue;
                else if ((Slider) e.Source == Hue2Slider) State.Hue2 = (byte) e.NewValue;
                else if ((Slider) e.Source == SaturationSlider) State.Saturation = (byte) e.NewValue;
                else if ((Slider) e.Source == ValueSlider) State.Value = (byte) e.NewValue;
                else if ((Slider) e.Source == SpeedSlider) State.Speed = (byte) e.NewValue;
            }

            ColourPreview.Fill = new LinearGradientBrush(HsvConverter.HsvToColour(State.Hue1, State.Saturation, State.Value),
                HsvConverter.HsvToColour(State.Hue2, State.Saturation, State.Value), 90.0);
        }
    }
}
