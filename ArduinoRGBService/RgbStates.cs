using System.Security.Cryptography.X509Certificates;

namespace ArduinoRGBLib
{
    namespace States
    {
        public abstract class RgbState
        {
            public abstract MessageOut GetMessageOut();
            public abstract RgbEndpoint Parent { get; }
        }

        public class Solid : RgbState
        {
            public override RgbEndpoint Parent { get; }
            private byte _red;
            private byte _green;
            private byte _blue;

            public byte Red
            {
                get => _red;
                set {
                    _red = value;
                    Parent.SetLedState(GetMessageOut());
                }
            }
            public byte Green
            {
                get => _green;
                set {
                    _green = value;
                    Parent.SetLedState(GetMessageOut());
                }
            }
            public byte Blue
            {
                get => _blue;
                set {
                    _blue = value;
                    Parent.SetLedState(GetMessageOut());
                }
            }

            public Solid(byte red, byte green, byte blue, RgbEndpoint parent)
            {
                _red = red;
                _green = green;
                _blue = blue;
                Parent = parent;
            }

            public override MessageOut GetMessageOut()
            {
                return new UpdateLedStaticMessage(Red, Green, Blue);
            }
        }
    }
}