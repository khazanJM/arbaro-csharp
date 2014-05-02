using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Transformation;
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
        public CS_Transformation transf;
        //	Params par;

        public override CS_Transformation getTransformation() { return transf; }

        public CS_LeafImpl(CS_Transformation trf)
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
            CS_Vector pos = transf.getT();
            // the z-vector of transf is parallel to the
            // axis of the leaf, the y-vector is the normal
            // (of the upper side) of the leaf
            CS_Vector norm = transf.getY();

            double tpos = CS_Vector.atan2(pos.getY(), pos.getX());
            double tbend = tpos - CS_Vector.atan2(norm.getY(), norm.getX());
            // if (tbend>180) tbend = 360-tbend;

            double bend_angle = par.LeafBend * tbend;
            // transf = transf.rotz(bend_angle);
            // rotate about global z-axis
            transf = transf.rotaxis(bend_angle, CS_Vector.Z_AXIS);

            // rotation up
            norm = transf.getY();
            double fbend = CS_Vector.atan2(Math.Sqrt(norm.getX() * norm.getX() + norm.getY() * norm.getY()),
                    norm.getZ());

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
