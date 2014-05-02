using Arbaro2.Arbaro.Transformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.Arbaro.Tree
{
    public abstract class CS_Tree
    {

        /**
         * used with the TreeTraversal interface
         * 
         * @param traversal
         * @return when false stop travers the tree
         */
        public abstract bool traverseTree(CS_TreeTraversal traversal);

        /**
         * 
         * @return the number of all stems of all levels of the tree
         */
        public abstract long getStemCount();

        /**
         * 
         * @return the number of leaves of the tree
         */
        public abstract long getLeafCount();

        /**
         * 
         * @return a vector with the highest coordinates of the tree.
         * (Considering all stems of all levels)
         */
        public abstract CS_Vector getMaxPoint();

        /**
         * 
         * @return a vector with the lowest coordinates of the tree.
         * (Considering all stems of all levels)
         */
        public abstract CS_Vector getMinPoint();

        /**
         * 
         * @return the seed of the tree. It is used for randomnization.
         */
        public abstract int getSeed();

        /**
         * 
         * @return the height of the tree (highest z-coordinate)
         */
        public abstract double getHeight();

        /**
         * 
         * @return the widht of the tree (highest value of sqrt(x*x+y*y))
         */
        public abstract double getWidth();

        /**
         * Writes the trees parameters to a stream
         * @param w
         */
        public abstract void paramsToXML(StreamWriter w);

        /**
         * 
         * @return the tree species name
         */
        public abstract String getSpecies();

        /**
         *  
         * @return the tree stem levels
         */
        public abstract int getLevels();

        /**
         * 
         * @return the leaf shape name
         */
        public abstract String getLeafShape();

        /**
         * 
         * @return the leaf width
         */
        public abstract double getLeafWidth();

        /**
         * 
         * @return the leaf length
         */
        public abstract double getLeafLength();

        /**
         * 
         * @return the virtual leaf stem length, i.e. the distance of
         * the leaf from the stem center line
         */
        public abstract double getLeafStemLength();

        /**
         * Use this for verbose output when generating a mesh or
         * exporting a tree 
         * 
         * @param level
         * @return an information string with the number of section
         * points for this stem level and if smoothing should be used
         */
        public abstract String getVertexInfo(int level);
	
    }
}
