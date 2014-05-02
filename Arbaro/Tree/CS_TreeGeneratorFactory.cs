using Arbaro2.Arbaro.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  Ported from original Arbaro software

namespace Arbaro2.Arbaro.Tree
{
    public class CS_TreeGeneratorFactory
    {
        static public CS_TreeGenerator createTreeGenerator()
        {
            return new CS_TreeGeneratorImpl();
        }

        static public CS_TreeGenerator createTreeGenerator(CS_Params csparams)
        {
            return new CS_TreeGeneratorImpl(csparams);
        }

        static public CS_TreeGenerator createShieldedTreeGenerator()
        {
            return new CS_ShieldedTreeGenerator(new CS_TreeGeneratorImpl());
        }

        static public CS_TreeGenerator createShieldedTreeGenerator(CS_Params csparams)
        {
            return new CS_ShieldedTreeGenerator(new CS_TreeGeneratorImpl(csparams));
        }

    }
}
