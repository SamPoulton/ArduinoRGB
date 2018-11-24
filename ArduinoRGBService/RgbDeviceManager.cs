using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ArduinoRGBLib
{
    public static class RgbDeviceManager
    {
        public const string XmlFilePath = "";
        public static readonly List<RgbDevice> Devices;
        static RgbDeviceManager()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFilePath + "ArduinoRGBConfig.xml");
            foreach (XmlNode node in doc.SelectNodes("/Config/Devices/Device"))
            {
                Devices.Add(new RgbDevice(node));
            }
        }
    }
}
