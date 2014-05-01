using Arbaro2.Arbaro.GUI;
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

        public ArbaroMainForm()
        {
            InitializeComponent();
        }

        private void ArbaroMainForm_Shown(object sender, EventArgs e)
        {
            pgv = new CS_ParamGroupsView(treeView1);
        }
    }
}
