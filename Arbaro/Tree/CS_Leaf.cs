using Arbaro2.Arbaro.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  Ported from original Arbaro software

namespace Arbaro2.Arbaro.Tree
{
    public abstract class CS_Leaf
    {
        /**
         * used with TreeTraversal interface
         *  
         * @param traversal
         * @return when false stop travers tree at this level
         */
        
        public abstract bool traverseTree(CS_TreeTraversal traversal);

        /**
         * @return the leaf's transformation matrix containing
         * the position vector and the rotation matrix.
         */
        public abstract CS_Transformation getTransformation();
    }
}
