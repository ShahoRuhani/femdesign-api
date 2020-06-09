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
    public class FdVector3d
    {
        [XmlAttribute("x")]
        public double x { get; set; }
        [XmlAttribute("y")]
        public double y { get; set; }
        [XmlAttribute("z")]
        public double z { get; set; }

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        public FdVector3d()
        {
            
        }

        /// <summary>
        /// Construct FdVector3d by i, j and k components.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        internal FdVector3d(double i, double j, double k)
        {
            this.x = i;
            this.y = j;
            this.z = k;
        }

        /// <summary>
        /// Construct FdVector3d by start and end points.
        /// </summary>
        /// <param name="p0">Start point</param>
        /// <param name="p1">End point</param>
        internal FdVector3d(FdPoint3d p0, FdPoint3d p1)
        {
            this.x = p1.x - p0.x;
            this.y = p1.y - p0.y;
            this.z = p1.z - p0.z;
        }

        /// <summary>
        /// Calculate length of FdVector3d.
        /// </summary>
        internal double Length()
        {
            double len = Math.Pow((Math.Pow(this.x, 2) + Math.Pow(this.y, 2) + Math.Pow(this.z, 2)), (1.0/2.0));
            return len;
        }

        /// <summary>
        /// Normalize FdVector3d (i.e. scale so that length equals 1).
        /// </summary>
        internal FdVector3d Normalize()
        {
            double l = this.Length();
            FdVector3d normal = new FdVector3d(this.x / l, this.y / l, this.z / l);
            return normal;
        }

        /// <summary>
        /// Calculate cross-product of this FdVector3d and v FdVector3d.
        /// </summary>
        internal FdVector3d Cross(FdVector3d v)
        {
            FdVector3d v0 = this;
            FdVector3d v1 = v;

            double i, j, k;
            // https://en.wikipedia.org/wiki/Cross_product#Coordinate_notation
            i = v0.y * v1.z - v0.z * v1.y;
            j = v0.z * v1.x - v0.x * v1.z;
            k = v0.x * v1.y - v0.y * v1.x;

            FdVector3d v2 = new FdVector3d(i, j, k);;
            return v2;
        }

        /// <summary>
        /// Calculate dot-product of this FdVector3d and v FdVector3d.
        /// </summary>
        internal double Dot(FdVector3d v)
        {
            FdVector3d v0 = this;
            FdVector3d v1 = v;

            // https://en.wikipedia.org/wiki/Dot_product#Algebraic_definition
            double s = v0.x*v1.x + v0.y*v1.y + v0.z*v1.z;
            return s;

        }

        /// <summary>
        /// Scale this FdVector3d by s.
        /// </summary>
        internal FdVector3d Scale(double s)
        {   
            return new FdVector3d(this.x * s, this.y * s, this.z * s);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            FdVector3d v = obj as FdVector3d;
            if ((System.Object)v == null)
            {
                return false;
            }
            return (x == v.x) && (y == v.y) && (z == v.z);            
        }

        public bool Equals(FdVector3d v)
        {
            if ((object)v == null)
            {
                return false;
            }
            return (x == v.x) && (y == v.y) && (z == v.z);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
        #region dynamo
        /// <summary>
        /// Create FdVector3d from Dynamo vector.
        /// </summary>
        internal static FdVector3d FromDynamo(Autodesk.DesignScript.Geometry.Vector vector)
        {
            FdVector3d newVector = new FdVector3d(vector.X, vector.Y, vector.Z);
            return newVector;
        }

        /// <summary>
        /// Create Dynamo vector from FdVector3d.
        /// </summary>
        public Autodesk.DesignScript.Geometry.Vector ToDynamo()
        {
            return Autodesk.DesignScript.Geometry.Vector.ByCoordinates(this.x, this.y, this.z);
        }
        #endregion
    }
}