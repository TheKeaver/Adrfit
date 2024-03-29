﻿using System;
using Audrey;
using GameJam.Common;
using GameJam.Components;
using GameJam.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam.Processes
{
    public class ShooterEnemySpawner : IntervalProcess
    {
        readonly Engine Engine;
        readonly ContentManager Content;
        readonly ProcessManager ProcessManager;
        readonly MTRandom random = new MTRandom();

        readonly Family _playerShipFamily = Family.All(typeof(TransformComponent), typeof(PlayerShipComponent)).Get();
        readonly ImmutableList<Entity> _playerShipEntities;

        readonly Family _enemyFamily = Family.All(typeof(EnemyComponent)).Exclude(typeof(ProjectileComponent)).Get();
        readonly ImmutableList<Entity> _enemyEntities;

        public ShooterEnemySpawner(Engine engine, ContentManager content, ProcessManager processManager)
            : base(Constants.GamePlay.SPAWNER_SHOOTING_ENEMY_INITIAL_PERIOD)
        {
            Engine = engine;
            Content = content;
            ProcessManager = processManager;

            _playerShipEntities = engine.GetEntitiesFor(_playerShipFamily);
            _enemyEntities = engine.GetEntitiesFor(_enemyFamily);
        }

        protected override void OnTick(float interval)
        {
            if (_enemyEntities.Count < Constants.GamePlay.SPAWNER_MAX_ENEMY_COUNT)
            {
                Vector2 spawnPosition = new Vector2(0, 0);
                do
                {
                    spawnPosition.X = random.NextSingle(-Constants.Global.WINDOW_WIDTH / 2 * 0.9f, Constants.Global.WINDOW_WIDTH / 2 * 0.9f);
                    spawnPosition.Y = random.NextSingle(-Constants.Global.WINDOW_HEIGHT / 2 * 0.9f, Constants.Global.WINDOW_HEIGHT / 2 * 0.9f);
                } while (IsTooCloseToPlayer(spawnPosition));

                ShootingEnemyEntity.Create(Engine,
                    Content.Load<Texture2D>(Constants.Resources.TEXTURE_SHOOTER_ENEMY),
                    spawnPosition, ProcessManager, Content);
            }

            Interval = MathHelper.Max(Interval, Constants.GamePlay.SPAWNER_KAMIKAZE_PERIOD_MIN);
        }

        bool IsTooCloseToPlayer(Vector2 position)
        {
            float minDistanceToPlayer = float.MaxValue;

            foreach(Entity playerShip in _playerShipEntities)
            {
                TransformComponent transformComponent = playerShip.GetComponent<TransformComponent>();
                Vector2 toPlayer = transformComponent.Position - position;
                if(toPlayer.Length() < minDistanceToPlayer)
                {
                    minDistanceToPlayer = toPlayer.Length();
                }
            }

            return minDistanceToPlayer <= Constants.GamePlay.SPANWER_MIN_DISTANCE_AWAY_FROM_PLAYER;
        }
    }
}
