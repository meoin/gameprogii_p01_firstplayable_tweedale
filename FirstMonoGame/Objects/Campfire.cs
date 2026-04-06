using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using FirstMonoGame.Objects.MoveStrategies;

namespace FirstMonoGame.Objects.Enemies;

public class Campfire : Obstacle
{
    public const int HEALING_RANGE = 50;

    public Campfire(Sprite sprite, Vector2 position) : base(sprite, position) {}

    
}