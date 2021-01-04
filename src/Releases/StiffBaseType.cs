// https://strusoft.com/

using System.Xml.Serialization;
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Releases
{
    /// <summary>
    /// stiff_base_type
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class StiffBaseType
    {
        [XmlAttribute("neg")]
        public double _neg;

        [XmlIgnore]
        public double Neg
        {
            get
            {
                return this._neg;
            }
            set
            {
                this._neg = RestrictedDouble.NonNegMax_1e15(value);
            }
        }

        [XmlAttribute("pos")]
        public double _pos;

        [XmlIgnore]
        public double Pos
        {
            get
            {
                return this._pos;
            }
            set
            {
                this._pos = RestrictedDouble.NonNegMax_1e15(value);
            }
        }
    }
}
