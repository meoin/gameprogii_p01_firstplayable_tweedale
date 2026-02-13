using System;
using System.Diagnostics;

namespace FirstMonoGame.Objects;

public class Health
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public Health(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public Health(int maxHealth, int currentHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
    }

    public int TakeDamage(int damage)
    {
        if (damage >= 0)
        {
            int spillover = Math.Max(0, damage - CurrentHealth);
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
            return spillover;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Damage cannot be less than 0!");
            Console.ForegroundColor = ConsoleColor.White;
            return 0;
        }

    }

    public void Restore()
    {
        CurrentHealth = MaxHealth;
    }

    public void Heal(int healing)
    {
        if (healing >= 0)
        {
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + healing);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Healing cannot be less than 0!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}