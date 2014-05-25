using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//
//      DXMesh and all related classes are an implementation
//      of an half-edge data structure to store polygonal mesh
//      It is derived from the work of Alexander Kolliopoulos
//      http://www.dgp.toronto.edu/~alexk/halfedge.pdf
//      
//      My implementation is highly inspired from what he did
//      
//

namespace Arbaro2.DX_Engine.DXMesh
{
    public class DXBadTopologyException : Exception
    {
        public DXBadTopologyException(string msg) : base(msg) {}
    }
}
