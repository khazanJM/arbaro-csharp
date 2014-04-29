﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.Arbaro.Params
{
    public class CS_ParamChangedArgs : EventArgs
    {

        private string szMessage;

        public CS_ParamChangedArgs(string TextMessage)
        {
            szMessage = TextMessage;
        }

        public string Message
        {
            get { return szMessage; }
            set { szMessage = value; }
        }
    }


    public abstract class CS_AbstractParam
    {
        public static int GENERAL = -999; // no level - general params
        String name;
        String group;
        int level;
        int order;
        String shortDesc;
        String longDesc;
        bool enabled;

        public event EventHandler<CS_ParamChangedArgs> OnParamChanged;

        public CS_AbstractParam(String nam, String grp, int lev, int ord,
                String sh, String lng)
        {
            name = nam;
            group = grp;
            level = lev;
            order = ord;
            shortDesc = sh;
            longDesc = lng;
            enabled = true;
        }

        public abstract void setValue(String val);
        public abstract String getValue();
        public abstract String getDefaultValue();
        public abstract void clear();
        public abstract bool empty();

        public static bool loading = false;

        protected void warn(String warning)
        {
            //if (! loading) Console.errorOutput("WARNING: "+warning);
        }

        public void setEnabled(bool en)
        {
            enabled = en;
            raiseOnParamChanged("");
        }

        public bool getEnabled()
        {
            return enabled;
        }

        public String getName()
        {
            return name;
        }

        public String getGroup()
        {
            return group;
        }

        public int getLevel()
        {
            return level;
        }

        public int getOrder()
        {
            return order;
        }

        public String getShortDesc()
        {
            return shortDesc;
        }

        public String toString()
        {
            if (!empty())
            {
                return getValue();
            }
            // else 
            return getDefaultValue();
        }

        public String getLongDesc()
        {
            return longDesc;
        }



        protected void raiseOnParamChanged(string p)
        {
            EventHandler<CS_ParamChangedArgs> handler = OnParamChanged;

            if (handler != null)
            {
                handler(null, new CS_ParamChangedArgs(p));
            }
        }
    }
}
