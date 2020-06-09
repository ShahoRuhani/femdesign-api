// https://strusoft.com/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Shells
{
    /// <summary>
    /// slab_type
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class Slab: EntityBase
    {
        private static int plateInstance = 0;
        private static int wallInstance = 0;
        [XmlIgnore]
        public Materials.Material material { get; set; }
        [XmlIgnore]
        public Reinforcement.SurfaceReinforcementParameters surfaceReinforcementParameters { get; set; }
        [XmlIgnore]
        public List<Reinforcement.SurfaceReinforcement> surfaceReinforcement = new List<Reinforcement.SurfaceReinforcement>();
        [XmlAttribute("name")]
        public string name {get; set;} // identifier
        [XmlAttribute("type")]
        public string _type; // slabtype
        [XmlIgnore]
        public string type
        {
            get {return this._type;}
            set {this._type = RestrictedString.SlabType(value);}
        }
        [XmlElement("slab_part", Order=1)]
        public SlabPart slabPart {get; set;}
        [XmlElement("end", Order=2)]
        public string end {get; set;} // empty_type

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private Slab()
        {

        }

        /// <summary>
        /// Construct Slab.
        /// </summary>
        private Slab(string type, string name, SlabPart slabPart, Materials.Material material)
        {
            this.EntityCreated();
            this.name = name;
            this.type = type;
            this.slabPart = slabPart;
            this.material = material;
            this.end = "";
        }

        internal static Slab Plate(string identifier, Materials.Material material, Geometry.Region region, ShellEdgeConnection shellEdgeConnection, ShellEccentricity eccentricity, ShellOrthotropy orthotropy, List<Thickness> thickness)
        {
            plateInstance++;
            string type = "plate";
            string name = identifier + "." + plateInstance.ToString() + ".1";
            SlabPart slabPart = SlabPart.Define(name, region, thickness, material, shellEdgeConnection, eccentricity, orthotropy);
            Slab shell = new Slab(type, name, slabPart, material);
            return shell;
        }
        internal static Slab Wall(string identifier, Materials.Material material, Geometry.Region region, ShellEdgeConnection shellEdgeConnection, ShellEccentricity eccentricity, ShellOrthotropy orthotropy, List<Thickness> thickness)
        {
            // check if surface is vertical
            if (Math.Abs(region.coordinateSystem.localZ.z) > FemDesign.Tolerance.point3d)
            {
                throw new System.ArgumentException("Wall is not vertical! Create plate instead.");
            }
            
            wallInstance++;
            string type = "wall";
            string name = identifier + "." + wallInstance.ToString() + ".1";
            SlabPart slabPart = SlabPart.Define(name, region, thickness, material, shellEdgeConnection, eccentricity, orthotropy);
            Slab shell = new Slab(type, name, slabPart, material);
            return shell;
        }

        /// <summary>
        /// Set ShellEdgeConnections by indices.
        /// </summary>
        /// <param name="slab">Slab.</param>
        /// <param name="shellEdgeConnection">ShellEdgeConnection.</param>
        /// <param name="indices">Index. List of items. Use SlabDeconstruct to extract index for each respective edge.</param>
        [IsVisibleInDynamoLibrary(true)]
        public static Slab SetShellEdgeConnection(Slab slab, ShellEdgeConnection shellEdgeConnection, List<int> indices)
        {
            // clone slab
            Slab slabClone = slab.DeepClone();

            foreach (int index in indices)
            {
                if (index >= 0 & index < slab.slabPart.GetEdgeConnections().Count)
                {
                    // pass
                }
                else
                {
                    throw new System.ArgumentException("Index is out of bounds.");
                }
                
                //
                slabClone.slabPart.region.SetEdgeConnection(shellEdgeConnection, index);  

            }

            //
            return slabClone;          
        }
        /// <summary>
        /// Set average mesh size to slab. Note: Does not work for walls in FEM-Design 19.00.001.
        /// </summary>
        /// <remarks>Action</remarks>
        /// <param name="slab">Slab.</param>
        /// <param name="avgMeshSize">Average mesh size.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static Slab SetAverageSurfaceElementSize(Slab slab, double avgMeshSize)
        {
            // clone slab
            Slab slabClone = slab.DeepClone();

            //
            slabClone.slabPart.meshSize = avgMeshSize;

            // return
            return slabClone;
        }
        #region dynamo
        /// <summary>
        /// Create a plate element.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="surface">Surface. Surface must be flat.</param>
        /// <param name="shellEccentricity">ShellEccentricity. Optional, if undefined default value will be used.</param>
        /// <param name="shellOrthotropy">ShellOrthotropy. Optional, if undefined default value will be used.</param>
        /// <param name="shellEdgeConnection">ShellEdgeConnection. Optional, if undefined rigid.</param>
        /// <param name="thickness">Thickness.</param>
        /// <param name="material">Material.</param>
        /// <param name="identifier">Identifier of plate element. Optional.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static Slab Plate(Autodesk.DesignScript.Geometry.Surface surface, [DefaultArgument("ShellEccentricity.Default()")] ShellEccentricity shellEccentricity, [DefaultArgument("ShellOrthotropy.Default()")] ShellOrthotropy shellOrthotropy, [DefaultArgument("ShellEdgeConnection.Default()")] ShellEdgeConnection shellEdgeConnection,  double thickness, Materials.Material material, string identifier = "P")
        {
            // create FlatSurface
            Geometry.Region region = Geometry.Region.FromDynamo(surface);

            // create Thickness object
            List<Thickness> _thickness = new List<Thickness>();
            _thickness.Add(new Thickness(region.coordinateSystem.origin, thickness));

            // create shell
            Slab _shell = Slab.Plate(identifier, material, region, shellEdgeConnection, shellEccentricity, shellOrthotropy, _thickness);

            return _shell;
        }
        /// <summary>
        /// Create a plate element with variable thickness.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="surface">Surface. Surface must be flat.</param>
        /// <param name="shellEccentricity">ShellEccentricity. Optional, if undefined default value will be used.</param>
        /// <param name="shellOrthotropy">ShellOrthotropy. Optional, if undefined default value will be used.</param>
        /// <param name="shellEdgeConnection">ShellEdgeConnection. Optional, if undefined rigid.</param>
        /// <param name="thickness">Thickness. List of 3 items [t1, t2, t3].</param>
        /// <param name="material">Material.</param>
        /// <param name="identifier">Identifier of plate element. Optional.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static Slab PlateVariableThickness(Autodesk.DesignScript.Geometry.Surface surface, [DefaultArgument("ShellEccentricity.Default()")] ShellEccentricity shellEccentricity, [DefaultArgument("ShellOrthotropy.Default()")] ShellOrthotropy shellOrthotropy, [DefaultArgument("ShellEdgeConnection.Default()")] ShellEdgeConnection shellEdgeConnection, List<Thickness> thickness, Materials.Material material, string identifier = "P")
        {
            // create FlatSurface
            Geometry.Region region = Geometry.Region.FromDynamo(surface);

            // check length of thickness
            if (thickness.Count != 3)
            {
                throw new System.ArgumentException("Thickness must contain exactly 3 items.");
            }

            // create shell
            Slab _shell = Slab.Plate(identifier, material, region, shellEdgeConnection, shellEccentricity, shellOrthotropy, thickness);

            return _shell;
        }
        /// <summary>
        /// Create a wall element.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="surface">Surface. Surface must be flat.</param>
        /// <param name="shellEccentricity">ShellEccentricity. Optional, if undefined default value will be used.</param>
        /// <param name="shellOrthotropy">ShellOrthotropy. Optional, if undefined default value will be used.</param>
        /// <param name="shellEdgeConnection">ShellEdgeConnection. Optional, if undefined rigid.</param>
        /// <param name="thickness">Thickness.</param>
        /// <param name="material">Material.</param>
        /// <param name="identifier">Identifier of wall element. Optional.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static Slab Wall(Autodesk.DesignScript.Geometry.Surface surface,  [DefaultArgument("ShellEccentricity.Default()")] ShellEccentricity shellEccentricity, [DefaultArgument("ShellOrthotropy.Default()")] ShellOrthotropy shellOrthotropy, [DefaultArgument("ShellEdgeConnection.Default()")] ShellEdgeConnection shellEdgeConnection, double thickness, Materials.Material material, string identifier = "W")
        {
            // create FlatSurface
            Geometry.Region region = Geometry.Region.FromDynamo(surface);

            // create Thickness object
            List<Thickness> _thickness = new List<Thickness>();
            _thickness.Add(new Thickness(region.coordinateSystem.origin, thickness));

            // check if surface is vertical
            if (Math.Abs(region.coordinateSystem.localZ.z) > FemDesign.Tolerance.point3d)
            {
                throw new System.ArgumentException("Wall is not vertical! Create plate instead.");
            }

            // create shell
            Slab _shell = Slab.Wall(identifier, material, region, shellEdgeConnection, shellEccentricity, shellOrthotropy, _thickness);

            return _shell;
        }
        /// <summary>
        /// Create a wall element with variable thickness.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="surface">Surface. Surface must be flat.</param>
        /// <param name="shellEccentricity">ShellEccentricity. Optional, if undefined default value will be used.</param>
        /// <param name="shellOrthotropy">ShellOrthotropy. Optional, if undefined default value will be used.</param>
        /// <param name="shellEdgeConnection">ShellEdgeConnection. Optional, if undefined rigid.</param>
        /// <param name="thickness">Thickness. List of 2 items [t1, t2].</param>
        /// <param name="material">Material.</param>
        /// <param name="identifier">Identifier of wall element. Optional.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static Slab WallVariableThickness(Autodesk.DesignScript.Geometry.Surface surface,  [DefaultArgument("ShellEccentricity.Default()")] ShellEccentricity shellEccentricity, [DefaultArgument("ShellOrthotropy.Default()")] ShellOrthotropy shellOrthotropy, [DefaultArgument("ShellEdgeConnection.Default()")] ShellEdgeConnection shellEdgeConnection, List<Thickness> thickness, Materials.Material material, string identifier = "W")
        {
            // create FlatSurface
            Geometry.Region region = Geometry.Region.FromDynamo(surface);

            // check if surface is vertical
            if (Math.Abs(region.coordinateSystem.localZ.z) > FemDesign.Tolerance.point3d)
            {
                throw new System.ArgumentException("Wall is not vertical! Create plate instead.");
            }

            // check length of thickness
            if (thickness.Count != 2)
            {
                throw new System.ArgumentException("Thickness must contain exactly 2 items.");
            }

            // create shell
            Slab _shell = Slab.Wall(identifier, material, region, shellEdgeConnection, shellEccentricity, shellOrthotropy, thickness);

            return _shell;
        }
        /// <summary>
        /// Set local x- and z-axes. Local y-axis will be defined according to a right handed coordinate system.
        /// </summary>
        /// <param name="localX">Vector. Must be perpendicular to slab surface normal.</param>
        /// <param name="localZ">Vector. Must be parallell to slab surface normal.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public Slab SetLocalAxes(Autodesk.DesignScript.Geometry.Vector localX, Autodesk.DesignScript.Geometry.Vector localZ)
        {
            //
            Slab slab = this.DeepClone();

            // set local x and local z
            slab.slabPart.localX = Geometry.FdVector3d.FromDynamo(localX);
            slab.slabPart.localZ = Geometry.FdVector3d.FromDynamo(localZ);

            //
            return slab;
        }
        #endregion
    }
}