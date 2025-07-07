
using System;
namespace Scripts
{
    class Player
    {
        public string Name { get; private set; }
        public int Age { get; private set; }
    }

    class Weapon
    {
        public int WeaponDamage { get; private set; }
        public float WeaponCooldown { get; private set; }
        public Weapon(int damage, int coldown)
        {
            if (damage <= 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "Урон не может быть отрицательным или равен 0");

            if (coldown <= 0)
                throw new ArgumentOutOfRangeException(nameof(coldown), "Перезарядка не может быть отрицательным или равен 0");

            WeaponDamage = damage;
            WeaponCooldown = coldown;
        }
    }

    class PlayerMovement
    {
        public float MovementSpeed { get; private set; }
        public float MovementDirectionX { get; private set; }
        public float MovementDirectionY { get; private set; }

        public void Move()
        {
            //Do move
        }
    }

    class PlayerCombat
    {
        private Weapon _weapon = new Weapon(5, 10);

        public void Attack()
        {
            //attack
        }

        public bool IsReloading()
        {
            throw new NotImplementedException();
        }
    }

}

