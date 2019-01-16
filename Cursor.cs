using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mapper
{
    public partial class Cursor : Control
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        public Cursor()
        {
            this.Size = new Size(25, 25);
            this.CursorRadius = 5;
            InitializeComponent();
        }

        private int _cursorRadius;
        public int CursorRadius
        {
            get { return _cursorRadius; }
            set { _cursorRadius = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            using (Pen redPen = new Pen(Color.Red, 1))
            using (Pen bluePen = new Pen(Color.Blue, 1))
            {
                pe.Graphics.DrawEllipse(redPen, new Rectangle(new Point(0, 0), new Size(this.Size.Width - 1, this.Size.Height - 1)));

                Point vLineTop = new Point(this.Size.Width / 2, 0);
                Point vLineBottom = new Point(this.Size.Width / 2, this.Size.Height);

                Point hLineLeft = new Point(0, this.Size.Height / 2);
                Point hLineRight = new Point(this.Size.Width, this.Size.Height / 2);

                pe.Graphics.DrawLine(redPen, vLineTop, vLineBottom);
                pe.Graphics.DrawLine(redPen, hLineLeft, hLineRight);

                Point innerCirclePoint = new Point((int)((this.Size.Width / 2) - (CursorRadius / 2)), (int)((this.Size.Height / 2) - (CursorRadius / 2)));
                Size innerCircleSize = new Size(CursorRadius-1, CursorRadius-1);

                pe.Graphics.DrawEllipse(bluePen, new Rectangle(innerCirclePoint, innerCircleSize));
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent) { }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            RecreateHandle();
        }
    }
}