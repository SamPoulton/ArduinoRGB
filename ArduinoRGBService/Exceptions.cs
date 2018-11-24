using System;

namespace ArduinoRGBLib
{
    namespace Exceptions
    {
        public class InvalidConfigException : Exception { }
        public class NotADeviceException : Exception { }
        public class DeviceDisconnectedException : Exception { };
    }
}