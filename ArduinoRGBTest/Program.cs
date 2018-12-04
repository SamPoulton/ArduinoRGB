using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuminanceLib;

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
                dev.Endpoints[0].SetLedState(new UpdateLedStaticMessage(255, 255, 255));
            }
        }
    }
}
