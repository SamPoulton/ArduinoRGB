using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using LuminanceLib;

// MESSAGE TYPES
// Initialise: 0
// Get Endpoints: 1
// Update State: 2
//      Solid: 20
//      Gradient: 21


namespace LuminanceLib
{
    public abstract class MessageOut
    {
        public abstract int MessageType { get; }
        public override string ToString() => MessageType.ToString();
        public abstract int EndpointIndex { get; set; }
        public static implicit operator string(MessageOut d)
        {
            return d.ToString();
        }
    }

    public class InitialiseCommMessage : MessageOut
    {
        public override int EndpointIndex { get => 0; set { }}

        public override int MessageType
        {
            get => 0;
        }

        public override string ToString() => "00";
    }

    public class GetNameMessage : MessageOut
    {
        public override int EndpointIndex { get => 0; set { } }
        public override int MessageType
        {
            get => 0;
        }

        public override string ToString() => "01";
    }

    public class GetEndpointsMessage : MessageOut
    {
        public override int EndpointIndex { get => 0; set { } }
        public override int MessageType
        {
            get => 1;
        }
    }

    public class UpdateLedSolidMessage : MessageOut
    {
        public override int EndpointIndex { get; set; }
        public override int MessageType
        {
            get => 2;
        }

        public readonly byte Red, Green, Blue;

        public override string ToString()
        {
            return MessageType + "0%" + Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
        }

        public UpdateLedSolidMessage(byte red, byte green, byte blue, int endpoint)
        {
            Red = red;
            Green = green;
            Blue = blue;
            EndpointIndex = endpoint;
        }
    }

    public class UpdateLedGradientMessage : MessageOut
    {
        public override int EndpointIndex { get; set; }
        public override int MessageType
        {
            get => 2;
        }

        public readonly byte Hue1, Hue2, Saturation, Luminance, Speed;

        public override string ToString()
        {
            return MessageType + "1%" + Hue1.ToString("X2") + Hue2.ToString("X2") + Saturation.ToString("X2") +
                   Luminance.ToString("X2") + Speed.ToString("X2");
        }

        public UpdateLedGradientMessage(byte hue1, byte hue2, byte saturation, byte luminance, byte speed, int endpoint)
        {
            Hue1 = hue1;
            Hue2 = hue2;
            Saturation = saturation;
            Luminance = luminance;
            Speed = speed;
            EndpointIndex = endpoint;
        }
    }
}