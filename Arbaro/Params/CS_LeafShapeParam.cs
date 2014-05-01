using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Ported from original Arbaro software
//

namespace Arbaro2.Arbaro.Params
{
    public class CS_LeafShapeParam : CS_StringParam
    {
        static String[] items = { "disc", "disc1", "disc2", "disc3", "disc4", "disc5", "disc6", "disc7", "disc8", "disc9", "square", "sphere" };

        /**
         * @param nam
         * @param def
         * @param grp
         * @param lev
         * @param sh
         * @param lng
         */
        public CS_LeafShapeParam(String nam, String def, String grp, int lev, int ord, String sh, String lng) :
            base(nam, def, grp, lev, ord, sh, lng)
        {
        }

        public static String[] values()
        {
            return items;
        }
    }
}
