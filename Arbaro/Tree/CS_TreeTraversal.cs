using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.Arbaro.Tree
{
    public abstract class CS_TreeTraversal
    {
        /**
         * going into a Tree
         * 
         * @param tree
         * @return when false, stop traversal at this level
         */
        public abstract bool enterTree(CS_Tree tree);

        /**
         * coming out of a Tree
         * 
         * @param tree
         * @return when false, stop traversal at this level
         */
        public abstract bool leaveTree(CS_Tree tree);

        /**
         * going into a Stem
         * 
         * @param stem
         * @return when false, stop traversal at this level
         */
        public abstract bool enterStem(CS_Stem stem);

        /**
         * coming out of a Stem
         * 
         * @param stem
         * @return when false, stop traversal at this level
         */
        public abstract bool leaveStem(CS_Stem stem);

        /**
         * passing a Leaf
         * 
         * @param leaf
         * @return when false, stop traversal at this level
         */
        public abstract bool visitLeaf(CS_Leaf leaf);
    }
}
