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
    public class CS_ShapeParam : CS_IntParam
    {
        //Integer [] values;
        static String[] items = { "conical", "spherical", "hemispherical", "cylindrical", 
			"tapered cylindrical","flame","inverse conical","tend flame","envelope" };

        /**
         * @param nam
         * @param mn
         * @param mx
         * @param def
         * @param grp
         * @param lev
         * @param sh
         * @param lng
         */
        public CS_ShapeParam(String nam, int mn, int mx, int def, String grp, int lev, int ord, String sh, String lng)
            : base(nam, mn, mx, def, grp, lev, ord, sh, lng)
        { }

        public new String toString()
        {
            return items[intValue()];
        }

        public static String[] values()
        {
            return items;
        }

    }
}
