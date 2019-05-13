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
        public List<RgbEndpoint> Endpoints = new List<RgbEndpoint>();
        public string DeviceName;
        public abstract void SendMessage(string msg);
    }

    public class SerialRgbDevice : RgbDevice
    {
        private readonly SerialPort Port = new SerialPort();
        private Queue<string> CommandQueue = new Queue<string>(); 
        private Thread CommunicationThread;

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
                Port.WriteTimeout = 500;
                Port.Open();
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is FormatException || ex is IOException || ex is UnauthorizedAccessException)
            {
                throw new Exceptions.DeviceDisconnectedException();
                Port.Close();
            }
            // actually communicate with the device

            try
            {
                SendMessage(new InitialiseCommMessage());
            }
            catch (TimeoutException)
            {
                throw new Exceptions.NotADeviceException();
            }

            string response = ReceiveMessage();

            if (response != "OK") throw new NotADeviceException();
            
            SendMessage(new GetNameMessage());
            DeviceName = ReceiveMessage();

            // get the endpoints from the device
            SendMessage(new GetEndpointsMessage());
            string message = ReceiveMessage();

            foreach (string endpoint in message.Split(','))
            {
                Endpoints.Add(new RgbEndpoint(endpoint.Replace(";",""), this));
            }
        }

        public override void SendMessage(string message)
        {
            try
            {
                if (message[0] == '2')
                {
                    Console.WriteLine("Sending message " + message + "to port " + Port.PortName);
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
                    try
                    {
                        ReceiveMessage();
                    }
                    catch (Exception)
                    {
                        ;
                    }
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
        public readonly bool IsAddressable;
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
            IsAddressable = "1" == separated[2];
            switch (separated[3])
            {
                case "0":
                    _state = new States.Solid(byte.Parse(separated[4], NumberStyles.HexNumber), byte.Parse(separated[5], NumberStyles.HexNumber), byte.Parse(separated[6], NumberStyles.HexNumber), this);
                    break;
                case "1":
                    _state = new States.Gradient(byte.Parse(separated[3], NumberStyles.HexNumber), 
                        byte.Parse(separated[5], NumberStyles.HexNumber), 
                        byte.Parse(separated[6], NumberStyles.HexNumber),
                        byte.Parse(separated[7], NumberStyles.HexNumber),
                        byte.Parse(separated[8], NumberStyles.HexNumber), 
                        this);
                    break;
            }
        }

        public void SetLedState(MessageOut msg)
        {
            Parent.SendMessage(msg.ToString().Replace("%", Index.ToString()));
        }
    }
}