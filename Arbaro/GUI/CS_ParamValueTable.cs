using Arbaro2.Arbaro.Params;
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
        private TreeView _tv;
        private CS_Params _csparams;
        private string _groupName;
        private int _groupLevel;

        // tv used to grab the parent panel
        // also to put table cells below the tree view
        public CS_ParamValueTable(TreeView tv, CS_Params csparams) 
        {
            _tv = tv;
            _csparams = csparams;
        }

        public void ShowGroup(string group, int level)
        {
            _groupName = group;
            _groupLevel = level;

            Dictionary<int, CS_AbstractParam> p = _csparams.getParamGroup(_groupLevel, _groupName);
            
        }
    }

	
}
