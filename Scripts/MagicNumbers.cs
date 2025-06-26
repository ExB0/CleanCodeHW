using System;
namespace Scripts
{
    class Weapon
    {
        private const int BulletPerShot = 1;

        private int _bullets;

        public bool CanShoot() => _bullets >= BulletPerShot;

        public void Shoot()
        {
            if (CanShoot())
            {
                _bullets -= BulletPerShot;
            }
        }
    }
}

