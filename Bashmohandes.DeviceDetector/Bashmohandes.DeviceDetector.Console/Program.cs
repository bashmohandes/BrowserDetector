using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Bashmohandes.DeviceDetector.Library.Parser;
using System.Xml;

namespace Bashmohandes.DeviceDetector.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "mobile.browser";

            DeviceDetector.Library.DeviceDetector detector = new Library.DeviceDetector();
            detector.AddFile(fileName);
            detector.Initialize();

            var capResults = detector.DetectCaps(new Dictionary<string, string>() {
                {"User-Agent", "Nokia3650/1.0 SymbianOS/6.1 Series60/1.2 Profile/MIDP-1.0 Configuration/CLDC-1.0"}
            });
        }
    }
}
