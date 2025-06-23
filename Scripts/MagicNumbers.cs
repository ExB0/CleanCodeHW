using System;
namespace Scripts
{
    class Weapon
    {
        private  int _bullets;
        private const int _bulletPerShot = 1;

        public bool CanShoot() => _bullets > 0;

        public void Shoot() => _bullets -= _bulletPerShot;
    }
}

