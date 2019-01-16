using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Foundry.Fluent;

namespace QTTest2
{
    public static class PointFExtensions
    {
        public static float DistanceTo(this PointF p1, PointF p2)
        {
            return (float)
                (Math.Sqrt(
                    Math.Pow(Math.Abs(p1.X - p2.X), 2) +
                    Math.Pow(Math.Abs(p1.Y - p2.Y), 2))
                );
        }
    }

    public class AStar
    {
        public IEnumerable<QuadTreeNode> InitialSmooth(LinkedList<QuadTreeNode> path, QuadTree tree)
        {
            yield return path.First.Value;

            int current = 0;
            int curHalf = 0;

            do
            {
                // If current = count of nodes, break out.
                if (current == path.Count) break;

                QuadTreeNode curNode = path.ElementAt(current);
                QuadTreeNode testNode = path.Last.Value;

                // Check to see if we can see the destination from the current node.
                LineF testLine = new LineF
                {
                    StartPos = curNode.Box.Midpoint(),
                    EndPos = testNode.Box.Midpoint()
                };
                var commonAncestor = curNode.FindCommonAncestorWith(testNode);
                var intersectingLines = tree.Flatten(commonAncestor).Where(n => QuadTree.DoesLineIntersectRect(testLine, n.Box));

                if (!intersectingLines.Any(n => n.Cost > curNode.Cost))
                {
                    // It's not blocked - return this then quit.
                    yield return testNode;
                    yield break;
                }
                else
                {
                    // Get the current nearest half to check
                    if (curHalf == 0)
                        curHalf = current + (int)((path.Count - current) / 2);
                    else
                        curHalf = current + (int)((curHalf - current) / 2);

                    // If curHalf == current then we just return the next node.
                    if (curHalf == current)
                    {
                        curHalf = 0;
                        current++;
                        if (current >= path.Count) break;
                        yield return path.ElementAt(current);
                        continue;
                    }

                    // Check to see if this half is obstructed
                    testNode = path.ElementAt(curHalf);
                    testLine = new LineF
                    {
                        StartPos = curNode.Box.Midpoint(),
                        EndPos = testNode.Box.Midpoint()
                    };
                    commonAncestor = curNode.FindCommonAncestorWith(testNode);
                    intersectingLines = tree.Flatten(commonAncestor).Where(n => QuadTree.DoesLineIntersectRect(testLine, n.Box));

                    // If it is, continue;
                    if (intersectingLines.Any(n => n.Cost > curNode.Cost)) continue;

                    // If it's not, yield this position and reset curHalf.
                    yield return testNode;
                    current = curHalf;
                    curHalf = 0;
                }
            } while (true);

            yield return path.Last.Value;
        }

        public IEnumerable<QuadTreeNode> SmoothPath(int limit, LinkedList<QuadTreeNode> path, QuadTree tree, Action<LineF, IList<QuadTreeNode>, QuadTreeNode, QuadTreeNode> perStepCallback)
        {
            yield return path.First.Value;

            LinkedListNode<QuadTreeNode> curNode = path.First;
            LinkedListNode<QuadTreeNode> testNode = curNode.Next.Next;

            while (testNode != null)
            {
                LineF testLine = new LineF
                {
                    StartPos = curNode.Value.Box.Midpoint(),
                    EndPos = testNode.Value.Box.Midpoint()
                };

                var commonAncestor = curNode.Value.FindCommonAncestorWith(testNode.Value);
                var intersectingLines = tree.Flatten(commonAncestor).Where(n => QuadTree.DoesLineIntersectRect(testLine, n.Box));

                if (perStepCallback != null)
                {
                    perStepCallback(testLine, intersectingLines.ToList(), curNode.Value, testNode.Value);
                }

                bool blocking = false;
                foreach (var node in intersectingLines)
                {
                    blocking = false;
                    if (node.Cost > curNode.Value.Cost)
                    {
                        // Blocking
                        // If testNode.Previous = curNode then we need to just make curNode = testNode
                        if (testNode.Previous == curNode)
                        {
                            yield return testNode.Value;
                            curNode = testNode;
                        }
                        else
                        {
                            // Add previous test node to smoothed path
                            yield return testNode.Previous.Value;
                            curNode = testNode.Previous;
                        }

                        testNode = curNode.Next;
                        blocking = true;
                        break;
                    }
                    else if (node.Cost < curNode.Value.Cost)
                    {
                        // Non-Blocking, lower cost entry
                        // Add this node into the path and continue.
                        yield return node;
                        path.AddAfter(testNode, node);
                        curNode = testNode.Next;
                        testNode = curNode;
                        break;
                    }
                }

                if (!blocking)
                {
                    // Check to see if the distance here is too great.
                    if (limit != 0)
                    {
                        if (curNode.Value.Box.Midpoint().DistanceTo(testNode.Value.Box.Midpoint()) > limit)
                        {
                            yield return testNode.Value;
                            curNode = testNode;
                        }
                    }

                    testNode = testNode.Next;
                }
            }

            yield return path.Last.Value;
        }

        public LinkedList<QuadTreeNode> FindPath(float hMod, QuadTreeNode start, QuadTreeNode goal, Action<IEnumerable<QuadTreeNode>, IEnumerable<QuadTreeNode>, int> perStepCallback)
        {
            Timing.ClearTimings(); 
            using (new Timing("AStar"))
            {
                using (new Timing("ClearFlags"))
                    start.Tree.Flatten().ForEach(n => { n.AStarIsClosed = false; n.AStarIsOpen = false; });

                using (new Timing("AStarBody"))
                {
                    start.AStarHCost =
                    (int)(hMod * 138 * (Math.Sqrt(
                        Math.Pow(Math.Abs(start.Box.Midpoint().X - goal.Box.Midpoint().X), 2) +
                        Math.Pow(Math.Abs(start.Box.Midpoint().Y - goal.Box.Midpoint().Y), 2)
                    )));

                    start.AStarGCost = start.Cost *start.Depth;

                    BinaryHeap<QuadTreeNode> openList;
                    openList = new BinaryHeap<QuadTreeNode> { start };

                    List<QuadTreeNode> openThisStep = new List<QuadTreeNode>();

                    long steps = 0;

                    do
                    {
                        if ((steps % 25) == 0)
                        {
                            if (perStepCallback != null) perStepCallback(openThisStep, new List<QuadTreeNode>(), 0);
                            openThisStep.Clear();
                            steps = 0;
                        }

                        steps++;

                        QuadTreeNode current;
                        current = openList.Pop();

                        if (current == goal) break;
                        if (current == null) return new LinkedList<QuadTreeNode>();

                        current.AStarIsClosed = true;

                        QuadTreeNode[] neighbors;
                        neighbors = current.Neighbors.Where(n => n.Cost != 255).ToArray();

                        for (int i = 0; i < neighbors.Length; i++)
                        {
                            int dist;
                            dist = (int)(Math.Sqrt(
                                Math.Pow(Math.Abs(goal.Box.Midpoint().X - neighbors[i].Box.Midpoint().X), 2) +
                                Math.Pow(Math.Abs(goal.Box.Midpoint().Y - neighbors[i].Box.Midpoint().Y), 2)
                                )
                            );

                            if (!(neighbors[i].AStarIsOpen || neighbors[i].AStarIsClosed))
                            {
                                var cost = current.AStarGCost + (neighbors[i].Cost * dist * neighbors[i].Depth);
                                var h = (int)(hMod * 138 * dist);

                                neighbors[i].AStarGCost = cost;
                                neighbors[i].AStarHCost = h;

                                neighbors[i].AStarParent = current;
                                openList.Add(neighbors[i]);

                                neighbors[i].AStarIsOpen = true;
                                openThisStep.Add(neighbors[i]);
                            }
                        }

                    } while (true);

                    if (perStepCallback != null) perStepCallback(openThisStep, new List<QuadTreeNode>(), 0);
                }


                using (new Timing("CreateWalkpath"))
                {
                    var ll = new LinkedList<QuadTreeNode>();

                    QuadTreeNode walkbackCurrent = goal;
                    do
                    {
                        ll.AddFirst(walkbackCurrent);
                        walkbackCurrent = walkbackCurrent.AStarParent;
                    } while (walkbackCurrent != start);
                    ll.AddFirst(start);

                    return ll;
                }
            }
        }
    }
}