﻿using Arbaro2.Arbaro.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//
//  Ported from original Arbaro software
//

namespace Arbaro2.Arbaro.GUI
{
    public class CS_ParamValueTable
    {     
        private Panel _paramValuePanel;
        private CS_Params _csparams;
        private string _groupName;
        private int _groupLevel;
       
        public CS_ParamValueTable(Panel paramValuePanel, CS_Params csparams) 
        {
            _paramValuePanel = paramValuePanel;
            _csparams = csparams;
        }

        public void ShowGroup(string group, int level)
        {
            _groupName = group;
            _groupLevel = level;

            SortedList<int, CS_AbstractParam> par = _csparams.getParamGroup(_groupLevel, _groupName);

            _paramValuePanel.Controls.Clear();

            int Y = 5;
            Label lbl = null;

            foreach (CS_AbstractParam p in par.Values)
            {
                lbl = new Label();
                lbl.Parent = _paramValuePanel;
                lbl.Left = 5; lbl.Top = Y; Y += lbl.Height + 3;
                lbl.Text = p.name;
            }

            _paramValuePanel.Height = lbl.Bottom + 5;
        }
    }

	
}
