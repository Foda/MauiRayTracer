using Microsoft.Maui.Graphics;
using RayTrace.Lights;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public static class Utils
    {
        public static Vector3 ToVector3(this SKColor color)
        {
            return new Vector3(color.Red / 255.0f, color.Green / 255.0f, color.Blue / 255.0f);
        }

        public static Vector3 RandomInUnitDisk()
        {
            Random r = new Random();
            while (true)
            {
                var p = new Vector3(
                    (float)((r.NextDouble() * 2) - 1),
                    (float)((r.NextDouble() * 2) - 1),
                    0);

                if (p.LengthSquared() >= 1)
                    continue;

                return p;
            }
        }

        public static Vector3 RandomInUnitSphere()
        {
            Random r = new Random();
            while (true)
            {
                var p = new Vector3(
                    (float)((r.NextDouble() * 2) - 1),
                    (float)((r.NextDouble() * 2) - 1),
                    (float)((r.NextDouble() * 2) - 1));

                if (p.LengthSquared() >= 1)
                    continue;

                return p;
            }
        }

        public static Vector3 RandomInHemiphere(Vector3 normal)
        {
            Random r = new Random();
            Vector3 unitSphere = RandomInUnitSphere();
            if (Vector3.Dot(unitSphere, normal) > 0)
                return unitSphere;
            return -unitSphere;
        }

        public static Vector3 GetConeSample(Random rand, Vector3 direction, float coneAngle)
        {
            float cosAngle = MathF.Cos(coneAngle);

            // Generate points on the spherical cap around the north pole [1].
            // [1] See https://math.stackexchange.com/a/205589/81266
            float z = (float)rand.NextDouble() * (1.0f - cosAngle) + cosAngle;
            float phi = (float)rand.NextDouble() * 2.0f * MathF.PI;

            float x = MathF.Sqrt(1.0f - z * z) * MathF.Cos(phi);
            float y = MathF.Sqrt(1.0f - z * z) * MathF.Sin(phi);
            Vector3 north = new Vector3(0.0f, 0.0f, 1.0f);

            // Find the rotation axis `u` and rotation angle `rot` [1]
            Vector3 axis = Vector3.Normalize(Vector3.Cross(north, Vector3.Normalize(direction)));
            float angle = MathF.Acos(Vector3.Dot(Vector3.Normalize(direction), north));

            // Convert rotation axis and angle to 3x3 rotation matrix [2]

            Matrix4x4 R = Matrix4x4.CreateFromAxisAngle(axis, angle);
            return Vector3.Transform(new Vector3(x, y, z), R);
        }

        public static Vector3 ApplyMatrix(this Vector3 self, Matrix4x4 matrix)
        {
            return new Vector3(
                matrix.M11 * self.X + matrix.M12 * self.Y + matrix.M13 * self.Z,
                matrix.M21 * self.X + matrix.M22 * self.Y + matrix.M23 * self.Z,
                matrix.M31 * self.X + matrix.M32 * self.Y + matrix.M33 * self.Z
            );
        }
    }
}
