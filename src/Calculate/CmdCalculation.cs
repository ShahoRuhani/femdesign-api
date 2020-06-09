// https://strusoft.com/
using System.Xml.Serialization;
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion


namespace FemDesign.Calculate
{
    /// <summary>
    /// fdscript.xsd
    /// CMDCALCULATION
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class CmdCalculation
    {
        [XmlElement("analysis")]
        public Analysis analysis { get; set; } // ANALYSIS
        [XmlElement("design")]
        public Design design { get; set; } // DESIGNCALC
        [XmlAttribute("command")]
        public string command = "; CXL $MODULE CALC"; // token
        
        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private CmdCalculation()
        {
            
        }
        public CmdCalculation(Analysis analysis)
        {
            this.analysis = analysis;
        }
        public CmdCalculation(Analysis analysis, Design design)
        {
            this.analysis = analysis;
            this.design = design;
        }
    }
}