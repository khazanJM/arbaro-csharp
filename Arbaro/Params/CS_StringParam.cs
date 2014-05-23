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
    public class CS_StringParam : CS_AbstractParam
    {
        private String deflt;
        private String value;

        public CS_StringParam(String nam, String def, String grp, int lev, int ord, String sh, String lng, string html)
            : base(nam, grp, lev, ord, sh, lng, html)
        {
            deflt = def;
            value = "";
        }

        public override String getDefaultValue()
        {
            return deflt;
        }

        public override void clear()
        {
            value = "";
            raiseOnParamChanged("");
        }

        public override void setValue(String val)
        {
            value = val;
            raiseOnParamChanged("");
        }

        public override bool empty()
        {
            return value == "";
        }

        public override String getValue()
        {
            if (empty())
            {
                warn(name + " not given, using default value(" + deflt + ")");
                // set value to default, t.e. don't warn again
                value = deflt;
                raiseOnParamChanged("");
            }
            return value;
        }
    }
}
