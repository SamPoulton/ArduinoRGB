using System.Runtime.Remoting.Messaging;

namespace ArduinoRGBLib
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
            return MessageType + "0" + Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2") + ";";
        }

        public UpdateLedStaticMessage(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }
}