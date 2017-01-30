using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicApp.Basic
{
    public class basicForm : Form
    {
        private const int WM_ERASEBKGND = 0x0014;

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            BasicWindow theWindow = (BasicWindow)Tag;

            Graphics theGraphics = eventArgs.Graphics;

            theWindow.DrawContent(eventArgs, theGraphics);

        }

        protected override void OnResize(EventArgs eventArgs)  // Note that OnResize is called when creating the window handle.
        {
            base.OnResize(eventArgs);

            BasicWindow theWindow = (BasicWindow)Tag;

            Graphics theGraphics = this.CreateGraphics();

            theGraphics.Clear(Color.White);

            theWindow.DrawContent(eventArgs, theGraphics);

        }


        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    break;      // Necessary to prevent flickering

                default:
                    base.WndProc(ref m);
                    break;
            }
            return;
        }


    }
    public class BasicWindow : BasicApplicationChild
    {
        public basicForm record = null;

        public virtual void CreateRecord()
        {
            record = new basicForm();
            record.Tag = this;

            record.Text = "Basic Window";
            record.ClientSize = new System.Drawing.Size((int)400.0F, (int)300.0F);
            record.DesktopLocation = new System.Drawing.Point((int)200.0F, (int)100.0F);

            record.MouseDown += OnMouseDown;
            record.MouseMove += OnMouseMove;
            record.MouseUp += OnMouseUp;

            record.Show();
        }

        public virtual void OnMouseUp(object sender, MouseEventArgs e)
        {
            return;
        }

        public virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            return;
        }

        public virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            return;
        }



        public virtual void DrawContent(EventArgs eventargs, Graphics theGraphics)
        {
            return;
        }

    }
}
