using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LuminanceLib.Exceptions;

namespace LuminanceLib
{
    public static class RgbDeviceManager
    {
        public const string XmlFilePath = "";
        public static readonly List<RgbDevice> Devices = new List<RgbDevice>();

        static RgbDeviceManager()
        {

            foreach (string port in SerialPort.GetPortNames())
            {
                try
                {
                    Devices.Add(new SerialRgbDevice(port));
                }
                catch (Exception e) when (e is Exceptions.DeviceDisconnectedException ||
                                          e is Exceptions.NotADeviceException)
                {
                    ;
                }
            }
        }
    }
}
