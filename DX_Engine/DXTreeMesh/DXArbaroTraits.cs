using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine.DXTreeMesh
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DXArbaroNullTrait
    {
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXArbaroVertexTrait
    {
        public Vector3 Position;
        public DXArbaroVertexTrait(Vector3 p) { Position = new Vector3(p.X, p.Z, p.Y); }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXArbaroHalfedgeTrait
    {
        public Vector2 UV;
    }
    
}
