using System;
namespace Scripts
{
    class PlayerProgress { }
    class GunShooter { }
    class UnitChaser { }
    class Squad
    {
        public IReadOnlyCollection<Unit> Units { get; private set; }
    }
}

