using OpenTK;

namespace amulware.Graphics
{
    static public class Extensions
    {
        public static Vector3 Times(this Matrix3 matrix, Vector3 vector)
        {
            return new Vector3(
                matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z,
                matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z,
                matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z
                );
        }


        public static Matrix3 ScaleBy(this Matrix3 matrix, float scale)
        {
            return new Matrix3(
                matrix.M11 * scale, matrix.M12 * scale, matrix.M13 * scale,
                matrix.M21 * scale, matrix.M22 * scale, matrix.M23 * scale,
                matrix.M31 * scale, matrix.M32 * scale, matrix.M33 * scale
                );
        }
    }
}