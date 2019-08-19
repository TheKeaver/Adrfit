﻿using Microsoft.Xna.Framework.Input;

namespace GameJam.Input
{
    public class PrimaryKeyboardInputMethod : InputMethod
    {
        public override void Update(float dt)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if(keyboardState.IsKeyDown((Keys)CVars.Get<int>("keyboard_primary_clockwise")))
            {
                // Clockwise
                _snapshot.Angle -= CVars.Get<float>("keyboard_shield_angular_speed") * dt;
            }
            if(keyboardState.IsKeyDown((Keys)CVars.Get<int>("keyboard_primary_counter_clockwise")))
            {
                // Counter-clockwise
                _snapshot.Angle += CVars.Get<float>("keyboard_shield_angular_speed") * dt;
            }
        }
    }
}
