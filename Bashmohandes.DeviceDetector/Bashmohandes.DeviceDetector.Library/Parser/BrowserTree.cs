using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bashmohandes.DeviceDetector.Library.Parser
{
    class BrowserTree
    {
        Dictionary<string, BrowserTreeNode<IBrowserFileElement>> internalMap;
        BrowserTreeNode<IBrowserFileElement> root;
        List<BrowserTreeNode<IBrowserFileElement>> refNodes;

        private BrowserTree()
        {
            this.internalMap = new Dictionary<string, BrowserTreeNode<IBrowserFileElement>>();
            refNodes = new List<BrowserTreeNode<IBrowserFileElement>>();
        }

        public BrowserTree(browsers[] parsedBrowserFiles)
            : this()
        {
            if (null == parsedBrowserFiles)
            {
                throw new ArgumentNullException("parsedBrowserFiles");
            }

            // Step 1, load all nodes into the internal map & figure out the root node.
            foreach (browsers bFile in parsedBrowserFiles)
            {
                foreach (object item in bFile.Items)
                {
                    IBrowserFileElement browserItem = item as IBrowserFileElement;
                    if (item == null)
                    {
                        throw new ArgumentNullException("Item type {0} is not supported", item.ToString());
                    }

                    BrowserTreeNode<IBrowserFileElement> node = new BrowserTreeNode<IBrowserFileElement>(browserItem);
                    if (String.IsNullOrEmpty(node.ID))
                    {
                        if (String.IsNullOrEmpty(node.RefID))
                        {
                            throw new DeviceDetectorException(String.Format("Found node with neither ID nor RefID"));
                        }
                        refNodes.Add(node);
                        continue;
                    }
                    if (!this.internalMap.ContainsKey(node.ID))
                    {
                        this.internalMap.Add(node.ID, node);
                    }
                    else
                    {
                        throw new DeviceDetectorException("Node with ID \"" + node.ID + "\" is duplicated");
                    }

                    if (String.IsNullOrEmpty(node.ParentID))
                    {
                        if (this.root == null)
                        {
                            this.root = node;
                        }
                        else
                        {
                            throw new DeviceDetectorException(String.Format("Multiple root nodes detected, current root \"{0}\", duplicate root \"{1}\"", this.root.ID, node.ID));
                        }
                    }
                }
            }

            // Step 2, update the parent-child relationships between nodes.
            foreach (var mapPair in this.internalMap)
            {
                if (String.IsNullOrEmpty(mapPair.Value.ParentID))
                {
                    continue;
                }
                mapPair.Value.Parent = this.internalMap[mapPair.Value.ParentID];
                mapPair.Value.Parent.Children.Add(mapPair.Value);
            }

            // Step 3, update the ref relationship
            foreach (var node in refNodes)
            {
                if (!this.internalMap.ContainsKey(node.RefID))
                {
                    throw new DeviceDetectorException("A node that reference a non existing node detected");
                }

                this.internalMap[node.RefID].RefNodes.Add(node);
            }

            
            // Step 4, compile capabilities
            this.root.UpdateCapabilities();
            
        }

        public Dictionary<string, string> DetectBrowserCaps(Dictionary<string, string> headers)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (this.root != null)
            {
                this.root.DetectBrowserCaps(headers, ref result);
            }
            return result;
        }
    }
}
