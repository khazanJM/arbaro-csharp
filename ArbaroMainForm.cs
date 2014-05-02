using Arbaro2.Arbaro.GUI;
using Arbaro2.Arbaro.Params;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2
{
    public partial class ArbaroMainForm : RenderForm
    {
        CS_ParamGroupsView pgv;
        CS_ParamValueTable pvt;
        CS_Params csParams = new CS_Params();

        public ArbaroMainForm()
        {
            InitializeComponent();
            Width = 1024;
            Height = 768;

            csParams.prepare(13);
        }

        private void ArbaroMainForm_Shown(object sender, EventArgs e)
        {
            pgv = new CS_ParamGroupsView(treeView1);
            pvt = new CS_ParamValueTable(paramTablePanel, csParams);
            treeView1.AfterSelect += treeView1_AfterSelect;
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null) {
                if (e.Node.Tag != null) {
                    CS_GroupNode gn = e.Node.Tag as CS_GroupNode;
                    pvt.ShowGroup(gn.getGroupName(), gn.getGroupLevel());
                }
            }
        }
    }
}
