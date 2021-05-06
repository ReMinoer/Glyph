using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class SceneNodeExtension
    {
        static public IEnumerable<ISceneNode> AllChildNodes(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return Enumerable.Empty<ISceneNode>();

            return Tree.BreadthFirstExclusive(sceneNode, x => x.Children);
        }

        static public IEnumerable<ISceneNode> AndAllChildNodes(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return Enumerable.Empty<ISceneNode>();

            return Tree.BreadthFirst(sceneNode, x => x.Children);
        }
        
        static public IEnumerable<ISceneNode> AllParentNodes(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return Enumerable.Empty<ISceneNode>();

            return Sequence.AggregateExclusive(sceneNode, x => x.ParentNode);
        }

        static public IEnumerable<ISceneNode> AndAllParentNodes(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return Enumerable.Empty<ISceneNode>();

            return Sequence.Aggregate(sceneNode, x => x.ParentNode);
        }

        static public ISceneNode RootNode(this ISceneNode sceneNode)
        {
            if (sceneNode == null)
                return null;
            
            while (sceneNode.ParentNode != null)
                sceneNode = sceneNode.ParentNode;

            return sceneNode;
        }

        static public float DistanceTo(this ISceneNode a, ISceneNode b)
        {
            return (a.Position - b.Position).Length();
        }

        static public Vector2 Project(this ISceneNode sceneNode, ISceneNode space)
        {
            Vector2 result = sceneNode.LocalPosition;
            Stack<ISceneNode> descendants = null;

            ISceneNode sharedParent = null;
            foreach (ISceneNode ascendant in sceneNode.AllParentNodes())
            {
                if (descendants == null)
                {
                    descendants = new Stack<ISceneNode>();
                    foreach (ISceneNode descendant in space.AndAllParentNodes())
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
                descendants = new Stack<ISceneNode>(space.AndAllParentNodes());
            else if (sharedParent != null && descendants.Count > 0)
                while (descendants.Pop() != sharedParent) { }

            while (descendants.Count > 0)
                result = descendants.Pop().LocalMatrix.Inverse * result;

            return result;
        }
    }
}