using System;

namespace MergeShelter.Combat
{
    [Serializable]
    public sealed class EnemyData
    {
        public string EnemyId;
        public int MaxHealth;
        public int Damage;
        public float Speed;
    }
}
