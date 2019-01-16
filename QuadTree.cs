using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using Foundry.Fluent;
using System.IO;

namespace QTTest2
{
    public enum NeighborDirection
    {
        North, East, South, West, All
    }

    public enum NodeOrientation
    {
        NorthWest = 0, NorthEast = 1,
        SouthWest = 2, SouthEast = 3
    }

    public class QuadTree
    {
        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(Root.Box.X);
                bw.Write(Root.Box.Y);
                bw.Write(Root.Box.Width);
                bw.Write(Root.Box.Height);
                Root.Save(bw);
            }
        }

        public static QuadTree Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                RectangleF box = new RectangleF(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                QuadTreeNode root = QuadTreeNode.Load(box, br);
                return new QuadTree(root);
            }
        }

        public QuadTreeNode Root { get; private set; }

        public QuadTree(QuadTreeNode root)
        {
            Root = root;

            Stack<QuadTreeNode> workStack = new Stack<QuadTreeNode>();
            workStack.Push(root);

            while (workStack.Count != 0)
            {
                QuadTreeNode node = workStack.Pop();
                node.Tree = this;
                if (node.HasChildren)
                    4.Times(i => workStack.Push(node.Children[i]));
            }
        }

        public IEnumerable<QuadTreeNode> Flatten()
        {
            return Flatten(Root);
        }
        public IEnumerable<QuadTreeNode> Flatten(QuadTreeNode root)
        {
            var workStack = new Stack<QuadTreeNode>();

            workStack.Push(root);

            while (workStack.Count > 0)
            {
                QuadTreeNode curNode = workStack.Pop();
                if (curNode.HasChildren) 4.Times(i => workStack.Push(curNode.Children[3 - i]));
                else yield return curNode;
            }
        }

        public void ResetNeighborGraph()
        {
            var workStack = new Stack<QuadTreeNode>();

            workStack.Push(Root);

            while (workStack.Count > 0)
            {
                QuadTreeNode curNode = workStack.Pop();
                if (curNode.HasChildren) 4.Times(i => workStack.Push(curNode.Children[i]));
                curNode.ResetNeighbors();
            }
        }

        public void Compress()
        {
            Compress(Root);
            ResetNeighborGraph();
        }

        private void Compress(QuadTreeNode node)
        {
            if (node.HasChildren)
            {
                bool goAhead = true;
                4.Times(i => { if (node.Children[i].HasChildren) Compress(node.Children[i]); });
                4.Times(i => { if (node.Children[i].HasChildren) goAhead = false; });

                if (!goAhead) return;

                bool sameVal = node.Children[0].Cost == node.Children[1].Cost &&
                               node.Children[0].Cost == node.Children[2].Cost &&
                               node.Children[0].Cost == node.Children[3].Cost;

                if (sameVal)
                {
                    node.Cost = node.Children[0].Cost;
                    node.Children.Clear();
                    node.HasChildren = false;
                }
            }
        }

        public IEnumerable<QuadTreeNode> IntersectingNodes(LineF line)
        {
            return Flatten().Where(n => DoesLineIntersectRect(line, n.Box));
        }

        public static bool DoLinesIntersect(LineF firstLine, LineF secondLine)
        {
            double Ua, Ub, UaN, UaD, UbN, UbD;

            UaN = ((secondLine.EndPos.X - secondLine.StartPos.X) * (firstLine.StartPos.Y - secondLine.StartPos.Y) - (secondLine.EndPos.Y - secondLine.StartPos.Y) * (firstLine.StartPos.X - secondLine.StartPos.X));
            UaD = ((secondLine.EndPos.Y - secondLine.StartPos.Y) * (firstLine.EndPos.X - firstLine.StartPos.X) - (secondLine.EndPos.X - secondLine.StartPos.X) * (firstLine.EndPos.Y - firstLine.StartPos.Y));
            Ua = UaN / UaD;

            UbN = ((firstLine.EndPos.X - firstLine.StartPos.X) * (firstLine.StartPos.Y - secondLine.StartPos.Y) - (firstLine.EndPos.Y - firstLine.StartPos.Y) * (firstLine.StartPos.X - secondLine.StartPos.X));
            UbD = ((secondLine.EndPos.Y - secondLine.StartPos.Y) * (firstLine.EndPos.X - firstLine.StartPos.X) - (secondLine.EndPos.X - secondLine.StartPos.X) * (firstLine.EndPos.Y - firstLine.StartPos.Y));
            Ub = UbN / UbD;

            if (UaN == 0 && UaD == 0 && UbN == 0 && UbD == 0)
            {
                //return true;
                if (firstLine.StartPos.Y == firstLine.EndPos.Y)
                {
                    // Horizonal line
                    // Check if the points of one line are contained in the other
                    return (
                        (firstLine.StartPos.X > secondLine.StartPos.X &&
                        firstLine.StartPos.X < secondLine.EndPos.X) ||
                        (firstLine.EndPos.X > secondLine.StartPos.X &&
                        firstLine.EndPos.X < secondLine.EndPos.X) ||

                        (secondLine.StartPos.X > firstLine.StartPos.X &&
                        secondLine.StartPos.X < firstLine.EndPos.X) ||
                        (secondLine.EndPos.X > firstLine.StartPos.X &&
                        secondLine.EndPos.X < firstLine.EndPos.X)
                    );
                }
                else if (firstLine.StartPos.X == firstLine.EndPos.X)
                {
                    // Vertical line
                    // Check if the points of one line are contained in the other
                    return (
                        (firstLine.StartPos.Y > secondLine.StartPos.Y &&
                        firstLine.StartPos.Y < secondLine.EndPos.Y) ||
                        (firstLine.EndPos.Y > secondLine.StartPos.Y &&
                        firstLine.EndPos.Y < secondLine.EndPos.Y) ||

                        (secondLine.StartPos.Y > firstLine.StartPos.Y &&
                        secondLine.StartPos.Y < firstLine.EndPos.Y) ||
                        (secondLine.EndPos.Y > firstLine.StartPos.Y &&
                        secondLine.EndPos.Y < firstLine.EndPos.Y)
                    );
                }
                else
                    return true; // Error state - just assume true, we'll see what happens.
            }

            return (Ua >= 0.0f && Ua <= 1.0f && Ub >= 0.0f && Ub <= 1.0f);
        }

        public static bool DoesLineIntersectRect(LineF line, RectangleF rect)
        {
            if (rect.Contains(line.StartPos) || rect.Contains(line.EndPos)) return true;

            LineF test = new LineF
            {
                StartPos = new PointF(rect.X, rect.Y),
                EndPos = new PointF(rect.X + rect.Width, rect.Y)
            };
            if (DoLinesIntersect(line, test)) return true;

            test = new LineF
            {
                StartPos = new PointF(rect.X + rect.Width, rect.Y),
                EndPos = new PointF(rect.X + rect.Width, rect.Y + rect.Height)
            };
            if (DoLinesIntersect(line, test)) return true;

            test = new LineF
            {
                StartPos = new PointF(rect.X, rect.Y + rect.Height),
                EndPos = new PointF(rect.X + rect.Width, rect.Y + rect.Height)
            };
            if (DoLinesIntersect(line, test)) return true;

            return false;
        }

        public void GraphNeighbors()
        {
            Flatten().ForEach(n => n.Neighbors.Count());
        }

    }

    public struct LineF
    {
        public PointF StartPos;
        public PointF EndPos;
    }

    public class QuadTreeNode : IComparable<QuadTreeNode>
    {
        #region Constructors
        /// <summary>
        /// Creates a new QuadTree node
        /// </summary>
        /// <param name="box">The bounding box this node represents</param>
        public QuadTreeNode(RectangleF box)
        {
            Box = box;
            _children = new QuadTreeNode[4];
        }

        /// <summary>
        /// Creates a new QuadTree node
        /// </summary>
        /// <param name="box">The bounding box this node represents</param>
        /// <param name="parent">This QuadTreeNode's parent</param>
        public QuadTreeNode(RectangleF box, QuadTreeNode parent, NodeOrientation orientation)
            : this(box)
        {
            Parent = parent;
            Depth = parent.Depth + 1;
            Tree = parent.Tree;
            Orientation = orientation;
            Cost = parent.Cost;
        }
        #endregion

        #region Properties
        public NodeOrientation Orientation { get; private set; }

        #region AStar Properties
        public long AStarGCost { get; set; }
        public long AStarHCost { get; set; }
        public long AStarFCost { get { return AStarGCost + AStarHCost; } }
        public QuadTreeNode AStarParent { get; set; }
        public bool AStarIsClosed { get; set; }
        public bool AStarIsOpen { get; set; }
        #endregion

        /// <summary>
        /// Gets or Sets the transit cost for this node - used by the pathfinder.
        /// </summary>
        public byte Cost { get; set; }

        /// <summary>
        /// The tree this QuadTreeNode belongs to.
        /// </summary>
        public QuadTree Tree { get; set; }

        /// <summary>
        /// The number of levels deep this Node is.
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Whether this Node has child nodes or not.
        /// </summary>
        public bool HasChildren { get; internal set; }

        /// <summary>
        /// This Node's bounding box.
        /// </summary>
        public RectangleF Box { get; private set; }

        /// <summary>
        /// This Node's parent, or Null if this is the root node for this tree.
        /// </summary>
        public QuadTreeNode Parent { get; private set; }

        /// <summary>
        /// This Node's children, or Null if there are no children.
        /// </summary>
        public IList<QuadTreeNode> Children
        {
            get
            {
                if (!HasChildren) return null;
                return (IList<QuadTreeNode>)_children.ToList();
            }
        } private QuadTreeNode[] _children;

        /// <summary>
        /// This node's NorthWestern child
        /// </summary>
        public QuadTreeNode NWChild { get { if (!HasChildren) return null; else return _children[0]; } }

        /// <summary>
        /// This node's NorthEastern child
        /// </summary>
        public QuadTreeNode NEChild { get { if (!HasChildren) return null; else return _children[1]; } }

        /// <summary>
        /// This node's SouthWestern child
        /// </summary>
        public QuadTreeNode SWChild { get { if (!HasChildren) return null; else return _children[2]; } }

        /// <summary>
        /// This node's SouthEastern child
        /// </summary>
        public QuadTreeNode SEChild { get { if (!HasChildren) return null; else return _children[3]; } }

        /// <summary>
        /// This node's neighbors (cached - call UpdateNeighbors if this node's neighbors might change).
        /// </summary>
        public IEnumerable<QuadTreeNode> Neighbors
        {
            get
            {
                if (_neighbors == null)
                    _neighbors = FindNeighbors(NeighborDirection.All);

                return _neighbors;
            }
        } private IList<QuadTreeNode> _neighbors;

        #endregion

        #region Methods

        #region Quartering

        public void Quarter()
        {
            if (HasChildren)
                throw new Exception("Can't Quarter a node with children - call DeepQuarter instead");

            // Create 4 quadrant boxes based on this parent box.
            float boxHalfWidth = Box.Width / 2;
            float boxHalfHeight = Box.Height / 2;
            float boxMidX = Box.X + boxHalfWidth;
            float boxMidY = Box.Y + boxHalfHeight;

            RectangleF[] boxes = new RectangleF[4];

            boxes[0] = new RectangleF(Box.X, Box.Y, boxHalfWidth, boxHalfHeight); // NW
            boxes[1] = new RectangleF(Box.X + boxHalfWidth, Box.Y, boxHalfWidth, boxHalfHeight); // NE
            boxes[2] = new RectangleF(Box.X, Box.Y + boxHalfHeight, boxHalfWidth, boxHalfHeight); // SW
            boxes[3] = new RectangleF(Box.X + boxHalfWidth, Box.Y + boxHalfHeight, boxHalfWidth, boxHalfHeight); // SE

            // Create 4 new QT nodes, giving them this as parent and their appropriate box
            4.Times(i => _children[i] = new QuadTreeNode(boxes[i], this, (NodeOrientation)i));

            HasChildren = true;
        }

        public void DeepQuarter()
        {
            if (!HasChildren)
            {
                Quarter();
                return;
            }

            4.Times(i => _children[i].DeepQuarter());
        }

        #endregion

        #region Neighbor Finding

        /// <summary>
        /// Flags this node's Neighbors property as dirty and causes it to reconstruct
        /// the neighbors cache on the next request.
        /// </summary>
        public void ResetNeighbors()
        {
            _neighbors = null;
        }

        /// <summary>
        /// Finds this Node's neighbors in the specified direction.
        /// </summary>
        /// <param name="direction">The direction for which to find neighbors</param>
        /// <returns>A list of neighboring QuadTreeNodes</returns>
        public IList<QuadTreeNode> FindNeighbors(NeighborDirection direction)
        {
            // Check parent for null
            if (Parent == null) return new List<QuadTreeNode>();

            switch (direction)
            {
                case NeighborDirection.North: return FindNorthNeighbors();
                case NeighborDirection.East: return FindEastNeighbors();
                case NeighborDirection.South: return FindSouthNeighbors();
                case NeighborDirection.West: return FindWestNeighbors();
                case NeighborDirection.All:
                    List<QuadTreeNode> r = new List<QuadTreeNode>();
                    r.AddRange(FindNorthNeighbors());
                    r.AddRange(FindSouthNeighbors());
                    r.AddRange(FindEastNeighbors());
                    r.AddRange(FindWestNeighbors());
                    return r;
            }
            return null;
        }

        private IList<QuadTreeNode> FindWestNeighbors()
        {
            List<QuadTreeNode> neighbors = new List<QuadTreeNode>();

            // Easy case - I'm an eastern sibling, so just get my western sibling's easternmost children.
            QuadTreeNode westNeighborOfEqualDepth = null;
            if (Orientation == NodeOrientation.NorthEast) westNeighborOfEqualDepth = Parent.NWChild;
            if (Orientation == NodeOrientation.SouthEast) westNeighborOfEqualDepth = Parent.SWChild;

            // Test for easy case
            if (westNeighborOfEqualDepth != null)
            {
                if (!westNeighborOfEqualDepth.HasChildren)
                {
                    neighbors.Add(westNeighborOfEqualDepth);
                    return neighbors;
                }
                else
                {
                    neighbors.AddRange(westNeighborOfEqualDepth.FindEasternmostChildren());
                    return neighbors;
                }
            }

            // We're not an eastern sibling - we need to refer to our parent's sibling for this to work.
            // The basic idea here is to step up in the tree, remembering our steps, until we land on a
            // node that has an easy West sibling - that means it'll be either a NE or SE node in relation to
            // its parent. Then, we take our list of steps and step back (we'll use a stack), then using a
            // 'mirror' to flip the horizontal values, we step in through the history until we either run out
            // of history (at which point we return either the found node or its Easternmost children) or
            // we run out of children, at which point we return that last node.
            // If at any point while we're stepping back we hit the root node, we need to return the empty list
            // - this means we're on the border of our tree, and thus have no nodes to our West.

            // Simple really.

            var orientationHistory = new Stack<NodeOrientation>();
            QuadTreeNode curNode = this;
            while (true)
            {
                // Push the current node's orientation onto the history stack.
                orientationHistory.Push(curNode.Orientation);

                // Check to see if the current node is the root - if it is, we just return the empty neighbors list.
                if (curNode == Tree.Root)
                    return neighbors;

                // Check to see if the current node's orientation is eastern - if it is, we can start stepping back.
                if (curNode.Orientation == NodeOrientation.NorthEast || curNode.Orientation == NodeOrientation.SouthEast)
                    break;

                // Set the next node to its parent.
                curNode = curNode.Parent;
            }

            // We're at an Eastern node - we need to select its parent before we can start mirroring.
            curNode = curNode.Parent;

            // Sanity check:
            if (curNode == null) return neighbors;

            // Now we begin to plumb the mirrored depths of the history stack...
            foreach (var historyEntry in orientationHistory)
            {
                // If this node has no children, we need to return just this node.
                if (!curNode.HasChildren)
                {
                    neighbors.Add(curNode);
                    return neighbors;
                }

                // Flip the historyEntry horizontally
                NodeOrientation flippedHistory = default(NodeOrientation);
                if (historyEntry == NodeOrientation.NorthEast) flippedHistory = NodeOrientation.NorthWest;
                if (historyEntry == NodeOrientation.SouthEast) flippedHistory = NodeOrientation.SouthWest;
                if (historyEntry == NodeOrientation.NorthWest) flippedHistory = NodeOrientation.NorthEast;
                if (historyEntry == NodeOrientation.SouthWest) flippedHistory = NodeOrientation.SouthEast;

                // Get the appropriate node
                var nextNode = curNode.Children[(int)flippedHistory];

                // Let's keep going!
                curNode = nextNode;
            }

            // curNode should now hold a reference to our neighbor cell. If it has children, we need to add all its
            // deep Easternmost children to the neighbors collection; otherwise, we just return this node.

            if (!curNode.HasChildren)
            {
                neighbors.Add(curNode);
                return neighbors;
            }

            neighbors.AddRange(curNode.FindEasternmostChildren());
            return neighbors;
        }

        private IList<QuadTreeNode> FindSouthNeighbors()
        {
            List<QuadTreeNode> neighbors = new List<QuadTreeNode>();

            QuadTreeNode southNeighborOfEqualDepth = null;
            if (Orientation == NodeOrientation.NorthEast) southNeighborOfEqualDepth = Parent.SEChild;
            if (Orientation == NodeOrientation.NorthWest) southNeighborOfEqualDepth = Parent.SWChild;

            if (southNeighborOfEqualDepth != null)
            {
                if (!southNeighborOfEqualDepth.HasChildren)
                {
                    neighbors.Add(southNeighborOfEqualDepth);
                    return neighbors;
                }
                else
                {
                    neighbors.AddRange(southNeighborOfEqualDepth.FindNorthernmostChildren());
                    return neighbors;
                }
            }

            var orientationHistory = new Stack<NodeOrientation>();
            QuadTreeNode curNode = this;
            while (true)
            {
                orientationHistory.Push(curNode.Orientation);

                if (curNode == Tree.Root)
                    return neighbors;

                if (curNode.Orientation == NodeOrientation.NorthEast || curNode.Orientation == NodeOrientation.NorthWest)
                    break;

                curNode = curNode.Parent;
            }

            curNode = curNode.Parent;
            if (curNode == null) return neighbors;

            foreach (var historyEntry in orientationHistory)
            {
                if (!curNode.HasChildren)
                {
                    neighbors.Add(curNode);
                    return neighbors;
                }

                NodeOrientation flippedHistory = default(NodeOrientation);
                if (historyEntry == NodeOrientation.NorthEast) flippedHistory = NodeOrientation.SouthEast;
                if (historyEntry == NodeOrientation.SouthEast) flippedHistory = NodeOrientation.NorthEast;
                if (historyEntry == NodeOrientation.NorthWest) flippedHistory = NodeOrientation.SouthWest;
                if (historyEntry == NodeOrientation.SouthWest) flippedHistory = NodeOrientation.NorthWest;

                var nextNode = curNode.Children[(int)flippedHistory];

                curNode = nextNode;
            }

            if (!curNode.HasChildren)
            {
                neighbors.Add(curNode);
                return neighbors;
            }

            neighbors.AddRange(curNode.FindNorthernmostChildren());
            return neighbors;
        }

        private IList<QuadTreeNode> FindEastNeighbors()
        {
            List<QuadTreeNode> neighbors = new List<QuadTreeNode>();

            QuadTreeNode eastNeighborOfEqualDepth = null;
            if (Orientation == NodeOrientation.NorthWest) eastNeighborOfEqualDepth = Parent.NEChild;
            if (Orientation == NodeOrientation.SouthWest) eastNeighborOfEqualDepth = Parent.SEChild;

            if (eastNeighborOfEqualDepth != null)
            {
                if (!eastNeighborOfEqualDepth.HasChildren)
                {
                    neighbors.Add(eastNeighborOfEqualDepth);
                    return neighbors;
                }
                else
                {
                    neighbors.AddRange(eastNeighborOfEqualDepth.FindWesternmostChildren());
                    return neighbors;
                }
            }

            var orientationHistory = new Stack<NodeOrientation>();
            QuadTreeNode curNode = this;
            while (true)
            {
                orientationHistory.Push(curNode.Orientation);

                if (curNode == Tree.Root)
                    return neighbors;

                if (curNode.Orientation == NodeOrientation.NorthWest || curNode.Orientation == NodeOrientation.SouthWest)
                    break;

                curNode = curNode.Parent;
            }

            curNode = curNode.Parent;
            if (curNode == null) return neighbors;

            foreach (var historyEntry in orientationHistory)
            {
                if (!curNode.HasChildren)
                {
                    neighbors.Add(curNode);
                    return neighbors;
                }

                NodeOrientation flippedHistory = default(NodeOrientation);
                if (historyEntry == NodeOrientation.NorthWest) flippedHistory = NodeOrientation.NorthEast;
                if (historyEntry == NodeOrientation.SouthWest) flippedHistory = NodeOrientation.SouthEast;
                if (historyEntry == NodeOrientation.NorthEast) flippedHistory = NodeOrientation.NorthWest;
                if (historyEntry == NodeOrientation.SouthEast) flippedHistory = NodeOrientation.SouthWest;

                var nextNode = curNode.Children[(int)flippedHistory];

                curNode = nextNode;
            }

            if (!curNode.HasChildren)
            {
                neighbors.Add(curNode);
                return neighbors;
            }

            neighbors.AddRange(curNode.FindWesternmostChildren());
            return neighbors;
        }

        private IList<QuadTreeNode> FindNorthNeighbors()
        {
            List<QuadTreeNode> neighbors = new List<QuadTreeNode>();

            QuadTreeNode northNeighborOfEqualDepth = null;
            if (Orientation == NodeOrientation.SouthEast) northNeighborOfEqualDepth = Parent.NEChild;
            if (Orientation == NodeOrientation.SouthWest) northNeighborOfEqualDepth = Parent.NWChild;

            if (northNeighborOfEqualDepth != null)
            {
                if (!northNeighborOfEqualDepth.HasChildren)
                {
                    neighbors.Add(northNeighborOfEqualDepth);
                    return neighbors;
                }
                else
                {
                    neighbors.AddRange(northNeighborOfEqualDepth.FindSouthernmostChildren());
                    return neighbors;
                }
            }

            var orientationHistory = new Stack<NodeOrientation>();
            QuadTreeNode curNode = this;
            while (true)
            {
                orientationHistory.Push(curNode.Orientation);

                if (curNode == Tree.Root)
                    return neighbors;

                if (curNode.Orientation == NodeOrientation.SouthEast || curNode.Orientation == NodeOrientation.SouthWest)
                    break;

                curNode = curNode.Parent;
            }

            curNode = curNode.Parent;
            if (curNode == null) return neighbors;

            foreach (var historyEntry in orientationHistory)
            {
                if (!curNode.HasChildren)
                {
                    neighbors.Add(curNode);
                    return neighbors;
                }

                NodeOrientation flippedHistory = default(NodeOrientation);
                if (historyEntry == NodeOrientation.SouthEast) flippedHistory = NodeOrientation.NorthEast;
                if (historyEntry == NodeOrientation.NorthEast) flippedHistory = NodeOrientation.SouthEast;
                if (historyEntry == NodeOrientation.SouthWest) flippedHistory = NodeOrientation.NorthWest;
                if (historyEntry == NodeOrientation.NorthWest) flippedHistory = NodeOrientation.SouthWest;

                var nextNode = curNode.Children[(int)flippedHistory];

                curNode = nextNode;
            }

            if (!curNode.HasChildren)
            {
                neighbors.Add(curNode);
                return neighbors;
            }

            neighbors.AddRange(curNode.FindSouthernmostChildren());
            return neighbors;
        }

        #endregion

        #region Deep (Recursive) searches for border children

        public IList<QuadTreeNode> FindEasternmostChildren()
        {
            List<QuadTreeNode> children = new List<QuadTreeNode>();

            if (!NEChild.HasChildren) children.Add(NEChild);
            else children.AddRange(NEChild.FindEasternmostChildren());

            if (!SEChild.HasChildren) children.Add(SEChild);
            else children.AddRange(SEChild.FindEasternmostChildren());

            return children;
        }

        public IList<QuadTreeNode> FindNorthernmostChildren()
        {
            List<QuadTreeNode> children = new List<QuadTreeNode>();

            if (!NEChild.HasChildren) children.Add(NEChild);
            else children.AddRange(NEChild.FindNorthernmostChildren());

            if (!NWChild.HasChildren) children.Add(NWChild);
            else children.AddRange(NWChild.FindNorthernmostChildren());

            return children;
        }

        public IList<QuadTreeNode> FindWesternmostChildren()
        {
            List<QuadTreeNode> children = new List<QuadTreeNode>();

            if (!NWChild.HasChildren) children.Add(NWChild);
            else children.AddRange(NWChild.FindWesternmostChildren());

            if (!SWChild.HasChildren) children.Add(SWChild);
            else children.AddRange(SWChild.FindWesternmostChildren());

            return children;
        }

        public IList<QuadTreeNode> FindSouthernmostChildren()
        {
            List<QuadTreeNode> children = new List<QuadTreeNode>();

            if (!SEChild.HasChildren) children.Add(SEChild);
            else children.AddRange(SEChild.FindSouthernmostChildren());

            if (!SWChild.HasChildren) children.Add(SWChild);
            else children.AddRange(SWChild.FindSouthernmostChildren());

            return children;
        }

        #endregion

        #region Serialization

        internal void Save(BinaryWriter bw)
        {
            bw.Write(Cost);
            bw.Write(HasChildren);
            if (HasChildren)
                4.Times(i => Children[i].Save(bw));
        }

        internal static QuadTreeNode Load(RectangleF box, BinaryReader br)
        {
            QuadTreeNode node = new QuadTreeNode(box);
            node.Cost = br.ReadByte();
            bool hasChildren = br.ReadBoolean();
            if (hasChildren)
            {
                node.Quarter();
                4.Times(i => node.Load(node._children[i], node, br));
            }

            return node;
        }
        internal void Load(QuadTreeNode node, QuadTreeNode parent, BinaryReader br)
        {
            node.Parent = parent;
            node.Cost = br.ReadByte();
            bool hasChildren = br.ReadBoolean();
            if (hasChildren)
            {
                node.Quarter();
                4.Times(i => node._children[i].Load(node._children[i], node, br));
            }
        }

        #endregion

        public QuadTreeNode FindCommonAncestorWith(QuadTreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.Tree != this.Tree)
                throw new ArgumentException("Compared nodes must be in the same tree.", "node");

            if (node == this ||
                Tree.Flatten(this).Contains(node) ||
                this == Tree.Root)
                return this;

            if (Tree.Flatten(node).Contains(this) ||
                node == Tree.Root)
                return node;

            QuadTreeNode testNode = this.Parent;
            while (testNode != null && testNode != this.Tree.Root)
            {
                if (testNode.Tree.Flatten(testNode).Contains(node)) return testNode;
                testNode = testNode.Parent;
            }

            return testNode;
        }

        #endregion

        #region IComparable<QuadTreeNode> Members

        public int CompareTo(QuadTreeNode other)
        {
            return this.AStarFCost.CompareTo(other.AStarFCost);
        }

        #endregion

        public QuadTreeNode DeepClone()
        {
            QuadTreeNode n = new QuadTreeNode(this.Box);
            n.Cost = this.Cost;
            n.HasChildren = this.HasChildren;
            if (this.HasChildren)
                4.Times(i => { n._children[i] = this._children[i].DeepClone(); n._children[i].Parent = n; });

            return n;
        }

    }
}
