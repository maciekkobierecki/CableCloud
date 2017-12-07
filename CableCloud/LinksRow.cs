using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CableCloud
{
    class LinksRow
    {
        private String sourceNodeName;
        private int sourceNodePort;
        private String destinationNodeName;
        private int destinationNodePort;

        public LinksRow(String sourceNodeName, int sourceNodePort, String destinationNodeName, int destinationNodePort)
        {
            this.sourceNodeName = sourceNodeName;
            this.sourceNodePort = sourceNodePort;
            this.destinationNodeName = destinationNodeName;
            this.destinationNodePort = destinationNodePort;
        }
        public String getSourceNodeName()
        {
            return sourceNodeName;
        }
        public int getSourceNodePort()
        {
            return sourceNodePort;
        }
        public String getDestinationNodeName()
        {
            return destinationNodeName;
        }
        public int getDestinationNodePort()
        {
            return destinationNodePort;
        }
    }
}
