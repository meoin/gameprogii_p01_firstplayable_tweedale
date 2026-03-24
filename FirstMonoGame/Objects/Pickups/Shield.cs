using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Obstacles;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FirstMonoGame.Objects.Pickups;

public class Shield : Pickup
{
    public int Value;

    public Shield(Vector2 position, Sprite sprite, int value) : base(position, sprite)
    {
        Value = value;
    }

    public override void Collect(Player player)
    {
        player.Shield.Heal(Value);
        base.Collect(player);
    }
}