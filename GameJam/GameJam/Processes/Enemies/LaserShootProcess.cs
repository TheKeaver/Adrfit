﻿using System;
using Audrey;
using GameJam.Components;
using GameJam.Entities;
using Microsoft.Xna.Framework;

namespace GameJam.Processes.Enemies
{
    public class LaserShootProcess : Process
    {
        private readonly Engine Engine;
        private Entity LaserEnemyEntity;

        private Timer Timer;

        private float _initialBeamThickness;

        public LaserShootProcess(Engine engine, Entity laserEnemyEntity)
        {
            Engine = engine;
            LaserEnemyEntity = laserEnemyEntity;

            Timer = new Timer(CVars.Get<float>("laser_enemy_fire_duration"));
        }

        protected override void OnInitialize()
        {
            if (!Engine.GetEntities().Contains(LaserEnemyEntity))
            {
                SetNext(null);
                Kill();
                return;
            }

            // Create laser beam if the laser enemy doesn't currently have one
            LaserEnemyComponent laserEnemyComp = LaserEnemyEntity.GetComponent<LaserEnemyComponent>();
            TransformComponent transformComp = LaserEnemyEntity.GetComponent<TransformComponent>();
            if (laserEnemyComp.LaserBeamEntity == null)
            {
                laserEnemyComp.LaserBeamEntity = LaserBeamEntity.Create(Engine, transformComp.Position);
            }

            LaserBeamComponent laserBeamComp = laserEnemyComp.LaserBeamEntity.GetComponent<LaserBeamComponent>();
            _initialBeamThickness = laserBeamComp.Thickness;
            laserBeamComp.InteractWithShield = true;
            laserBeamComp.ComputeReflection = true;

            // Add CollisionComponent if the laser beam doesn't have one
            // TODO: CollisionComponent
        }

        protected override void OnKill()
        {
            // Destroy laser beam
            LaserEnemyComponent laserEnemyComp = LaserEnemyEntity.GetComponent<LaserEnemyComponent>();
            DestroySubLaser(laserEnemyComp.LaserBeamEntity.GetComponent<LaserBeamComponent>());
            Engine.DestroyEntity(laserEnemyComp.LaserBeamEntity);
            laserEnemyComp.LaserBeamEntity = null;
        }

        protected override void OnTogglePause()
        {

        }

        protected override void OnUpdate(float dt)
        {
            if (!Engine.GetEntities().Contains(LaserEnemyEntity))
            {
                SetNext(null);
                Kill();
                return;
            }

            Timer.Update(dt);
            if (Timer.HasElapsed())
            {
                Kill();
                return;
            }

            UpdateLaserBeam(MathHelper.Min(Timer.Alpha, 1));
        }

        private void UpdateLaserBeam(float alpha)
        {
            LaserEnemyComponent laserEnemyComp = LaserEnemyEntity.GetComponent<LaserEnemyComponent>();
            LaserBeamComponent laserBeamComp = laserEnemyComp.LaserBeamEntity.GetComponent<LaserBeamComponent>();

            float frequency = CVars.Get<float>("laser_enemy_fire_frequency");
            float offset = CVars.Get<float>("laser_enemy_fire_thickness");
            float zeroToPeak = CVars.Get<float>("laser_enemy_fire_thickness_variability");
            float initialThicknessDecaySpeed = CVars.Get<float>("laser_enemy_fire_initial_thickness_decay_factor");
            float closingEnvelopeDecaySpeed = CVars.Get<float>("laser_enemy_fire_closing_envelope_decay_factor");

            float thickness = (float)(-zeroToPeak * Math.Sin(2 * MathHelper.Pi * frequency * Timer.Elapsed)) + MathHelper.Lerp(_initialBeamThickness, offset, 1 - (float)Math.Exp(-initialThicknessDecaySpeed*5*alpha));
            thickness *= (float)(1 - Math.Exp(-closingEnvelopeDecaySpeed * 5 * (1 - alpha))); // Closing envelope
            laserBeamComp.Thickness = thickness;
        }

        private void DestroySubLaser(LaserBeamComponent laserBeamComp)
        {
            Entity beamReflectionEntity = laserBeamComp.ReflectionBeamEntity;
            if (beamReflectionEntity != null)
            {
                DestroySubLaser(laserBeamComp.ReflectionBeamEntity.GetComponent<LaserBeamComponent>());
            }
            Engine.DestroyEntity(beamReflectionEntity);
        }
    }
}
