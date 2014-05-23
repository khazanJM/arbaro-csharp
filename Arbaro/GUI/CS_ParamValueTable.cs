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
            _paramValuePanel.Controls.Clear();
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
                lbl.Left = 5; lbl.Top = Y+3;
                lbl.Width = 80;
                lbl.Text = p.name;
                               
                if (p.name == "Shape")
                {
                    ComboBox shp = new ComboBox();
                    shp.Parent = _paramValuePanel;
                    shp.Left = 90;
                    shp.Width = 100;
                    shp.Top = Y;
                    shp.Items.AddRange(CS_ShapeParam.values());
                    shp.SelectedText = (p as CS_ShapeParam).toString();
                    shp.Enabled = (p as CS_AbstractParam).getEnabled();

                    shp.Tag = "Shape";
                    shp.SelectedValueChanged += shp_SelectedValueChanged;
                }
                else if (p.name == "LeafShape") 
                {
                    ComboBox shp = new ComboBox();
                    shp.Parent = _paramValuePanel;
                    shp.Left = 90;
                    shp.Width = 100;
                    shp.Top = Y;
                    shp.Items.AddRange(CS_LeafShapeParam.values());
                    shp.SelectedText = (p as CS_LeafShapeParam).toString();
                    shp.Enabled = (p as CS_AbstractParam).getEnabled();
                }
                else
                {
                    p.OnParamChanged += p_OnParamChanged;

                    TextBox tb = new TextBox();
                    tb.Parent = _paramValuePanel;
                    tb.Left = 90;
                    tb.Width = 100;
                    tb.Top = Y;
                    
                    tb.Text = p.getValue().ToString();
                    tb.Enabled = (p as CS_AbstractParam).getEnabled();
                    tb.Tag = p.name;
                    tb.Validated += tb_Validated;
                }

                Y += lbl.Height + 3;
            }

            _paramValuePanel.Height = lbl.Bottom + 5;
        }

        void p_OnParamChanged(object sender, CS_ParamChangedArgs e)
        {
            _csparams.raiseOnParamChanged("");
        }

        void tb_Validated(object sender, EventArgs e)
        {
            string pName = (string)((sender as Control).Tag);
            string pValue = (sender as TextBox).Text;
            _csparams.setParam(pName, pValue);            
        }

        void shp_SelectedValueChanged(object sender, EventArgs e)
        {
            string pName = (string)((sender as Control).Tag);
            string pValue = ((sender as ComboBox).SelectedIndex).ToString();

            _csparams.setParam(pName, pValue);
        }
    }

	
}
