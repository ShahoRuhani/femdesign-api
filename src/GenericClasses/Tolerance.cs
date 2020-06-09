using System;

namespace FemDesign
{
    /// <summary>
    /// Tolerance. This class contains definitions for different project specific tolerances.
    /// </summary>
    internal class Tolerance
    {
        /// <summary>
        /// Predefined brep tolerance.
        /// </summary>
        internal static double brep = Math.Pow(10, -6);

        /// <summary>
        /// Predefined tolerance when controlling if two vectors, v0 and v1, are perpendicular by assuming v0.Dot(v1) == 0.
        /// </summary>
        /// <returns></returns>
        internal static double dotProduct = Math.Pow(10, -10);

        /// <summary>
        /// Predefined tolernace for length evaluations.
        /// </summary>
        internal static double lengthComparison = Math.Pow(10, -5);

        /// <summary>
        /// Predefined point3d tolerance.
        /// </summary>
        internal static double point3d = Math.Pow(10, -15);
    }
}