using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Foundry.Fluent;
using FloodFill2;
using System.IO;

namespace QTTest2
{
    public partial class Form2 : Form
    {
        public const int size = 2048;
        public const int circleSize = 8;
        Form1 mainForm;

        public Form2(Form1 f)
        {
            mainForm = f;
            InitializeComponent();
            pictureBox1.Image = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                g.Clear(Color.FromArgb(128, 128, 128));

            pictureBox1.Size = new Size(size + 1, size + 1);
            pictureBox1.BackColor = Color.White;
        }

        private void Decompose()
        {
            Bitmap b = (Bitmap)pictureBox1.Image;
            QuadTree q = new QuadTree(new QuadTreeNode(new RectangleF(0, 0, size, size)));

            //q.Root.DeepQuarter();
            11.Times(i => { q.Root.DeepQuarter(); label1.Text = "Deep Q " + i.ToString(); Application.DoEvents(); });

            int count = 0;
            int total = q.Flatten().Count();

            foreach (var n in q.Flatten())
            {
                count++;
                if (count % 10000 == 0)
                {
                    label1.Text = count.ToString() + " / " + total.ToString();
                    Application.DoEvents();
                }

                var colorIndex = new Dictionary<int, int>();

                // Get the most prevelant pixel color in this box.
                for (int y = 0; y < n.Box.Height; y++)
                {
                    for (int x = 0; x < n.Box.Width; x++)
                    {
                        int pix = b.GetPixel((int)(n.Box.X + x), (int)(n.Box.Y + y)).ToArgb();
                        if (colorIndex.ContainsKey(pix)) colorIndex[pix] += 1;
                        else colorIndex.Add(pix, 1);
                    }
                }

                var biggestColor = Color.FromArgb((from kvp in colorIndex
                                                   orderby kvp.Value descending
                                                   select kvp.Key).First());

                n.Cost = (byte)(255 - biggestColor.R);
            }

            label1.Text = "Compressing";
            Application.DoEvents();
            q.Compress();

            label1.Text = "Neighbor Graphing";
            Application.DoEvents();
            q.GraphNeighbors();

            label1.Text = "Drawing";
            Application.DoEvents();

            mainForm.SetQT(q);
            label1.Text = "Done";
            Application.DoEvents();

        }




        int colorVal = 255;
        bool drawing = false;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!flood)
            {
                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(255 - colorVal, 255 - colorVal, 255 - colorVal)))
                {
                    g.FillEllipse(b, e.Location.X - circleSize / 2, e.Location.Y - circleSize / 2, circleSize, circleSize);
                }
                pictureBox1.Refresh();
                drawing = true;
            }
            else
            {
                // Get the image out of the current image.
                Bitmap b = new Bitmap(size, size);
                using (Graphics gDest = Graphics.FromImage(b))
                {
                    gDest.DrawImage(pictureBox1.Image, new Point(0, 0));
                }

                QueueLinearFloodFiller filler = new QueueLinearFloodFiller(null);

                filler.Bitmap = new PictureBoxScroll.EditableBitmap(b);
                filler.FillColor = Color.FromArgb(255 - colorVal, 255 - colorVal, 255 - colorVal);
                filler.FillDiagonally = true;
                filler.Slow = false;
                filler.FloodFill(e.Location);

                Bitmap b2 = new Bitmap(size, size);
                using (Graphics gDest = Graphics.FromImage(b2))
                {
                    gDest.DrawImage(filler.Bitmap.Bitmap, new Point(0, 0));
                }

                pictureBox1.Image = b2;

                pictureBox1.Refresh();
                flood = false;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(255 - colorVal, 255 - colorVal, 255 - colorVal)))
                {
                    g.FillEllipse(b, e.Location.X - circleSize / 2, e.Location.Y - circleSize / 2, circleSize, circleSize);
                }
                pictureBox1.Refresh();
            }
        }

        private void DecomposeButton_Click(object sender, EventArgs e)
        {
            Decompose();
        }

        private void WeightTextbox_Validating(object sender, CancelEventArgs e)
        {
            int val;
            if (!int.TryParse(WeightTextbox.Text, out val))
            {
                e.Cancel = true;
                return;
            }

            if (val < 0 || val > 255)
                e.Cancel = true;
        }

        private void WeightTextbox_Validated(object sender, EventArgs e)
        {
            colorVal = int.Parse(WeightTextbox.Text);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                g.Clear(Color.FromArgb(128, 128, 128));
            pictureBox1.Refresh();
        }

        bool flood = false;

        private void FloodButton_Click(object sender, EventArgs e)
        {
            flood = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string image = Path.Combine(desktop, "South Qeynos.bmp");
            Bitmap b = (Bitmap)Bitmap.FromFile(image);
            pictureBox1.Image = new Bitmap(b);
            b.Dispose();
        }

    }
    
    public static class PointExtensions
    {
        public static PointF AsPointF(this Point p)
        {
            return new PointF(p.X, p.Y);
        }
    }
}
