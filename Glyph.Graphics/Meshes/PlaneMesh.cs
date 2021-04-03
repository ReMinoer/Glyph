using System.Collections.Generic;
using Glyph.Graphics.Meshes.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes
{
    public class PlaneMesh : ProceduralMeshBase
    {
        private float _x;
        public float X
        {
            get => _x;
            set => SetAndDirtyCachesOnChanged(ref _x, value);
        }

        private float _y;
        public float Y
        {
            get => _y;
            set => SetAndDirtyCachesOnChanged(ref _y, value);
        }

        private float _width;
        public float Width
        {
            get => _width;
            set => SetAndDirtyCachesOnChanged(ref _width, value);
        }

        private float _height;
        public float Height
        {
            get => _height;
            set => SetAndDirtyCachesOnChanged(ref _height, value);
        }

        private int _columns = 1;
        public int Columns
        {
            get => _columns;
            set => SetAndDirtyCachesOnChanged(ref _columns, value);
        }

        private int _rows = 1;
        public int Rows
        {
            get => _rows;
            set => SetAndDirtyCachesOnChanged(ref _rows, value);
        }

        public Color Color { get; set; } = Color.White;
        protected override Color GetColor(int vertexIndex) => Color;

        public override PrimitiveType Type => PrimitiveType.TriangleList;

        public PlaneMesh(Vector2 center, Vector2 size, Point gridSize)
        {
            X = center.X - size.X / 2;
            Y = center.Y - size.Y / 2;
            Width = size.X;
            Height = size.Y;
            Columns = gridSize.X;
            Rows = gridSize.Y;
        }

        protected override void RefreshCache(List<Vector2> vertices, List<int> indices)
        {
            vertices.Clear();
            indices.Clear();

            var cellSize = new Vector2(Width / Columns, Height / Rows);

            for (int i = 0; i <= Rows; i++)
                for (int j = 0; j <= Columns; j++)
                {
                    vertices.Add(new Vector2(_x, _y) + new Vector2(j, i) * cellSize);
                }

            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                {
                    indices.Add(GetIndex(i, j));
                    indices.Add(GetIndex(i, j + 1));
                    indices.Add(GetIndex(i + 1, j));

                    indices.Add(GetIndex(i + 1, j + 1));
                    indices.Add(GetIndex(i + 1, j));
                    indices.Add(GetIndex(i, j + 1));
                }

            int GetIndex(int i, int j) => i * (Columns + 1) + j;
        }
    }
}