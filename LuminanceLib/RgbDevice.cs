using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using LuminanceLib.Exceptions;

namespace LuminanceLib
{
    public abstract class RgbDevice
    {
        public List<RgbEndpoint> Endpoints;
        public readonly string DeviceName;
        public abstract void SendMessage(MessageOut msg);
    }

    public class SerialRgbDevice : RgbDevice
    {
        private readonly SerialPort Port = new SerialPort();
        public List<RgbEndpoint> Endpoints = new List<RgbEndpoint>();
        public readonly string DeviceName;
        private Queue<string> CommandQueue = new Queue<string>(); 
        private Thread CommunicationThread;

        public string PortName
        {
            get => Port.PortName;
        }

        public SerialRgbDevice(string port)
        {
            CommunicationThread = new Thread(UpdateMessageThread);
            CommunicationThread.IsBackground = true;
            CommunicationThread.Start();
            // interpret xml data
            try
            {
                Port.PortName = port;
                Port.BaudRate = 9600;
                Port.Handshake = Handshake.None;
                Port.ReadTimeout = 500;
                Port.Open();
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is FormatException || ex is IOException)
            {
                throw new Exceptions.DeviceDisconnectedException();
            }
            // actually communicate with the device

            SendMessage(new InitialiseCommMessage());
            string response = ReceiveMessage();

            if (response != "OK") throw new NotADeviceException();
            
            SendMessage(new GetNameMessage());
            DeviceName = ReceiveMessage();

            // get the endpoints from the device
            SendMessage(new GetEndpointsMessage());
            string message = ReceiveMessage();

            foreach (string device in message.Split(','))
            {
                Endpoints.Add(new RgbEndpoint(device.Replace(";",""), this));
            }
        }

        public override void SendMessage(MessageOut _message)
        {
            string message = _message.ToString();
            try
            {
                if (message[0] == '2')
                {
                    CommandQueue.Enqueue(message);
                }
                else
                {
                    Port.Write(message + ";");
                    Console.WriteLine("Sending message " + message + "to port " + Port.PortName);
                }
            }
            catch (InvalidOperationException)
            {
                throw new Exceptions.DeviceDisconnectedException();
            }
        }

        public string ReceiveMessage()
        {
           try
            {
                return Port.ReadTo(";");
            }
            catch (TimeoutException)
            {
                throw new Exceptions.DeviceDisconnectedException();
            }
        }

        // this runs on a separate thread
        public void UpdateMessageThread()
        {
            while (true)
            {
                if (CommandQueue.Count > 0)
                {
                    while (CommandQueue.Count > 1) CommandQueue.Dequeue();
                    Port.Write(CommandQueue.Dequeue() + ";");
                    ReceiveMessage();
                }
                Thread.Sleep(10);
            }
        }
    }

    public class RgbEndpoint
    {
        public readonly string Name;
        public int Index;
        private States.RgbState _state;
        public readonly RgbDevice Parent;
        public States.RgbState State
        {
            get => _state;
            set
            {
                _state = value;
                SetLedState(value.GetMessageOut());
            }
        }

        public RgbEndpoint(string name_index, RgbDevice parent)
        {
            string[] separated = name_index.Split("&".ToCharArray());

            Index = int.Parse(separated[1]);
            Name = separated[0];
            Parent = parent;
            switch (separated[2])
            {
                case "0":
                    _state = new States.Solid(byte.Parse(separated[3], NumberStyles.HexNumber), byte.Parse(separated[4], NumberStyles.HexNumber), byte.Parse(separated[5], NumberStyles.HexNumber), this);
                    break;
                case "1":
                    _state = new States.Gradient(byte.Parse(separated[3], NumberStyles.HexNumber), 
                        byte.Parse(separated[4], NumberStyles.HexNumber), 
                        byte.Parse(separated[5], NumberStyles.HexNumber),
                        byte.Parse(separated[6], NumberStyles.HexNumber),
                        byte.Parse(separated[7], NumberStyles.HexNumber), 
                        this);
                    break;
            }
        }

        public void SetLedState(MessageOut msg)
        {
            Parent.SendMessage(msg);
        }
    }
}