using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Bashmohandes.DeviceDetector.Library.Parser;

namespace Bashmohandes.DeviceDetector.Library
{
    public class DeviceDetector
    {
        Dictionary<string, browsers> browserFiles;
        BrowserTree browsersTree;

        bool initialized;

        public DeviceDetector()
        {
            this.browserFiles = new Dictionary<string, browsers>();
            this.initialized = false;
        }

        public void AddFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new DeviceDetectorException("Specified browser file doesn't exist", new FileNotFoundException(string.Empty, fileName));
            }

            if (String.Compare(Path.GetExtension(fileName), ".browser", true) != 0)
            {
                throw new DeviceDetectorException("Specified file has invalid extension");
            }

            using (XmlReader xmlReader = XmlReader.Create(fileName))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(browsers));
                if (!xmlSerializer.CanDeserialize(xmlReader))
                {
                    throw new DeviceDetectorException("Invalid file format");
                }

                try
                {
                    browsers browsersFile = xmlSerializer.Deserialize(xmlReader) as browsers;
                    string keyName = Path.GetFileNameWithoutExtension(fileName);
                    if (!this.browserFiles.ContainsKey(keyName))
                    {
                        this.browserFiles.Add(keyName, browsersFile);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new DeviceDetectorException("Cannot parse specified file \"" + fileName + "\"", ex);
                }
            }
        }


        public void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            browsers[] parsedBrowsersFiles = this.browserFiles.Values.ToArray();
            this.browsersTree = new BrowserTree(parsedBrowsersFiles);

            this.initialized = true;
        }

        public Dictionary<string, string> DetectCaps(Dictionary<string, string> headers)
        {
            var result = this.browsersTree.DetectBrowserCaps(headers);


            return result;
        }
    }
}
