using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    static public class SceneNodeExtension
    {
        static public IEnumerable<ISceneNode> ChildrenNodeQueue(this ISceneNode sceneNode)
        {
            foreach (ISceneNode child in sceneNode.Children)
                yield return child;

            foreach (ISceneNode child in sceneNode.Children.SelectMany(x => x.ChildrenNodeQueue()))
                yield return child;
        }

        static public IEnumerable<ISceneNode> ItselfAndChildrenNodeQueue(this ISceneNode sceneNode)
        {
            yield return sceneNode;
            foreach (ISceneNode child in sceneNode.ChildrenNodeQueue())
                yield return child;
        }

        static public IEnumerable<ISceneNode> ParentNodeQueue(this ISceneNode sceneNode)
        {
            for (ISceneNode parent = sceneNode.ParentNode; parent != null; parent = parent.ParentNode)
                yield return parent;
        }

        static public IEnumerable<ISceneNode> ItselfAndParentNodeQueue(this ISceneNode sceneNode)
        {
            yield return sceneNode;
            for (ISceneNode parent = sceneNode.ParentNode; parent != null; parent = parent.ParentNode)
                yield return parent;
        }

        static public Vector2 Project(this ISceneNode sceneNode, ISceneNode space)
        {
            Vector2 result = sceneNode.LocalPosition;
            Stack<ISceneNode> descendants = null;

            ISceneNode sharedParent = null;
            foreach (ISceneNode ascendant in sceneNode.ParentNodeQueue())
            {
                if (descendants == null)
                {
                    descendants = new Stack<ISceneNode>();
                    foreach (ISceneNode descendant in space.ItselfAndParentNodeQueue())
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
                descendants = new Stack<ISceneNode>(space.ItselfAndParentNodeQueue());
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