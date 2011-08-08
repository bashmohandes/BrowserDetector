using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Bashmohandes.DeviceDetector.Library.Parser
{
    [DebuggerDisplay("[{NodeType}] ID={ID}, PID={ParentID}, RefID={RefID}")]
    class BrowserTreeNode<T> where T : IBrowserFileElement
    {
        private T item;
        private BrowserTreeNode<T> parent = null;
        private List<BrowserTreeNode<T>> children;
        private List<BrowserTreeNode<T>> refNodes;
        private IDictionary<string, capability> capabilities;

        public BrowserTreeNode(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException("item");
            }

            this.item = item;
            this.children = new List<BrowserTreeNode<T>>();
            this.refNodes = new List<BrowserTreeNode<T>>();
            this.capabilities = new Dictionary<string, capability>();
            InitializeCaps();
        }

        private void InitializeCaps()
        {
            if (this.item.capabilities == null || this.item.capabilities.capability == null)
                return;

            foreach (var cap in this.item.capabilities.capability)
            {
                this.capabilities[cap.name] = cap;
            }
        }

        public string ID
        {
            get { return this.item.id; }
        }

        public string ParentID
        {
            get { return this.item.parentID; }
        }

        public BrowserTreeNode<T> Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        public string RefID
        {
            get { return this.item.refID; }
        }

        public IList<BrowserTreeNode<T>> RefNodes
        {
            get { return this.refNodes; }
        }

        public IList<BrowserTreeNode<T>> Children
        {
            get
            {
                return this.children;
            }
        }

        public BrowserTreeNodeType NodeType
        {
            get { return this.item.NodeType; }
        }

        public IDictionary<string, capability> Capabilities
        {
            get { return this.capabilities; }
        }

        public bool IsLeaf
        {
            get { return this.children.Count == 0; }
        }

        internal void UpdateCapabilities()
        {
            //Step 1: Copy capabilities down from parent to children recursively
            this.CascadeCapabilitiesRec(this);

            //Step 2: Apply ref nodes capabilities
            this.ApplyRefCapabilities(this);
        }

        private void ApplyRefCapabilities(BrowserTreeNode<T> browserTreeNode)
        {
            foreach (var refNode in this.refNodes)
            {
                foreach (var cap in refNode.capabilities)
                {
                    this.capabilities[cap.Key] = cap.Value;
                }
            }
        }

        private void CascadeCapabilitiesRec(BrowserTreeNode<T> currentNode)
        {
            if (null == currentNode)
                return;

            foreach (var node in currentNode.children)
            {
                foreach (var cap in currentNode.capabilities)
                {
                    if (!node.capabilities.ContainsKey(cap.Key))
                        node.capabilities.Add(cap);
                }

                if (!node.IsLeaf)
                {
                    CascadeCapabilitiesRec(node);
                }
            }
        }

        internal void DetectBrowserCaps(Dictionary<string, string> headers, ref Dictionary<string, string> capabilities)
        {
            if (ApplyIdentification(headers, capabilities))
            {
                foreach (var cap in this.capabilities)
                {
                    capabilities[cap.Key] = cap.Value.value;
                }

                ApplyCapture(headers, ref capabilities);

                foreach (var child in this.children)
                {
                    child.DetectBrowserCaps(headers, ref capabilities);
                }
            }
        }

        private void ApplyCapture(Dictionary<string, string> headers, ref Dictionary<string, string> capabilities)
        {
            if (this.item.capture != null)
            {
                if (this.item.capture.userAgent != null)
                {
                    string userAgent = headers["User-Agent"];
                    if (!String.IsNullOrEmpty(userAgent))
                    {
                        foreach (IMatchable uaRule in this.item.capture.userAgent)
                        {
                            if (uaRule.MatchExpression != null)
                            {
                                Match match = uaRule.MatchExpression.Match(userAgent);
                                while(match.Success)
                                {
                                    foreach (Group group in match.Groups)
                                    {
                                        Console.WriteLine(group.Value);
                                    }
                                    match = match.NextMatch();
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool ApplyIdentification(Dictionary<string, string> headers, Dictionary<string, string> capabilities)
        {
            bool match = true;

            if (item.identification == null)
            {
                return item.NodeType == BrowserTreeNodeType.Gateway;
            }

            string userAgent = headers["User-Agent"];
            if (!String.IsNullOrEmpty(userAgent))
            {
                if (item.identification.userAgent != null)
                {
                    foreach (IMatchable uaRule in item.identification.userAgent)
                    {
                        if (uaRule.MatchExpression != null)
                        {
                            match &= uaRule.MatchExpression.IsMatch(userAgent);
                        }
                        else if (uaRule.NonMatchExpression != null)
                        {
                            match &= !uaRule.NonMatchExpression.IsMatch(userAgent);
                        }
                    }
                }
            }

            if (item.identification.header != null)
            {
                foreach (var header in item.identification.header)
                {
                    string headerValue;
                    if (!headers.TryGetValue(header.name, out headerValue))
                    {
                        return false;
                    }

                    if (header.MatchExpression != null)
                    {
                        match &= header.MatchExpression.IsMatch(headerValue);
                    }
                    else if (header.NonMatchExpression != null)
                    {
                        match &= !header.NonMatchExpression.IsMatch(headerValue);
                    }
                }
            }

            if (this.item.identification.capability != null)
            {
                if (capabilities == null)
                {
                    return false;
                }

                foreach (var cap in this.item.identification.capability)
                {
                    string capValue;
                    if (!capabilities.TryGetValue(cap.name, out capValue))
                    {
                        return false;
                    }

                    if (cap.MatchExpression != null)
                    {
                        match &= cap.MatchExpression.IsMatch(capValue);
                    }
                    else if (cap.NonMatchExpression != null)
                    {
                        match &= cap.NonMatchExpression.IsMatch(capValue);
                    }
                }
            }
            return match;
        }
    }
}
