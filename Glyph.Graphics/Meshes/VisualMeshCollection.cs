using System.Collections;
using System.Collections.Generic;

namespace Glyph.Graphics.Meshes
{
    public class VisualMeshCollection : IVisualMeshProvider, IList<IVisualMesh>
    {
        private readonly IList<IVisualMesh> _list = new List<IVisualMesh>();
        public IEnumerable<IVisualMesh> Meshes => _list;

        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                foreach (IVisualMesh mesh in _list)
                    mesh.Visible = value;
            }
        }

        public IVisualMesh this[int index]
        {
            get => _list[index];
            set
            {
                value.Visible = Visible;
                _list[index] = value;
            }
        }

        public int Count => _list.Count;

        public void Add(IVisualMesh item)
        {
            item.Visible = Visible;
            _list.Add(item);
        }

        public void Insert(int index, IVisualMesh item)
        {
            item.Visible = Visible;
            _list.Insert(index, item);
        }

        public bool Remove(IVisualMesh item) => _list.Remove(item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public void Clear() => _list.Clear();
        public bool Contains(IVisualMesh item) => _list.Contains(item);
        public int IndexOf(IVisualMesh item) => _list.IndexOf(item);
        public void CopyTo(IVisualMesh[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        bool ICollection<IVisualMesh>.IsReadOnly => _list.IsReadOnly;

        public IEnumerator<IVisualMesh> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}