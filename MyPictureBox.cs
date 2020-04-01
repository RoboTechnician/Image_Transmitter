using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ImageTransmitter
{
    class MyPictureBox : PictureBox
    {
        public Rectangle rectangle = new Rectangle();
        public Pen pen = new Pen(Color.White);

        private void DrawRec(Graphics gr)
        {
            gr.DrawRectangle(pen, rectangle);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            DrawRec(pe.Graphics);
        }
    }
}