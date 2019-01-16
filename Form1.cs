using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Foundry.Fluent;
using System.Threading;
using System.IO;

using Foundry.Autocrat35.EverQuest2;
using System.Diagnostics;
using Foundry.Autocrat35.MathExtensions;
using System.Drawing.Drawing2D;

namespace QTTest2
{
    public partial class Form1 : Form
    {
        public const int size = 2048;
        private EQ2Interface eq2i;

        public Form1()
        {
            InitializeComponent();
            new Form2(this).Show();
        }

        QuadTree QT;
        Dictionary<int, QuadTreeNode> NodeByIndex;

        public void SetQT(QuadTree t)
        {
            QT = t;
            DrawQuadTree(QT);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(size + 1, size + 1);
            pictureBox1.BackColor = Color.White;
            ClearQT();
        }

        private void DrawQuadTree(QuadTree tree)
        {
            var root = tree.Root;
            NodeByIndex = new Dictionary<int, QuadTreeNode>();
            
            Bitmap b = new Bitmap(size, size);
            var workQueue = new Queue<QuadTreeNode>();
            
            using (Graphics g = Graphics.FromImage(b))
            using (Font labelFont = new Font(new FontFamily("Arial"), 6))
            {
                g.Clear(Color.White);
                workQueue.Enqueue(root);

                int curNodeVal = 1;
                while (workQueue.Count > 0)
                {
                    QuadTreeNode curNode = workQueue.Dequeue();

                    if (!curNode.HasChildren)
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255 - curNode.Cost, 255 - curNode.Cost, 255 - curNode.Cost)), curNode.Box);

                    Pen borderPen = null;
                    borderPen = new Pen(Brushes.Yellow, 1);

                    g.DrawRectangle(borderPen, curNode.Box);
                    borderPen.Dispose();

                    if (curNode.HasChildren) 4.Times(i => workQueue.Enqueue(curNode.Children[i]));

                    NodeByIndex.Add(curNodeVal, curNode);

                    curNodeVal++;
                }
            }

            pictureBox1.Image = b;
        }

        private void ClearQT()
        {
            QT = new QuadTree(new QuadTreeNode(new RectangleF(0, 0, size , size )));
            DrawQuadTree(QT);
        }

        private void DeepQuarterQT()
        {
            QT.Root.DeepQuarter();
            QT.ResetNeighborGraph();
            DrawQuadTree(QT);
        }

        private void QuarterNode(int val)
        {
            QuarterNode(NodeByIndex[val]);
        }
        private void QuarterNode(QuadTreeNode node)
        {
            node.Quarter();
            node.Neighbors.ForEach(n => n.ResetNeighbors());
            node.ResetNeighbors();
            DrawQuadTree(QT);
        }

        private void ShowAllBorderNodes(QuadTreeNode node)
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            using (Pen borderPen = new Pen(Brushes.Blue, 5))
            {
                Action<QuadTreeNode> draw = new Action<QuadTreeNode>(n => g.DrawRectangle(borderPen, n.Box));

                node.FindNorthernmostChildren().ForEach(draw);
                node.FindEasternmostChildren().ForEach(draw);
                node.FindSouthernmostChildren().ForEach(draw);
                node.FindWesternmostChildren().ForEach(draw);
            }

            pictureBox1.Refresh();
        }

        private void ShowNeighborsForNode(int val)
        {
            QuadTreeNode node = NodeByIndex[val];
            
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            using (Pen borderPen = new Pen(Brushes.Blue, 5))
            {
                Action<QuadTreeNode> draw = new Action<QuadTreeNode>(n => g.DrawRectangle(borderPen, n.Box));
                node.FindNeighbors(NeighborDirection.North).ForEach(draw);
            }

            pictureBox1.Refresh();
        }

        private void DrawNeighbors(QuadTree tree)
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            using (Pen linePen = new Pen(Brushes.Blue, 2))
            {
                var draw = new Action<QuadTreeNode, QuadTreeNode>((n1, n2) => g.DrawLine(linePen, n1.Box.Midpoint(), n2.Box.Midpoint()));

                tree.Flatten().Where(node => node.Cost != 255).
                    ForEach(node => node.Neighbors.Where(n => n.Cost != 255).
                        ForEach(n => draw(node, n))
                    );
            }

            pictureBox1.Refresh();
        }

        private void SetNodeCost(QuadTreeNode node, byte val)
        {
            node.Cost = val;
            node.Neighbors.ForEach(n => n.ResetNeighbors());
            node.ResetNeighbors();
            DrawQuadTree(QT);
        }

        private void RunAStarTest(QuadTree tree)
        {
            foundPath = new List<QuadTreeNode>();
            AStar astar = new AStar();

            statusLabel.Text = "Running A* pathfinder";
            Application.DoEvents();

            float hMod = float.Parse(HModTextbox.Text);

            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            using (Pen bluelinePen = new Pen(Brushes.LightBlue, 5))
            using (Pen redlinePen = new Pen(Brushes.DarkViolet, 5))
            {
                for (int i = 0; i < aStarNodes.Count - 1; i++)
                {
                    var path = astar.FindPath(hMod, aStarNodes[i], aStarNodes[i + 1], ShowAStarCheckbox.Checked ? new Action<IEnumerable<QuadTreeNode>, IEnumerable<QuadTreeNode>, int>(AStarCallback) : null);

                    if (path.Count == 0)
                    {
                        statusLabel.Text = "No valid path could be found.";
                        return;
                    }

                    statusLabel.Text = "Drawing Path";
                    Application.DoEvents();

                    LinkedListNode<QuadTreeNode> current = path.First;
                    while (current != null && current.Next != null)
                    {
                        g.DrawLine(bluelinePen, current.Value.Box.Midpoint(), current.Next.Value.Box.Midpoint());
                        current = current.Next;
                    }
                    pictureBox1.Refresh();

                    statusLabel.Text = "Initial Smoothing:";
                    Application.DoEvents();

                    QuadTreeNode lastNode = null;
                    var nodes = astar.InitialSmooth(path, QT);
                    foreach (var node in nodes)
                    {
                        foundPath.Add(node);
                        if (lastNode != null)
                            g.DrawLine(redlinePen, lastNode.Box.Midpoint(), node.Box.Midpoint());
                        lastNode = node;
                        Application.DoEvents();
                        pictureBox1.Refresh();
                    }
                }
            }

            statusLabel.Text = "Done...";
            Application.DoEvents();
        }

        List<QuadTreeNode> foundPath;

        private void SmoothCallback(LineF line, IList<QuadTreeNode> lineNodes, QuadTreeNode curNode, QuadTreeNode testNode)
        {
            return;
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            using (Pen linePen = new Pen(Color.HotPink, 2))
            using (Pen curPen = new Pen(Color.Blue, 5))
            using (Pen testPen = new Pen(Color.Red, 5))
            using (Pen boxPen = new Pen(Color.Green, 5))
            {
                lineNodes.ForEach(n => g.DrawRectangle(boxPen, n.Box));
                g.DrawRectangle(curPen, curNode.Box);
                g.DrawRectangle(testPen, testNode.Box);
                g.DrawLine(linePen, line.StartPos, line.EndPos);
            }

            pictureBox1.Refresh();

            Application.DoEvents();
        }

        private void AStarCallback(IEnumerable<QuadTreeNode> open, IEnumerable<QuadTreeNode> closed, int steps)
        {
            //return;
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            using (Pen openPen = new Pen(Color.Red,2))
            {
                foreach (var node in open)
                {
                    if (node.AStarParent != null)
                        g.DrawLine(openPen, node.Box.Midpoint(), node.AStarParent.Box.Midpoint());
                }
            }

            //statusLabel.Text = steps.ToString();

            pictureBox1.Refresh();

            Application.DoEvents();
        }


        private void ClearQTButton_Click(object sender, EventArgs e)
        {
            ClearQT();
        }

        private void DeepQuarterQTButton_Click(object sender, EventArgs e)
        {
            DeepQuarterQT();
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            RunAStarTest(QT);
        }

        private void RefreshQTButton_Click(object sender, EventArgs e)
        {
            DrawQuadTree(QT);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // Check to ensure click loc is in the bounds of the tree
            if (!QT.Root.Box.Contains(e.Location)) return;

            // Find the leaf node that contains the click location by walking down the branch nodes that contain the
            // click location.
            QuadTreeNode node = QT.Root;
            while (node.HasChildren)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (node.Children[i].Box.Contains(e.Location))
                    {
                        node = node.Children[i];
                        break;
                    }
                }
            }

            if (node == null) return;

            if (e.Button == MouseButtons.Right)
            {
                return;
                byte val;
                if (!byte.TryParse(NodeToQuarterTextbox.Text, out val))
                    MessageBox.Show("Please enter a numeric value between 0 and 255.");
                else if (val < 0 || val > 255)
                    MessageBox.Show("Please enter a numeric value between 0 and 255.");
                else
                    SetNodeCost(node, val);
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (SettingUpNodes)
                {
                    aStarNodes.Add(node);

                    using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                    using (Brush blk = new SolidBrush(Color.Black))
                    using (Brush grn = new SolidBrush(Color.Lime))
                    using (Pen bp = new Pen(Color.Black))
                    using (Font f = new Font("Arial",5))
                    {
                        g.FillRectangle(grn, node.Box);
                        g.DrawRectangle(bp, node.Box);
                        g.DrawString(aStarNodes.Count.ToString(), f, blk, node.Box.Midpoint());
                    }
                    pictureBox1.Refresh();
                    //DrawQuadTree(QT);
                }
                else
                {
                    return;
                    QuarterNode(node);
                }
            }
        }

        bool SettingUpNodes = false;
        List<QuadTreeNode> aStarNodes;

        private void SetupAStarNodesButton_Click(object sender, EventArgs e)
        {
            if (!SettingUpNodes) { SetupAStarNodesButton.Text = "Done Setting Up Nodes"; aStarNodes = new List<QuadTreeNode>(); }
            else SetupAStarNodesButton.Text = "Setup A* Nodes";

            SettingUpNodes = !SettingUpNodes;
            DoSettingUpNodes();
        }

        public void DoSettingUpNodes()
        {
            if (!SettingUpNodes)
            {
                statusLabel.Text = "";
                return;
            }

            statusLabel.Text = "Select the Start node.";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            QT.Save("c:\\test.qt");
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "sq.qt");
            statusLabel.Text = "Loading South Qeynos map";
            Application.DoEvents();
            QT = QuadTree.Load(path);

            statusLabel.Text = "Updating Neighbor Graph";
            Application.DoEvents();
            QT.GraphNeighbors();

            statusLabel.Text = "Refreshing View";
            Application.DoEvents();
            DrawQuadTree(QT);

            statusLabel.Text = "Ready...";
        }

        private void WalkButton_Click(object sender, EventArgs e)
        {
            Process EQ2Process = Process.GetProcessesByName("EverQuest2").FirstOrDefault();
            if (EQ2Process == null)
            {
                MessageBox.Show("Please run EverQuest II before trying to walk a path.");
                return;
            }

            EQ2Interface eq2i = new EQ2Interface(EQ2Process);
            if (eq2i.ZoneName != "South Qeynos")
            {
                MessageBox.Show("This test only functions in South Qeynos.");
                return;
            }

            if (foundPath == null)
            {
                MessageBox.Show("Please create a path first by defining the start and end points and clicking Find Path");
                return;
            }

            List<PointF> points = new List<PointF>();
            foreach (var waynode in foundPath)
            {
                points.Add(waynode.Box.Midpoint());
            }

            GraphicsPath gPath = new GraphicsPath();
            gPath.AddCurve(points.ToArray(), 0.0f);
            gPath.Flatten(new Matrix(), 2.5f);

            foreach (var waypoint in gPath.PathPoints)
            {
                PointF curDest = new PointF((waypoint.X / 4) + 330, (waypoint.Y / 4) + 25);
                statusLabel.Text = "Walking To: " + curDest.ToString();
                Application.DoEvents();
                WalkTo(eq2i, curDest);
            }

            gPath.Dispose();

            eq2i.AutoRun = false;
        }

        private void WalkTo(EQ2Interface eq2i, PointF dest)
        {
            WalkButton.Enabled = false;

            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            using (Brush b = new SolidBrush(Color.LimeGreen))
            {
                do
                {
                    if (dest.DistanceTo(eq2i.CharXY) <= 1.5) break;
                    eq2i.CharHeading = eq2i.CharXY.HeadingTo(dest);
                    eq2i.CamYaw = eq2i.CharXY.HeadingTo(dest);
                    eq2i.AutoRun = true;

                    PointF destG = new PointF((dest.X - 330) * 4, (dest.Y - 25) * 4);
                    g.FillCircle(b, destG, 5);

                    pictureBox1.Refresh();

                    Application.DoEvents();
                    Thread.Sleep(10);
                } while (true);
            }

            WalkButton.Enabled = true;
        }
    }

    public static class GraphicsHelpers
    {
        public static void DrawRectangle(this Graphics g, Pen pen, RectangleF rect)
        {
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static PointF Midpoint(this RectangleF box)
        {
            return new PointF(box.X + box.Width / 2, box.Y + box.Height / 2);
        }

        public static void DrawCircle(this Graphics g, Pen p, PointF center, float radius)
        {
            RectangleF r = new RectangleF(new PointF(center.X - radius, center.Y - radius), new SizeF(radius * 2, radius * 2));
            g.DrawEllipse(p, r);
        }

        public static void FillCircle(this Graphics g, Brush b, PointF center, float radius)
        {
            RectangleF r = new RectangleF(new PointF(center.X - radius, center.Y - radius), new SizeF(radius * 2, radius * 2));
            g.FillEllipse(b, r);
        }
    }
}
