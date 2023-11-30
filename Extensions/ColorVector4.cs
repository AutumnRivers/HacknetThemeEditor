using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Numerics = System.Numerics;

namespace HacknetThemeEditor.Extensions
{
    public static class ColorExtensions
    {
        public static Numerics.Vector4 ToNumVector4(this Vector4 xnaVector4)
        {
            return new Numerics.Vector4(
                x : xnaVector4.X,
                y : xnaVector4.Y,
                z : xnaVector4.Z,
                w : xnaVector4.W
                );
        }
    }

    public static class NumericsExtensions
    {
        public static Vector4 ToXNAVector4(this Numerics.Vector4 numVec4)
        {
            return new Vector4(
                x : numVec4.X,
                y : numVec4.Y,
                z : numVec4.Z,
                w : numVec4.W
                );
        }
    }
}
