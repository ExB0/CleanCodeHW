using System;
namespace Scripts
{
    class Player
    {
        private Mover _mover;
        private Weapon _weapon;

        public Player(string name, int age, Weapon weapon, Mover mover)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Нет имени");

            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age), "Возраст не может быть меньше 0");

            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon), "Нет оружия");

            if (mover == null)
                throw new ArgumentNullException(nameof(mover), "Нет системы ходьбы");

            Name = name;
            Age = age;
            _mover = mover;
            _weapon = weapon;
        }
        
        public string Name { get; private set; }
        public int Age { get; private set; }

    }

    class Weapon
    {
        public Weapon(int damage, int cooldown)
        {
            if (damage <= 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "Урон не может быть отрицательным или равен 0");

            if (cooldown <= 0)
                throw new ArgumentOutOfRangeException(nameof(cooldown), "Перезарядка не может быть отрицательным или равен 0");

            WeaponDamage = damage;
            WeaponCooldown = cooldown;
        }

        public int WeaponDamage { get; private set; }
        public float WeaponCooldown { get; private set; }

        public void Attack()
        {
            //attack
        }

        public bool IsReloading()
        {
            throw new NotImplementedException();
        }
    }

    class Mover
    {
        public Mover(float speedValue, float xValue, float yValue)
        {
            if (speedValue < 0)
                throw new ArgumentOutOfRangeException(nameof(speedValue), "Число не может быть меньше 0");

            if (xValue < 0)
                throw new ArgumentOutOfRangeException(nameof(xValue), "Число не может быть меньше 0");

            if (yValue < 0)
                throw new ArgumentOutOfRangeException(nameof(yValue), "Число не может быть меньше 0");

            MovementSpeed = speedValue;
            MovementDirectionX = xValue;
            MovementDirectionY = yValue;
        }

        public float MovementSpeed { get; private set; }
        public float MovementDirectionX { get; private set; }
        public float MovementDirectionY { get; private set; }

        public void Move()
        {
            //Do move
        }
    }
}

