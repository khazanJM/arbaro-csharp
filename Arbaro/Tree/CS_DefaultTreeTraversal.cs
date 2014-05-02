using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  Ported from original Arbaro software

namespace Arbaro2.Arbaro.Tree
{
    public class CS_DefaultTreeTraversal : CS_TreeTraversal
    {
        public override bool enterStem(CS_Stem stem)
        {
            return true;
        }

        public override bool enterTree(CS_Tree tree)
        {
            return true;
        }

        public override bool leaveStem(CS_Stem stem)
        {
            return true;
        }

        public override bool leaveTree(CS_Tree tree)
        {
            return true;
        }

        public override bool visitLeaf(CS_Leaf leaf)
        {
            return true;
        }

    }
}
