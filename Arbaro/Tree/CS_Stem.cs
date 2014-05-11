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
    public abstract class CS_Stem
    {
        /**
         * 
         * @return an enumeration of the stems sections
         */
        public abstract List<CS_SegmentImpl> getSections();

        /**
         * 
         * @return an section offset for clones, because uv-Coordinates should
         * be at same coordinates for stems and theire clones
         */
        public abstract int getCloneSectionOffset();

        /**
         * a vector with the smalles coordinates of the stem
         */
        public abstract Vector3 getMinPoint();

        /**
         * a vector with the heighest coordinates of the stem
         */
        public abstract Vector3 getMaxPoint();

        /**
         * The position of the stem in the tree. 0.1c2.3 means:
         * fourth twig of the third clone of the second branch growing
         * out of the first (only?) trunk 
         * 
         * @return The stem position in the tree as a string
         */
        public abstract String getTreePosition();

        /**
         * 
         * @return the stem length
         */
        public abstract double getLength();

        /**
         * @return the radius at the stem base
         */
        public abstract double getBaseRadius();

        /**
         * @return the radius at the stem peak
         */
        public abstract double getPeakRadius();

        /**
         * @return the stem level, 0 means it is a trunk
         */
        public abstract int getLevel();

        /**
         * used with TreeTraversal interface
         * 
         * @param traversal
         * @return when false stop traverse tree at this level
         */
        public abstract bool traverseTree(CS_TreeTraversal traversal);

        /**
         * 
         * @return the number leaves of the stem 
         */
        public abstract long getLeafCount();

        /**
         * 
         * @return true, if this stem is a clone of another stem
         */
        public abstract bool isClone();

        /**
         * 
         * @return this stem should be smoothed, so output normals
         * for Povray meshes
         */
        public abstract bool isSmooth();

        /**
         * 
         * @return the transformation of the stem, containing the position
         * vector and the rotation matrix of the stem base 
         */
        public abstract DX_Transformation getTransformation();

    }
}
