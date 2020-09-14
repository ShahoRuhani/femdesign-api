// https://strusoft.com/
using System.Collections.Generic;
using System.Xml.Serialization;

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign
{
    /// <summary>
    /// Connections and virtual objects
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class AdvancedFem
    {        
        // connected_points

        // connected_lines

        // surface_connection

        // virtual_bar
        [XmlElement("virtual_bar")]
        public List<VirtualBar> VirtualBar = new List<VirtualBar>();

        // virtual_shell

        // diaphragm

        // steel_joint

        /// <summary>
        /// List of Cover (cover_type)
        /// </summary>
        [XmlElement("cover")]
        public List<Cover> Cover = new List<Cover>();
    }
}