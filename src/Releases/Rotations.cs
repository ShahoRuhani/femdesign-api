// https://strusoft.com/

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Releases
{
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class Rotations: StiffnessType
    {
        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private Rotations()
        {

        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="x_neg">Cx' compression.</param>
        /// <param name="x_pos">Cx' tension.</param>
        /// <param name="y_neg">Cy' compression.</param>
        /// <param name="y_pos">Cy' tension.</param>
        /// <param name="z_neg">Cz' compression.</param>
        /// <param name="z_pos">Cz' tension.</param>
        private Rotations(double x_neg, double x_pos, double y_neg, double y_pos, double z_neg, double z_pos)
        {
            this.x_neg = x_neg;
            this.x_pos = x_pos;
            this.y_neg = y_neg;
            this.y_pos = y_pos;
            this.z_neg = z_neg;
            this.z_pos = z_pos; 
        }
        
        /// <summary>
        /// Define a new rotations release [kN/rad or kN/m/rad].
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="x_neg">Cx' compression.</param>
        /// <param name="x_pos">Cx' tension.</param>
        /// <param name="y_neg">Cy' compression.</param>
        /// <param name="y_pos">Cy' tension.</param>
        /// <param name="z_neg">Cz' compression.</param>
        /// <param name="z_pos">Cz' tension.</param>
        [IsVisibleInDynamoLibrary(true)]
        public static Rotations Define(double x_neg = 0, double x_pos = 0, double y_neg = 0, double y_pos = 0, double z_neg = 0, double z_pos = 0)
        {
            return new Rotations(x_neg, x_pos, y_neg, y_pos, z_neg, z_pos);
        }
        /// <summary>
        /// Define a rigid rotations release for a point-type release (1e+10 kNm/rad).
        /// </summary>
        [IsVisibleInDynamoLibrary(true)]
        public static Rotations RigidPoint()
        {
            return new Rotations(10000000000, 10000000000, 10000000000, 10000000000, 10000000000, 10000000000);
        }
        /// <summary>
        /// Define a rigid rotations release for a line-type release (1e+07 kNm/m/rad).
        /// </summary>
        [IsVisibleInDynamoLibrary(true)]
        public static Rotations RigidLine()
        {
            return new Rotations(10000000, 10000000, 10000000, 10000000,  10000000, 10000000);
        }
        /// <summary>
        /// Define a free rotations release.
        /// </summary>
        [IsVisibleInDynamoLibrary(true)]
        public static Rotations Free()
        {
            return new Rotations(0,0,0,0,0,0);
        }   
    }
}