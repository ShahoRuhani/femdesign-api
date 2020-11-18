// https://strusoft.com/
using System;
using System.Xml.Serialization;

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Geometry
{
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class FdCoordinateSystem
    {
        [XmlElement("local_pos", Order=1)]
        public FdPoint3d Origin { get; set; }
        [XmlElement("local_x", Order=2)]
        public FdVector3d _localX;
        [XmlIgnore]
        public FdVector3d LocalX
        {
            get
            {
                if (this._localX == null)
                {
                    throw new System.ArgumentException("LocalX is null. Property must be set.");
                }
                else
                {
                    return this._localX;
                }
            }
        }        
        [XmlElement("local_y", Order=3)]
        public FdVector3d _localY;
        [XmlIgnore]
        public FdVector3d LocalY
        {
            get
            {
                if (this._localY == null)
                {
                    throw new System.ArgumentException("LocalY is null. Property must be set.");
                }
                else
                {
                    return this._localY;
                }
            } 
        }
        [XmlIgnore]
        public FdVector3d _localZ;
        [XmlIgnore]
        public FdVector3d LocalZ
        {
            get
            {
                if (this.LocalX == null || this.LocalY == null)
                {
                    throw new System.ArgumentException("Impossible to get z-axis as either this.localX or this.localY is null.");
                }
                else
                {     
                    return this.LocalX.Cross(LocalY).Normalize();
                }
            }
        }

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private FdCoordinateSystem()
        {

        }

        /// <summary>
        /// Construct FdCoordinateSystem from origin point and local x and y axes.
        /// </summary>
        public FdCoordinateSystem(FdPoint3d origin, FdVector3d localX, FdVector3d localY)
        {
            this.Origin = origin;
            this._localX = localX;
            this._localY = localY;
            this._localZ = localX.Cross(localY);
            
            if (!this.IsComplete())
            {
                throw new System.ArgumentException("The defined coordinate system is not complete!");  
            }

            if (!this.IsOrthogonal())
            {
                throw new System.ArgumentException($"The defined coordinate system is not orthogonal within the tolerance {Tolerance.DotProduct}");
            }
        }

        /// <summary>
        /// Construct FdCoordinateSystem from origin point and local x, y, z axes.
        /// </summary>
        public FdCoordinateSystem(FdPoint3d origin, FdVector3d localX, FdVector3d localY, FdVector3d localZ)
        {
            this.Origin = origin;
            this._localX = localX;
            this._localY = localY;
            this._localZ = localZ;

            if (!this.IsComplete())
            {
                throw new System.ArgumentException("The defined coordinate system is not complete!");
            }

            if (!this.IsOrthogonal())
            {
                throw new System.ArgumentException($"The defined coordinate system is not orthogonal within the tolerance {Tolerance.DotProduct}");
            }
        }

        /// <summary>
        /// Check if this coordinate system is complete. 
        /// </summary>
        /// <returns></returns>
        public bool IsComplete()
        {
            return (this.Origin != null) && (this._localX != null) && (this._localY != null) && (this._localZ != null);
        }

        /// <summary>
        /// Check if this coordinate system is orthogonal within the tolernace.
        /// </summary>
        /// <returns></returns>
        public bool IsOrthogonal()
        {
            return (Math.Abs(this._localX.Dot(this._localY)) < Tolerance.DotProduct) && this._localX.Cross(this._localY).Equals(this._localZ, Tolerance.Point3d);
        }

        /// <summary>
        /// Set X-axis and rotate coordinate system accordingly around Z-axis.
        /// </summary>
        public void SetXAroundZ(FdVector3d vector)
        {
            // try to set axis
            FdVector3d val = vector.Normalize();
            FdVector3d z = this.LocalZ;

            double dot = z.Dot(val);
            if (Math.Abs(dot) < Tolerance.DotProduct)
            {
                this._localX = val;
                this._localY = z.Cross(val); // follows right-hand-rule
            }
            else
            {
                throw new System.ArgumentException($"The passed X-axis is not perpendicular to Z-axis. The dot-product is {dot}, but should be 0");
            }
        }

        /// <summary>
        /// Set Y-axis and rotate coordinate system accordingly around X-Axis.
        /// </summary>
        public void SetYAroundX(FdVector3d vector)
        {
            // try to set axis
            FdVector3d val = vector.Normalize();
            FdVector3d x = this.LocalX;

            double dot = x.Dot(val);
            if (Math.Abs(dot) < Tolerance.DotProduct)
            {
                this._localY = val;
                this._localZ = x.Cross(val); // follows right-hand-rule
            }
            else
            {
                throw new System.ArgumentException($"The passed Y-axis is not perpendicular to X-axis. The dot-product is {dot}, but should be 0");
            }
        }

        /// <summary>
        /// Set Y-axis and rotate coordinate system accordingly around Z-axis
        /// </summary>
        public void SetYAroundZ(FdVector3d vector)
        {
            // try set axis
            FdVector3d val = vector.Normalize();
            FdVector3d z = this.LocalZ;

            double dot = z.Dot(val);
            if (Math.Abs(dot) < Tolerance.DotProduct)
            {
                this._localY = val;
                this._localX = val.Cross(z); // follows right-hand-rule
            }
            
            else
            {
                throw new System.ArgumentException($"Y-axis is not perpendicular to Z-axis. The dot-product is {dot}, but should be 0");
            }
        }

        /// <summary>
        /// Set Z-axis and rotate coordinate system accordingly around X-axis
        /// </summary>
        public void SetZAroundX(FdVector3d vector)
        {
            // try to set axis
            Geometry.FdVector3d val = vector.Normalize();
            Geometry.FdVector3d x = this.LocalX;

            double dot = x.Dot(val);
            if (Math.Abs(dot) < Tolerance.DotProduct)
            {
                this._localZ = val;
                this._localY = val.Cross(x); // follows right-hand-rule
            }
            
            else
            {
                throw new System.ArgumentException($"Z-axis is not perpendicular to X-axis. The dot-product is {dot}, but should be 0");
            }
        }

        /// <summary>
        /// Orient this coordinate system to GCS as if this coordinate system was constrained as an edge (i.e x' is constrained by the edge)
        /// </summary>
        public void OrientEdgeTypeLcsToGcs()
        {
            if (this.IsComplete())
            {
                // if LocalX is parallell to UnitZ set (rotate) LocalY to UnitY
                int par = this.LocalX.Parallel(Geometry.FdVector3d.UnitZ());
                if (par == 1 || par == -1)
                {
                    this.SetYAroundX(FdVector3d.UnitY());
                }

                // else set (rotate) LocalY to UnitZ cross LocalX
                else
                {
                    this.SetYAroundX(FdVector3d.UnitZ().Cross(this.LocalX).Normalize());
                }
            }

            else
            {
                throw new System.ArgumentException("Impossible to orient axes as the passed coordinate system is incomplete.");
            }            
        }

        /// <summary>
        /// Orient this coordinate system to GCS as if this coordinate system was constrained as a plane (i.e. x' and y' are constrianed by the plane)
        /// If plane is not vertical plane z' will be orientated up.
        /// If plane is vertical y' will be orientated up.
        /// </summary>
        public void OrientPlaneTypeLcsToGcs()
        {
            double dot = this.LocalZ.Normalize().Dot(FdVector3d.UnitZ());
            if (dot == 1)
            {
                // the plane is horisontal and z' is equal to Z

                // set x' to X
                this.SetXAroundZ(FdVector3d.UnitX());
            }
            else if (dot < 1 && dot > 0)
            {
                // the plane is not horisontal nor vertical but z' is pointing up

                // set x' to the cross-product of z' and Z
                // this.SetXAroundZ(FdVector3d.UnitZ().Cross(this.LocalZ));
            }
            else if (dot == 0)
            {
                // the plane is vertical 

                // set y' to Z. This is the equivalent as setting x' to the cross-product of z' and Z in this case.
                this.SetYAroundZ(FdVector3d.UnitZ());
            }
            else if (dot < 0 && dot > -1)
            {
                // the plane is not horisontal nor vertical, z' is pointing down

                // flip coordinate system around x' so that z' points up
                this.SetZAroundX(this.LocalZ.Reverse());

                // set x' to the cross-product of z' and Z
                // this.SetXAroundZ(FdVector3d.UnitZ().Cross(this.LocalZ));

            }
            else if (dot == -1)
            {
                // the plane is horisontal but z' is equal to negative Z

                // flip coordinate system around x' so that z' points up
                this.SetZAroundX(this.LocalZ.Reverse());

                // set x' to X
                // this.SetXAroundZ(FdVector3d.UnitX());
            }
            else
            {
                throw new System.ArgumentException($"Impossible to orient axes. Dot product, {dot}, should be between -1 and 1");
            }
        }


        #region dynamo
        /// <summary>
        /// Create FdCoordinateSystem from Dynamo coordinate system of a Line or NurbsCurve(?).
        /// This method realignes the coordinate system.
        /// </summary>
        internal static FdCoordinateSystem FromDynamoCoordinateSystemLine(Autodesk.DesignScript.Geometry.CoordinateSystem obj)
        {
            FdPoint3d origin = FdPoint3d.FromDynamo(obj.Origin);
            FdVector3d localX = FdVector3d.FromDynamo(obj.YAxis);
            FdVector3d localY = FdVector3d.FromDynamo(obj.XAxis.Reverse());
            FdVector3d localZ = localX.Cross(localY).Normalize();
            return new FdCoordinateSystem(origin, localX, localY, localZ);
        }

        /// <summary>
        /// 
        /// Create FdCoordinateSystem from Dynamo coordinate system of a Arc or Circle.
        /// Dynamo Arcs and Circles follow left-hand rule.
        /// This method realignes the coordinate system.
        /// </summary>
        internal static FdCoordinateSystem FromDynamoCoordinateSystemArcOrCircle(Autodesk.DesignScript.Geometry.CoordinateSystem obj)
        {
            FdPoint3d origin = FdPoint3d.FromDynamo(obj.Origin);
            FdVector3d localX = FdVector3d.FromDynamo(obj.YAxis);
            FdVector3d localY = FdVector3d.FromDynamo(obj.XAxis);
            FdVector3d localZ = localX.Cross(localY).Normalize();
            return new FdCoordinateSystem(origin, localX, localY, localZ);
        }

        /// <summary>
        /// Create FdCoordinateSystem from Dynamo coordinate system of a Surface.
        /// No realignment neccessary.
        /// </summary>
        internal static FdCoordinateSystem FromDynamoCoordinateSystemSurface(Autodesk.DesignScript.Geometry.CoordinateSystem obj)
        {
            FdPoint3d origin = FdPoint3d.FromDynamo(obj.Origin);
            FdVector3d localX = FdVector3d.FromDynamo(obj.XAxis);
            FdVector3d localY = FdVector3d.FromDynamo(obj.YAxis);
            FdVector3d localZ = FdVector3d.FromDynamo(obj.ZAxis);
            return new FdCoordinateSystem(origin, localX, localY, localZ);
        }

        /// <summary>
        /// Create FdCoordinateSystem from Dynamo coordinate system on curve mid u-point.
        /// </summary>
        internal static FdCoordinateSystem FromDynamoCurve(Autodesk.DesignScript.Geometry.Curve obj)
        {
            // CoordinateSystemAtParameter returns a coordinate system on curve
            // with origin at the point at the given parameter.
            // The XAxis is aligned with the curve normal,
            // the YAxis is aligned with the curve tangent at this point, 
            // and the ZAxis is aligned with the up-vector or binormal at this point
            Autodesk.DesignScript.Geometry.CoordinateSystem cs = obj.CoordinateSystemAtParameter(0.5);

            // Note: Arcs and Circles in Dynamo are defined with left-hand rule while coordinate system is defined by right-hand rule
            if (obj.GetType() == typeof(Autodesk.DesignScript.Geometry.Arc) || obj.GetType() == typeof(Autodesk.DesignScript.Geometry.Circle))
            {
                return FdCoordinateSystem.FromDynamoCoordinateSystemArcOrCircle(cs);
            }
            else
            {
                return FromDynamoCoordinateSystemLine(cs);
            }
        }

        /// <summary>
        /// Create FdCoordinateSystem from Dynamo coordinate system on surface mid u/v-point.
        /// </summary>
        internal static FdCoordinateSystem FromDynamoSurface(Autodesk.DesignScript.Geometry.Surface obj)
        {
            Autodesk.DesignScript.Geometry.CoordinateSystem cs = obj.CoordinateSystemAtParameter(0.5, 0.5);
            return FdCoordinateSystem.FromDynamoCoordinateSystemSurface(cs);
        }
        #endregion

    }
}