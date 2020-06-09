// https://strusoft.com/
using System.Xml.Serialization;
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion


namespace FemDesign.Calculate
{
    [IsVisibleInDynamoLibrary(false)]
    public class CmdUser
    {
        [XmlAttribute("command")]
        public string _command; // token
        [XmlIgnore]
        public string command
        {
            get {return this._command;}
            set {this._command = "; CXL $MODULE " + RestrictedString.CmdUserModule(value);}
        }

        
        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>                          
        private CmdUser()
        {
            
        }
        public CmdUser(string module)
        {
            this.command = module;
        }
    }
}