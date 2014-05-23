using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Ported from original Arbaro software
//

namespace Arbaro2.Arbaro.Params
{
    public class CS_FloatParam : CS_AbstractParam
    {
        private float min;
        private float max;
        private float deflt;
        private float value;

        public CS_FloatParam(String nam, float mn, float mx, float def, String grp, int lev, int ord, String sh, String lng) :
            base(nam, grp, lev, ord, sh, lng)
        {
            min = mn;
            max = mx;
            deflt = def;
            value = float.NaN;
        }

        public override String getDefaultValue()
        {
            float d = deflt;
            return d.ToString();
        }

        public override void clear()
        {
            value = float.NaN;
            raiseOnParamChanged("");
        }

        public override void setValue(String val)
        {
            float d;
            try
            {
                string uiSep = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
                val = val.Replace(".", uiSep);
                d = float.Parse(val);
            }
            catch (Exception)
            {
                throw new Exception("Error setting value of " + name + ". \"" + val + "\" isn't a valid number.");
            }

            if (d < min)
            {
                throw new Exception("Value of " + name + " should be greater then or equal to " + min);
            }
            if (d > max)
            {
                throw new Exception("Value of " + name + " should be less then or equal to " + max);
            }

            if (Math.Abs(value - d) > 0.00001)
            {
                value = d;
                raiseOnParamChanged("");              
            }
        }

        public override String getValue()
        {
            float d = value;
            return d.ToString();
        }

        public override bool empty()
        {
            return float.IsNaN(value);
        }

        public float doubleValue()
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

        public new String getLongDesc()
        {
            String desc = base.getLongDesc();
            desc += "<br><br>";
            if (!Double.IsNaN(min))
            {
                desc += "Minimum: " + min + "\n";
            }
            if (!Double.IsNaN(max))
            {
                desc += "Maximum: " + max + "\n";
            }
            if (!Double.IsNaN(deflt))
            {
                desc += "Default: " + deflt + "\n";
            }
            return desc;
        }
    }
}
