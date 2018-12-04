using System.Runtime.Remoting.Messaging;

// MESSAGE TYPES
// Initialise: 0
// Get Endpoints: 1
// Update State: 2
//      Static: 20
//      Gradient: 21


namespace LuminanceLib
{
    public abstract class MessageOut
    {
        public abstract int MessageType { get;}
        public override string ToString() => MessageType.ToString();
        public static implicit operator string(MessageOut d)
        {
            return d.ToString();
        }
    }

    public class InitialiseCommMessage : MessageOut
    {
        public override int MessageType
        {
            get => 0;
        }
    }

    public class GetEndpointsMessage : MessageOut
    {
        public override int MessageType
        {
            get => 1;
        }
    }

    public class UpdateLedStaticMessage : MessageOut
    {
        public override int MessageType
        {
            get => 2;
        }

        public readonly byte Red, Green, Blue;
        public override string ToString()
        {
            return MessageType + "0%" + Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
        }

        public UpdateLedStaticMessage(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }

    public class UpdateLedGradientMessage : MessageOut
    {
        public override int MessageType
        {
            get => 2;
        }

        public readonly byte Hue1, Hue2, Saturation, Luminance;
        public override string ToString()
        {
            return MessageType + "1" + Hue1.ToString("X2") + Hue2.ToString("X2") + Saturation.ToString("X2") + Luminance.ToString("X2");
        }

        public UpdateLedGradientMessage(byte hue1, byte hue2, byte saturation, byte luminance)
        {
            Hue1 = hue1;
            Hue2 = hue2;
            Saturation = saturation;
            Luminance = luminance;
        }
    }
}