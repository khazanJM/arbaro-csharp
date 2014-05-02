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
        private double min;
        private double max;
        private double deflt;
        private double value;

        public CS_FloatParam(String nam, double mn, double mx, double def, String grp, int lev, int ord, String sh, String lng) :
            base(nam, grp, lev, ord, sh, lng)
        {
            min = mn;
            max = mx;
            deflt = def;
            value = Double.NaN;
        }

        public override String getDefaultValue()
        {
            Double d = deflt;
            return d.ToString();
        }

        public override void clear()
        {
            value = Double.NaN;
            raiseOnParamChanged("");
        }

        public override void setValue(String val)
        {
            double d;
            try
            {
                string uiSep = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
                val = val.Replace(".", uiSep);
                d = Double.Parse(val);
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
            value = d;
            raiseOnParamChanged("");
        }

        public override String getValue()
        {
            Double d = value;
            return d.ToString();
        }

        public override bool empty()
        {
            return Double.IsNaN(value);
        }

        public double doubleValue()
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
