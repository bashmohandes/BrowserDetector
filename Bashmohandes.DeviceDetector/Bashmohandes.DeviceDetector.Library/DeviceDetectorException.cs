using System;
using System.Runtime.Serialization;
namespace Bashmohandes.DeviceDetector
{
    [Serializable]
    public class DeviceDetectorException : Exception
    {
        public DeviceDetectorException()
        {
        }

        public DeviceDetectorException(string message)
            : base(message)
        {
        }

        public DeviceDetectorException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected DeviceDetectorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}