using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.Arbaro.Tree
{
    public class CS_LeafCounter : CS_DefaultTreeTraversal
    {
        private long leafCount;

        public long getLeafCount()
        {
            return leafCount;
        }

        public override bool enterStem(CS_Stem stem)
        {
            // add leaves of this stem
            leafCount += stem.getLeafCount();
            return true;
        }

        public override bool enterTree(CS_Tree tree)
        {
            leafCount = 0; // start counting leaves
            return true;
        }

        public override bool visitLeaf(CS_Leaf leaf)
        {
            return false; // don't visit more leaves
            // for efficency leaf count was got from the parent stem 
        }

    }
}
