using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine.DXMesh
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DXNullTrait
    {  
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXVertexBaseTrait
    {
        public Vector3 Position;
    }

    // the current version of the TreeMesh is :
    //using DXTreeMesh = DXMesh<DXNullTrait, DXNullTrait, DXNullTrait, DXVertexBaseTrait>;
}
