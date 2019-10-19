using System;
using System.Collections.Generic;
using Audrey;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam.Components
{
    public class VectorSpriteComponent : IComponent
    {
        public VectorSpriteComponent()
        {
        }
        public VectorSpriteComponent(RenderShape[] renderShapes)
        {
            RenderShapes = new List<RenderShape>(renderShapes);
        }

        public List<RenderShape> RenderShapes = new List<RenderShape>();

        public byte RenderGroup = 0x1;

        public bool Hidden;
    }

    public abstract class RenderShape
    {
        public abstract VertexPositionColor[] ComputeVertices();
    }

    public class TriangleRenderShape : RenderShape
    {
        readonly VertexPositionColor[] _verts;

        public TriangleRenderShape(Vector2 v1, Vector2 v2, Vector2 v3, Color color)
        {
            _verts = new VertexPositionColor[] {
                new VertexPositionColor(new Vector3(v1, 0), color),
                new VertexPositionColor(new Vector3(v2, 0), color),
                new VertexPositionColor(new Vector3(v3, 0), color)
            };
        }

        public override VertexPositionColor[] ComputeVertices()
        {
            return _verts;
        }
    }

    public class QuadRenderShape : RenderShape
    {
        VertexPositionColor[] _verts;

        private Vector2 _v1;
        private Vector2 _v2;
        private Vector2 _v3;
        private Vector2 _v4;
        private Color _color;

        public Vector2 V1
        {
            get
            {
                return _v1;
            }
            set
            {
                _v1 = value;
                ComputeVertices();
            }
        }
        public Vector2 V2
        {
            get
            {
                return _v2;
            }
            set
            {
                _v2 = value;
                ComputeVertices();
            }
        }
        public Vector2 V3
        {
            get
            {
                return _v3;
            }
            set
            {
                _v3 = value;
                ComputeVertices();
            }
        }
        public Vector2 V4
        {
            get
            {
                return _v4;
            }
            set
            {
                _v4 = value;
                ComputeVertices();
            }
        }
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                ComputeVertices();
            }
        }

        public QuadRenderShape(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, Color color)
        {
            _v1 = v1;
            _v2 = v2;
            _v3 = v3;
            _v4 = v4;
            _color = color;

            RebuildVerts();
        }

        public override VertexPositionColor[] ComputeVertices()
        {
            return _verts;
        }

        private void RebuildVerts()
        {
            _verts = new VertexPositionColor[] {
                new VertexPositionColor(new Vector3(_v1, 0), _color),
                new VertexPositionColor(new Vector3(_v2, 0), _color),
                new VertexPositionColor(new Vector3(_v3, 0), _color),

                new VertexPositionColor(new Vector3(_v1, 0), _color),
                new VertexPositionColor(new Vector3(_v3, 0), _color),
                new VertexPositionColor(new Vector3(_v4, 0), _color)
            };
        }
    }

    public class PolyRenderShape : RenderShape
    {
        public enum PolyCapStyle
        {
            None,
            AwayFromCenter,
            Filled
        }

        readonly VertexPositionColor[] _verts;

        public PolyRenderShape(Vector2[] points, float thickness, Color color, PolyCapStyle polyCapStyle = PolyCapStyle.None, bool closed = false)
        {
            int count = points.Length;
            if(closed)
            {
                count++;
            }
            switch(polyCapStyle)
            {
                case PolyCapStyle.Filled:
                    _verts = new VertexPositionColor[(count - 1) * 9]; // TODO
                    break;
                case PolyCapStyle.AwayFromCenter:
                default:
                    _verts = new VertexPositionColor[(count - 1) * 6];
                    break;
            }
            int v = 0;
            for(int i = 1; i < points.Length + (closed ? 1 : 0); i++)
            {
                Vector2 p1 = points[i - 1];
                Vector2 p2;
                if (i >= points.Length) {
                    p2 = points[0];
                } else
                {
                    p2 = points[i];
                }

                Vector2 p2p1 = p2 - p1;
                Vector2 d = new Vector2(-p2p1.Y, p2p1.X);
                d.Normalize();


                if (polyCapStyle == PolyCapStyle.AwayFromCenter)
                {
                    Vector2 oToP1 = new Vector2(p1.X, p1.Y);
                    oToP1.Normalize();
                    Vector2 oToP2 = new Vector2(p2.X, p2.Y);
                    oToP2.Normalize();

                    float p1Thickness = thickness / Math.Abs(Vector2.Dot(d, oToP1));
                    float p2Thickness = thickness / Math.Abs(Vector2.Dot(d, oToP2));

                    Vector2 v1b = p1 - p1Thickness / 2 * oToP1;
                    Vector2 v1t = p1 + p1Thickness / 2 * oToP1;
                    Vector2 v2b = p2 - p2Thickness / 2 * oToP2;
                    Vector2 v2t = p2 + p2Thickness / 2 * oToP2;

                    if (MathHelper.WrapAngle((float)(Math.Atan2(p2.Y, p2.X) - Math.Atan2(p1.Y, p1.X))) > 0)
                    {
                        _verts[v++] = new VertexPositionColor(new Vector3(v1b.X, v1b.Y, 0), color);
                        _verts[v++] = new VertexPositionColor(new Vector3(v1t.X, v1t.Y, 0), color);
                        _verts[v++] = new VertexPositionColor(new Vector3(v2t.X, v2t.Y, 0), color);

                        _verts[v++] = new VertexPositionColor(new Vector3(v1b.X, v1b.Y, 0), color);
                        _verts[v++] = new VertexPositionColor(new Vector3(v2t.X, v2t.Y, 0), color);
                        _verts[v++] = new VertexPositionColor(new Vector3(v2b.X, v2b.Y, 0), color);
                    }
                    else
                    {
                        _verts[v++] = new VertexPositionColor(new Vector3(v2t.X, v2t.Y, 0), color);
                        _verts[v++] = new VertexPositionColor(new Vector3(v1t.X, v1t.Y, 0), color);
                        _verts[v++] = new VertexPositionColor(new Vector3(v1b.X, v1b.Y, 0), color);

                        _verts[v++] = new VertexPositionColor(new Vector3(v2b.X, v2b.Y, 0), color);
                        _verts[v++] = new VertexPositionColor(new Vector3(v2t.X, v2t.Y, 0), color);
                        _verts[v++] = new VertexPositionColor(new Vector3(v1b.X, v1b.Y, 0), color);
                    }
                    continue;
                }

                { // Empty
                    Vector2 v1b = p1 - d * thickness;
                    Vector2 v1t = p1 + d * thickness;
                    Vector2 v2b = p2 - d * thickness;
                    Vector2 v2t = p2 + d * thickness;

                    _verts[v++] = new VertexPositionColor(new Vector3(v2t.X, v2t.Y, 0), color);
                    _verts[v++] = new VertexPositionColor(new Vector3(v1t.X, v1t.Y, 0), color);
                    _verts[v++] = new VertexPositionColor(new Vector3(v1b.X, v1b.Y, 0), color);

                    _verts[v++] = new VertexPositionColor(new Vector3(v2b.X, v2b.Y, 0), color);
                    _verts[v++] = new VertexPositionColor(new Vector3(v2t.X, v2t.Y, 0), color);
                    _verts[v++] = new VertexPositionColor(new Vector3(v1b.X, v1b.Y, 0), color);

                    if(polyCapStyle == PolyCapStyle.Filled)
                    {
                        int j;
                        if (i >= points.Length)
                        {
                            j = 0;
                        }
                        else
                        {
                            j = i;
                        }
                        Vector2 p3 = points[j];
                        Vector2 p4;

                        if(j == points.Length - 1)
                        {
                            if(!closed)
                            {
                                continue;
                            }

                            p4 = points[0];
                        } else
                        {
                            p4 = points[j + 1];
                        }

                        Vector2 p4p3 = p4 - p3;
                        Vector2 d2 = new Vector2(-p4p3.Y, p4p3.X);
                        d2.Normalize();

                        Vector2 v3b = p3 - d2 * thickness;
                        Vector2 v3t = p3 + d2 * thickness;

                        // Check which to fill in - top or bottom
                        Vector2 top = v3t - v2t;

                        Vector2 midpoint = p2;
                        if (Vector2.Dot(top, p2p1) > 0)
                        {
                            //Vector2 midpoint = (v3b - v2b) / 2 + v2b;
                            _verts[v++] = new VertexPositionColor(new Vector3(v3t.X, v3t.Y, 0), color);
                            _verts[v++] = new VertexPositionColor(new Vector3(v2t.X, v2t.Y, 0), color);
                            _verts[v++] = new VertexPositionColor(new Vector3(midpoint.X, midpoint.Y, 0), color);
                        } else
                        {
                            //Vector2 midpoint = (v3t - v2t) / 2 + v2t;
                            _verts[v++] = new VertexPositionColor(new Vector3(v2b.X, v2b.Y, 0), color);
                            _verts[v++] = new VertexPositionColor(new Vector3(v3b.X, v3b.Y, 0), color);
                            _verts[v++] = new VertexPositionColor(new Vector3(midpoint.X, midpoint.Y, 0), color);
                        }
                    }
                }
            }
        }

        public override VertexPositionColor[] ComputeVertices()
        {
            return _verts;
        }
    }
}
