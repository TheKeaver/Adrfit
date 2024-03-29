﻿using System;
using Audrey;
using GameJam.Components;
using Microsoft.Xna.Framework;

namespace GameJam.Processes
{
    public class GameOverAnimationProcess : Process
    {
        Entity entity;
        Timer timer;
        readonly float time = 4;

        public GameOverAnimationProcess(Entity entity)
        {
            this.entity = entity;
            timer = new Timer(time);
        }

        protected override void OnInitialize()
        {
            OnUpdate(0);
        }

        protected override void OnKill()
        {
            
        }

        protected override void OnTogglePause()
        {
            
        }

        protected override void OnUpdate(float dt)
        {
            timer.Update(dt);
            if(timer.HasElapsed())
            {
                Kill();
            }

            float alpha = timer.Elapsed / time;
            alpha = MathHelper.Clamp(alpha, 0, 1);
            Vector2 pos = entity.GetComponent<TransformComponent>().Position;
            float beta = Easings.ExponentialEaseOut(alpha);
            pos.Y = MathHelper.Lerp(1.25f * Constants.Global.WINDOW_HEIGHT / 2, 0, beta);
            entity.GetComponent<TransformComponent>().Move(pos - entity.GetComponent<TransformComponent>().Position);
        }
    }
}
