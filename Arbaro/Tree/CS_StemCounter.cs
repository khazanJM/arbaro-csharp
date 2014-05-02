using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  Ported from original Arbaro software

namespace Arbaro2.Arbaro.Tree
{
    public class CS_StemCounter : CS_DefaultTreeTraversal
    {
        private long stemCount;

        public long getStemCount()
        {
            return stemCount;
        }

        public override bool enterStem(CS_Stem stem)
        {
            stemCount++; // one more stem
            return true;
        }

        public override bool enterTree(CS_Tree tree)
        {
            stemCount = 0; // start stem counting
            return true;
        }

        public override bool visitLeaf(CS_Leaf leaf)
        {
            return false; // don't count leaves
        }
    }
}
