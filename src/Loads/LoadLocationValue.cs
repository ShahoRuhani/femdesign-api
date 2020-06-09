// https://strusoft.com/
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Loads
{
    /// <summary>
    /// location_value_type
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class LoadLocationValue: LocationValue
    {
        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private LoadLocationValue()
        {

        }

        /// <summary>
        /// Internal constructor accessed by GH components and Dynamo nodes.
        /// </summary>
        internal LoadLocationValue(Geometry.FdPoint3d loadPosition, double val)
        {
            this.x = loadPosition.x;
            this.y = loadPosition.y;
            this.z = loadPosition.z;
            this.val = val;
        }

        #region dynamo
        /// <summary>
        /// Create a LoadLocationValue object.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="point">Position of Load.</param>
        /// <param name="intensity">Intensity of load.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static LoadLocationValue Define(Autodesk.DesignScript.Geometry.Point point, double intensity)
        {
            return new LoadLocationValue(Geometry.FdPoint3d.FromDynamo(point), intensity);
        }

        #endregion
    }
}