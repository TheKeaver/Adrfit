﻿using System;
using System.Collections.Generic;
using Events;
using GameJam.Events;
using GameJam.Events.InputHandling;
using GameJam.Events.UI;
using GameJam.UI;
using Microsoft.Xna.Framework.Graphics;
using UI.Content.Pipeline;

namespace GameJam.States
{
    class UIMenuGameState : GameState, IEventListener
    {
        SpriteBatch _spriteBatch;
        Root _root;

        private ProcessManager ProcessManager
        {
            get;
            set;
        }

        public UIMenuGameState(GameManager gameManager) : base(gameManager)
        {
            _spriteBatch = new SpriteBatch(GameManager.GraphicsDevice);
        }

        protected override void OnInitialize()
        {
            ProcessManager = new ProcessManager();

            _root = new Root(GameManager.GraphicsDevice.Viewport.Width,
                GameManager.GraphicsDevice.Viewport.Height);
            _root.BuildFromPrototypes(Content, Content.Load<List<WidgetPrototype>>("ui/MainMenu"));

            RegisterEvents();
            _root.RegisterListeners();

            base.OnInitialize();
        }

        protected override void OnUpdate(float dt)
        {
            ProcessManager.Update(dt);

            base.OnUpdate(dt);
        }

        protected override void OnFixedUpdate(float dt)
        {
            base.OnFixedUpdate(dt);
        }

        protected override void OnRender(float dt, float betweenFrameAlpha)
        {
            _spriteBatch.Begin();
            _root.Draw(_spriteBatch);
            _spriteBatch.End();

            base.OnRender(dt, betweenFrameAlpha);
        }

        protected override void OnKill()
        {
            UnregisterEvents();
            _root.UnregisterListeners();

            base.OnKill();
        }

        void RegisterEvents()
        {
            EventManager.Instance.RegisterListener<PlayGameButtonPressedEvent>(this);
            EventManager.Instance.RegisterListener<OptionsButtonPressedEvent>(this);
            EventManager.Instance.RegisterListener<QuitGameButtonPressedEvent>(this);
            EventManager.Instance.RegisterListener<GamePadButtonDownEvent>(this);
        }

        void UnregisterEvents()
        {
            EventManager.Instance.UnregisterListener(this);
        }

        public bool Handle(IEvent evt)
        {
            if(evt is PlayGameButtonPressedEvent)
            {
                ChangeState(new UILobbyGameState(GameManager));
            }
            if(evt is OptionsButtonPressedEvent)
            {
                Console.WriteLine("Options Pressed");
            }
            if(evt is QuitGameButtonPressedEvent)
            {
                EventManager.Instance.QueueEvent(new GameShutdownEvent());
            }
            return false;
        }
    }
}
