using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Xml;

namespace ArduinoRGBLib
{
    public class RgbDevice
    {
        private readonly SerialPort Port = new SerialPort();
        public List<RgbEndpoint> Endpoints = new List<RgbEndpoint>();
        public readonly string DeviceName;


        public RgbDevice(XmlNode root)
        {
            // interpret xml data
            try
            { 
                Port.PortName = root.SelectSingleNode("./Port").InnerText;
                DeviceName = root.SelectSingleNode("./Name").InnerText;
                Port.BaudRate = int.Parse(root.SelectSingleNode("./BaudRate").InnerText);
                Port.ReadTimeout = 500;
                Port.Open();
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is FormatException) {
                throw new Exceptions.InvalidConfigException();
            }
            // actually communicate with the device

            SendMessage(new InitialiseCommMessage().ToString());
            string response = ReceiveMessage();
            
            if (response != "OK") throw new Exceptions.NotADeviceException(); 
            
            // get the endpoints from the device
            SendMessage(new GetEndpointsMessage());
            string message = ReceiveMessage();

            foreach (string device in message.Split(','))
            {
                Endpoints.Add(new RgbEndpoint(device.Replace(";",""), this));
            }

            foreach (RgbEndpoint endpoint in Endpoints)
            {
                endpoint.State = new States.Solid(255, 127, 0, endpoint);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                Port.Write(message + ";");
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
                Parent.SendMessage(Index + value.GetMessageOut());
            }
        }

        public RgbEndpoint(string name, RgbDevice parent)
        {
            Name = name;
            Parent = parent;
        }

        public void SetLedState(MessageOut msg)
        {
            Parent.SendMessage(Index + msg);
        }
    }
}