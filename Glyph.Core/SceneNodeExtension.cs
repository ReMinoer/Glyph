using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    static public class SceneNodeExtension
    {
        static private IEnumerable<ISceneNode> GetChildrenNodes(ISceneNode x) => x.Children;
        static private ISceneNode GetParentNode(ISceneNode x) => x.ParentNode;

        static public IEnumerable<ISceneNode> ChildrenNodesQueueExclusive(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return Enumerable.Empty<ISceneNode>();

            return Tree.BreadthFirstExclusive(sceneNode, GetChildrenNodes);
        }

        static public IEnumerable<ISceneNode> ChildrenNodesQueue(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return Enumerable.Empty<ISceneNode>();

            return Tree.BreadthFirst(sceneNode, GetChildrenNodes);
        }
        
        static public IEnumerable<ISceneNode> ParentNodeQueueExclusive(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return Enumerable.Empty<ISceneNode>();

            return Sequence.AggregateExclusive(sceneNode, GetParentNode);
        }

        static public IEnumerable<ISceneNode> ParentNodeQueue(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return Enumerable.Empty<ISceneNode>();

            return Sequence.Aggregate(sceneNode, GetParentNode);
        }

        static public Vector2 Project(this ISceneNode sceneNode, ISceneNode space)
        {
            Vector2 result = sceneNode.LocalPosition;
            Stack<ISceneNode> descendants = null;

            ISceneNode sharedParent = null;
            foreach (ISceneNode ascendant in sceneNode.ParentNodeQueueExclusive())
            {
                if (descendants == null)
                {
                    descendants = new Stack<ISceneNode>();
                    foreach (ISceneNode descendant in space.ParentNodeQueue())
                    {
                        if (ascendant.Represent(descendant))
                        {
                            sharedParent = descendant;
                            break;
                        }
                        
                        descendants.Push(descendant);
                    }
                }
                else
                {
                    sharedParent = descendants.FirstOrDefault(x => !ascendant.Represent(x));
                }
                
                if (sharedParent != null)
                    break;
                
                result = ascendant.LocalMatrix * result;
            }

            if (descendants == null)
                descendants = new Stack<ISceneNode>(space.ParentNodeQueue());
            else if (sharedParent != null && descendants.Count > 0)
                while (descendants.Pop() != sharedParent) { }

            while (descendants.Count > 0)
                result = descendants.Pop().LocalMatrix.Inverse * result;

            return result;
        }

        static public float DistanceTo(this ISceneNode a, ISceneNode b)
        {
            return (a.Position - b.Position).Length();
        }
    }
}