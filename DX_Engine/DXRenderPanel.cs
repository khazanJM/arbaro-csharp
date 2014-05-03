using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//
//  Panel with "focus" because by default Panels cannot retrieve the focus
//  Courtesy of someone on stackoverflow asking the question and someone else providing a sample code
//

namespace Arbaro2.DX_Engine
{
    public class DXRenderPanel : Panel
    {
        public DXRenderPanel()
        {
            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
        }
        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down) return true;
            if (keyData == Keys.Left || keyData == Keys.Right) return true;
            return base.IsInputKey(keyData);
        }

        protected override void OnEnter(EventArgs e)
        {
            //this.Invalidate();
            base.OnEnter(e);
        }
        protected override void OnLeave(EventArgs e)
        {
            //this.Invalidate();
            base.OnLeave(e);
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);           
        }
    }
}
