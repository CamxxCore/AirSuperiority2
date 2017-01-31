using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CMatrix : IEquatable<Matrix>
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public CMatrix(Matrix matrix)
        {
            M11 = matrix.M11;
            M12 = matrix.M12;
            M13 = matrix.M13;
            M14 = matrix.M14;
            M21 = matrix.M21;
            M22 = matrix.M22;
            M23 = matrix.M23;
            M24 = matrix.M24;
            M31 = matrix.M31;
            M32 = matrix.M32;
            M33 = matrix.M33;
            M34 = matrix.M34;
            M41 = matrix.M41;
            M42 = matrix.M42;
            M43 = matrix.M43;
            M44 = matrix.M44;
        }

        public static implicit operator CMatrix(Matrix mx)
        {
            return new CMatrix(mx);
        }

        public static bool operator ==(CMatrix mx1, Matrix mx2)
        {
            return mx1.Equals(mx2);
        }

        public static bool operator !=(CMatrix mx1, Matrix mx2)
        {
            return !mx1.Equals(mx2);
        }

        public static bool operator ==(Matrix mx1, CMatrix mx2)
        {
            return mx1.Equals(mx2);
        }

        public static bool operator !=(Matrix mx1, CMatrix mx2)
        {
            return !mx1.Equals(mx2);
        }

        public static bool operator ==(CMatrix mx1, CMatrix mx2)
        {
            return mx1.Equals(mx2);
        }

        public static bool operator !=(CMatrix mx1, CMatrix mx2)
        {
            return !mx1.Equals(mx2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return ((CMatrix)obj == this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(CMatrix mx)
        {
            return mx.M11 == M11 && mx.M12 == M12 && mx.M13 == M13 && mx.M14 == M14 &&
                mx.M21 == M21 && mx.M22 == M22 && mx.M23 == M23 && mx.M24 == M24 &&
                mx.M31 == M31 && mx.M32 == M32 && mx.M33 == M33 && mx.M34 == M34 &&
                mx.M41 == M41 && mx.M42 == M42 && mx.M43 == M43 && mx.M44 == M44;
        }

        public bool Equals(Matrix mx)
        {
            return mx.M11 == M11 && mx.M12 == M12 && mx.M13 == M13 && mx.M14 == M14 &&
                mx.M21 == M21 && mx.M22 == M22 && mx.M23 == M23 && mx.M24 == M24 &&
                mx.M31 == M31 && mx.M32 == M32 && mx.M33 == M33 && mx.M34 == M34 &&
                mx.M41 == M41 && mx.M42 == M42 && mx.M43 == M43 && mx.M44 == M44;
        }
    }
}
