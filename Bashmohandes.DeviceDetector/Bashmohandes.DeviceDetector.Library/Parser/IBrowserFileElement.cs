using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bashmohandes.DeviceDetector.Library.Parser
{
    public enum BrowserTreeNodeType
    {
        Browser,
        Gateway,
        DefaultBrowser
    }

    public interface IBrowserFileElement
    {
        identification identification { get; }

        capture capture { get; }

        capabilities capabilities { get; }

        sampleHeaders sampleHeaders { get; }

        string id { get; }

        string parentID { get; }

        string refID { get; }

        BrowserTreeNodeType NodeType { get; }
    }
}
