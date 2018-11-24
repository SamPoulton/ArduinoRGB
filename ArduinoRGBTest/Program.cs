using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArduinoRGBLib;

namespace ArduinoRGBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connected Devices:");
            foreach (RgbDevice dev in RgbDeviceManager.Devices)
            {
                Console.WriteLine(dev.DeviceName);
            }
        }
    }
}
