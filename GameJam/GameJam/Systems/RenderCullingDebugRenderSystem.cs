﻿using Audrey;
using GameJam.Common;
using GameJam.Components;
using GameJam.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameJam.Systems
{
    public class RenderCullingDebugRenderSystem
    {
        protected Engine Engine
        {
            get;
            private set;
        }

        public static readonly Vector2 FlipY = new Vector2(1, -1);
        public static readonly Vector2 HalfHalf = new Vector2(0.5f, 0.5f);

        readonly Family _collisionFamily = Family.All(typeof(TransformComponent), typeof(VectorSpriteComponent)).Get();
        readonly ImmutableList<Entity> _collisionEntities;

        public SpriteBatch SpriteBatch { get; }
        public GraphicsDevice GraphicsDevice
        {
            get;
            private set;
        }

        public RenderCullingDebugRenderSystem(GraphicsDevice graphics, Engine engine)
        {
            Engine = engine;
            _collisionEntities = Engine.GetEntitiesFor(_collisionFamily);

            SpriteBatch = new SpriteBatch(graphics);
            GraphicsDevice = graphics;
        }

        /*public void Draw(float dt)
        {
            Draw(Matrix.Identity, dt);
        }*/

        public void Draw(Camera camera, float dt)
        {
            Matrix transformMatrix = camera.TransformMatrix;
            SpriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.PointClamp,
                null,
                null,
                null,
                transformMatrix);

            BoundingRect cameraRect = camera.boundingRect;
            SpriteBatch.DrawRectangle(new Rectangle((int)cameraRect.Left,
                (int)cameraRect.Bottom,
                (int)cameraRect.Width,
                (int)cameraRect.Height),
                Color.Orange,
                1);

            foreach (Entity entity in _collisionEntities)
            {
                VectorSpriteComponent vectorSpriteComp = entity.GetComponent<VectorSpriteComponent>();
                TransformComponent transformComp = entity.GetComponent<TransformComponent>();

                Vector2 position = transformComp.Position * FlipY;


                float cos = (float)Math.Cos(-transformComp.Rotation);
                float sin = (float)Math.Sin(-transformComp.Rotation);
                float scale = transformComp.Scale;

                foreach (RenderShape shape in vectorSpriteComp.RenderShapes)
                {
                    BoundingRect AABB = shape.GetAABB(scale);
                    AABB.Min += position;
                    AABB.Max += position;
                    SpriteBatch.DrawRectangle(new Rectangle((int)AABB.Left,
                        (int)AABB.Bottom,
                        (int)AABB.Width,
                        (int)AABB.Height),
                        Color.BlueViolet,
                        1);
                }

            }

            SpriteBatch.End();
        }

        private Vector2 rotateAroundOrigin(Vector2 p, float rotation)
        {
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);
            return rotateAroundOrigin(p, cos, sin);
        }
        private Vector2 rotateAroundOrigin(Vector2 p, float cos, float sin)
        {
            return new Vector2(p.X * cos - p.Y * sin,
                p.X * sin + p.Y * cos);
        }
    }
}
