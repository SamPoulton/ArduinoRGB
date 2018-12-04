using System;

namespace LuminanceLib
{
    namespace Exceptions
    {
        public class InvalidConfigException : Exception { }
        public class NotADeviceException : Exception { }
        public class DeviceDisconnectedException : Exception { };
        public class NoConfigException : Exception { };
    }
}