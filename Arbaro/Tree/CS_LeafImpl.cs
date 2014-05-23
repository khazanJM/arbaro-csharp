using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Transformation;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  Ported from original Arbaro software

namespace Arbaro2.Arbaro.Tree
{
    public class CS_LeafImpl : CS_Leaf
    {
        public DX_Transformation transf;
        //	Params par;

        public override DX_Transformation getTransformation() { return transf; }

        public CS_LeafImpl(DX_Transformation trf)
        {
            //		par = params;
            transf = trf;
        }

        /**
         *	Leaf rotation toward light
         */
        private void setLeafOrientation(CS_Params par)
        {          
            if (par.LeafBend == 0) return;                  

            // FIXME: make this function as fast as possible - a tree has a lot of leafs

            // rotation outside 
            Vector3 pos = transf.getT();
            // the z-vector of transf is parallel to the
            // axis of the leaf, the y-vector is the normal
            // (of the upper side) of the leaf
            Vector3 norm = transf.getY3();

            float tpos = (float)(Math.Atan2(pos.Y, pos.X) * 180 / Math.PI);
            float tbend = tpos - (float)(Math.Atan2(norm.Y, norm.X) * 180 / Math.PI); ;
            // if (tbend>180) tbend = 360-tbend;

            float bend_angle = par.LeafBend * tbend;
            // transf = transf.rotz(bend_angle);
            // rotate about global z-axis
            transf = transf.rotaxis(bend_angle, DX_Transformation.Z_AXIS);

            // rotation up
            norm = transf.getY3();
            float fbend = (float)(Math.Atan2((float)Math.Sqrt(norm.X * norm.X + norm.Y * norm.Y), norm.Z) * 180 / Math.PI); 

            bend_angle = par.LeafBend * fbend;

            transf = transf.rotx(bend_angle);

            //		this is from the paper, but is equivalent with
            //      local x-rotation (upper code line)
            //		
            //		double orientation = Vector.atan2(norm.getY(),norm.getX());
            //		transf = transf
            //			.rotaxis(-orientation,Vector.Z_AXIS)
            //			.rotx(bend_angle)
            //			.rotaxis(orientation,Vector.Z_AXIS);
             
        }


        /**
         * Makes the leave. Does nothing at the moment, because
         * all the values can be calculated in the constructor 
         */
        public void make(CS_Params par)
        {
            setLeafOrientation(par);
        }

        /* (non-Javadoc)
         * @see net.sourceforge.arbaro.tree.TraversableLeaf#traverseTree(net.sourceforge.arbaro.tree.TreeTraversal)
         */
        public override bool traverseTree(CS_TreeTraversal traversal)
        {
            return traversal.visitLeaf(this);
        }
    }
}
