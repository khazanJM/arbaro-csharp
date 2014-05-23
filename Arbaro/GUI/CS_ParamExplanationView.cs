using Arbaro2.Arbaro.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.Arbaro.GUI
{
    public class CS_ParamExplanationView
    {
        private CS_Params _csparams;
        private Panel _paramExplanationPanel;

        public CS_ParamExplanationView(Panel paramExplanationPanel, Panel paramValuePanel, CS_Params csparams) 
        {
            _paramExplanationPanel = paramExplanationPanel;
            paramExplanationPanel.Tag = this;
            _csparams = csparams;
        }

        public void showExplanation(string pname)
        {          
            CS_AbstractParam p = _csparams.getParam(pname);

            WebBrowser wb = (_paramExplanationPanel.Controls[0] as WebBrowser);
            wb.DocumentText = p.getHTMLDesc();
        }
    }
}
