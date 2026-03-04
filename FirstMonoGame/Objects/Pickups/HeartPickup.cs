using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Obstacles;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FirstMonoGame.Objects.Pickups;

public class HeartPickup : Pickup
{
    public int Value;

    public HeartPickup(Vector2 position, Sprite sprite, int value) : base(position, sprite)
    {
        Value = value;
    }
}