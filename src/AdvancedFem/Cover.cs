// https://strusoft.com/
using System.Globalization;
using System.Collections.Generic;
using System.Xml.Serialization;

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign
{
    /// <summary>
    /// cover_type
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class Cover: EntityBase
    {
        private static int coverInstance = 0; // used for numbering in name
        [XmlAttribute("name")]
        /// <summary>
        /// Name (identifier)
        /// </summary>
        public string name { get; set; }
        
        [XmlElement("load_bearing_direction", Order = 1)]
        /// <summary>
        /// Load bearing direction (point_type_3d)
        /// </summary>
        public Geometry.FdVector3d loadBearingDirection { get; set; }

        [XmlElement("region", Order = 2)]
        /// <summary>
        /// Region (region_type).
        /// </summary>
        public Geometry.Region region { get; set; }

        [XmlElement("supporting_structures", Order = 3)]
        /// <summary>
        /// Supporting structures (cover_referencelist_type)
        /// </summary>
        public CoverReferenceList supportingStructures { get; set; } 

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private Cover()
        {
            
        }

        private Cover(Geometry.Region region, CoverReferenceList supportingStructures, Geometry.FdVector3d loadBearingDirection)
        {
            coverInstance++;
            this.EntityCreated();
            string name = "C0." + coverInstance.ToString();
            this.loadBearingDirection = loadBearingDirection;
            this.region = region;
            this.supportingStructures = supportingStructures;
        }

        /// <summary>
        /// Create OneWayCover.
        /// </summary>
        internal static Cover OneWayCover(Geometry.Region region, List<object> supportingStructures, Geometry.FdVector3d loadBearingDirection)
        {
            // get supportingStructures.guid
            CoverReferenceList _supportingStructures = CoverReferenceList.FromObjectList(supportingStructures);

            // create cover
            Cover _cover = new Cover(region, _supportingStructures, loadBearingDirection);

            return _cover;
        }

        /// <summary>
        /// Create TwoWayCover.
        /// </summary>
        internal static Cover TwoWayCover(Geometry.Region region, List<object> supportingStructures)
        {
            // get supportingStructures.guid
            CoverReferenceList _supportingStructures = CoverReferenceList.FromObjectList(supportingStructures);

            // create cover
            Cover _cover = new Cover(region, _supportingStructures, null);

            return _cover;
        }

        #region dynamo
        /// <summary>
        /// Create a one way cover. Surfaces created by Surface.ByLoft method might cause errors, please use Surface.ByPatch or Surface.ByPerimeterPoints.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="surface">Surface. Surface must be flat.</param>
        /// <param name="supportingStructures">Single structure element och list of structure elements. List cannot be nested - use flatten.</param>
        /// <param name="loadBearingDirection">Vector of load bearing direction.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static Cover OneWayCover(Autodesk.DesignScript.Geometry.Surface surface, [DefaultArgument("[]")] List<object> supportingStructures, Autodesk.DesignScript.Geometry.Vector loadBearingDirection = null)
        {
            // create FlatSurface
            Geometry.Region region = Geometry.Region.FromDynamo(surface);

            // get loadBearingDirection
            Geometry.FdVector3d _loadBearingDirection = Geometry.FdVector3d.FromDynamo(loadBearingDirection).Normalize();

            // return
            return Cover.OneWayCover(region, supportingStructures, _loadBearingDirection);
        }

        /// <summary>
        /// Create a two way cover. 
        /// Surfaces created by Surface.ByLoft method might cause errors, please use Surface.ByPatch or Surface.ByPerimeterPoints.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="surface">Surface. Surface must be flat.</param>
        /// <param name="supportingStructures">Single structure element or list of structure elements. List cannot be nested - use flatten.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static Cover TwoWayCover(Autodesk.DesignScript.Geometry.Surface surface, [DefaultArgument("[]")] List<object> supportingStructures)
        {
            // create FlatSurface
            Geometry.Region region = Geometry.Region.FromDynamo(surface);

            // return
            return Cover.TwoWayCover(region, supportingStructures);
        }

        /// <summary>
        /// Create Dynamo surface from underlying Region of Cover.
        /// </summary>
        internal Autodesk.DesignScript.Geometry.Surface GetDynamoSurface()
        {
            return this.region.ToDynamoSurface();
        }

        /// <summary>
        /// Create Dynamo curves from underlying Edges in Region of Cover.
        /// </summary>
        internal  List<List<Autodesk.DesignScript.Geometry.Curve>> GetDynamoCurves()
        {
            return this.region.ToDynamoCurves();
        }

        #endregion

    }
}      