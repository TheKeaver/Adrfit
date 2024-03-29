﻿using Audrey;
using GameJam.Common;
using GameJam.Components;
using GameJam.Processes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameJam.Entities
{
    public static class ShootingEnemyEntity
    {
        public static Entity Create(Engine engine, Texture2D texture, Vector2 position, ProcessManager processManager, ContentManager conTENt)
        {
            Entity entity = engine.CreateEntity();

            entity.AddComponent(new TransformComponent(position));
            entity.AddComponent(new SpriteComponent(texture, Constants.ObjectBounds.SHOOTING_SHIP_BOUNDS));
            entity.AddComponent(new ShootingEnemyComponent(Constants.GamePlay.SHOOTING_ENEMY_PROJECTILE_AMMO));
            entity.AddComponent(new RotationComponent(Constants.GamePlay.SHOOTING_ENEMY_ROTATION_SPEED));
            entity.AddComponent(new MovementComponent());
            entity.AddComponent(new EnemyComponent());
            entity.AddComponent(new CollisionComponent(new BoundingRect(0, 0, 17.5f, 35)));

            FireProjectileProcess fpp = new FireProjectileProcess(entity, engine, conTENt);
            processManager.Attach(fpp);
            entity.AddComponent(new ProjectileSpawningProcessComponent(fpp));

            return entity;
        }
    }
}
