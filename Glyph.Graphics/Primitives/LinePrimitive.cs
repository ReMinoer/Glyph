﻿using System;
using System.Collections.Generic;
using Glyph.Graphics.Primitives.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class LinePrimitive : OutlinePrimitiveBase
    {
        private Vector2[] _points;
        public Vector2[] Points
        {
            get => _points;
            set
            {
                if (_points == value)
                    return;

                _points = value;
                DirtyVertices();
            }
        }

        public VertexBufferType BufferType { get; set; } = VertexBufferType.Strip;
        protected override PrimitiveType PrimitiveType => BufferType == VertexBufferType.Strip ? PrimitiveType.LineStrip : PrimitiveType.LineList;

        public LinePrimitive()
        {
        }

        public LinePrimitive(params Vector2[] points)
        {
            Points = points;
        }

        public LinePrimitive(Color color, params Vector2[] points)
            : this(points)
        {
            Colors = new[] { color };
        }

        public LinePrimitive(Color color, VertexBufferType bufferType, params Vector2[] points)
            : this(color, points)
        {
            BufferType = bufferType;
        }

        protected override int GetRefreshedVertexCount() => Points.Length;
        protected override IEnumerable<Vector2> GetRefreshedVertices() => Points;
        protected override int GetRefreshedIndexCount() => 0;
        protected override IEnumerable<ushort> GetRefreshedIndices() => null;
    }
}