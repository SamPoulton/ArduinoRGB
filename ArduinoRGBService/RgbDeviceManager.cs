using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ArduinoRGBLib.Exceptions;

namespace ArduinoRGBLib
{
    public static class RgbDeviceManager
    {
        public const string XmlFilePath = "";
        public static readonly List<RgbDevice> Devices = new List<RgbDevice>();
        static RgbDeviceManager()
        {

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlFilePath + "ArduinoRGBConfig.xml");
                foreach (XmlNode node in doc.SelectNodes("/Config/Devices/Device"))
                {
                    try
                    {
                        Devices.Add(new RgbDevice(node));
                    }
                    catch (Exceptions.DeviceDisconnectedException)
                    {
                        ;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new NoConfigException();
            }
        }
    }
}
