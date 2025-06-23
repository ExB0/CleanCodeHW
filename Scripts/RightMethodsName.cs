using System;
namespace Scripts
{
    class PlayerData { }
    class GunController { }
    class TargetFollower { }
    class UnitsCollection
    {
        public IReadOnlyCollection<Unit> Units { get; private set; }
    }
}

