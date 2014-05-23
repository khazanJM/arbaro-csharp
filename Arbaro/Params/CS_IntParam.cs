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
    public class CS_IntParam : CS_AbstractParam
    {
        private int min;
        private int max;
        private int deflt;
        private int value;

        public CS_IntParam(String nam, int mn, int mx, int def, String grp, int lev, int ord, String sh, String lng) :
            base(nam, grp, lev, ord, sh, lng)
        {
            min = mn;
            max = mx;
            deflt = def;
            value = Int32.MinValue;
        }

        public override String getDefaultValue()
        {
            int i = deflt;
            return i.ToString();
        }

        public override void clear()
        {
            value = Int32.MinValue;
            raiseOnParamChanged("");
        }

        public override void setValue(String val)
        {
            int i;
            try
            {
                i = Int32.Parse(val);
            }
            catch (Exception)
            {
                throw new Exception("Error setting value of " + name + ". \"" + val
                        + "\" isn't a valid integer.");
            }

            if (i < min)
            {
                throw new Exception("Value of " + name + " should be greater or equal to " + min);
            }

            if (i > max)
            {
                throw new Exception("Value of " + name + " should be greater or equal to " + max);
            }

            if (value != i)
            {
                value = i;
                raiseOnParamChanged("");            
            }
        }

        public override String getValue()
        {
            int i = value;
            return i.ToString();
        }

        public override bool empty()
        {
            return value == Int32.MinValue;
        }

        public int intValue()
        {
            if (empty())
            {
                warn(name + " not given, using default value(" + deflt + ")");
                // set value to default, i.e. don't warn again
                value = deflt;
                raiseOnParamChanged("");
            }
            return value;
        }

        public new String getLongDesc()
        {
            String desc = base.getLongDesc();
            desc += "<br><br>";
            if (min != Int32.MinValue)
            {
                desc += "Minimum: " + min + "\n";
            }
            if (max != Int32.MaxValue)
            {
                desc += "Maximum: " + max + "\n";
            }
            if (deflt != Int32.MinValue)
            {
                desc += "Default: " + deflt + "\n";
            }
            return desc;
        }
    }
}
