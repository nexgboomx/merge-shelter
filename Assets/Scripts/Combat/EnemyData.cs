using System;
using System.Collections.Generic;
using System.Text;

namespace MergeShelter.Combat
{
    [Serializable]
    public sealed class EnemyData
    {
        public string EnemyId;
        public string DisplayName;
        public string BehaviorTag;
        public int MaxHealth;
        public int Damage;
        public float Speed;

        public static string FormatWaveRoster(IReadOnlyList<EnemyData> enemies)
        {
            if (enemies == null || enemies.Count == 0)
                return "No enemies.";

            var groups = new List<RosterEntry>();
            foreach (var enemy in enemies)
            {
                if (enemy == null)
                    continue;

                var existing = groups.Find(g => g.DisplayName == GetDisplayName(enemy));
                if (existing != null)
                    existing.Count++;
                else
                    groups.Add(new RosterEntry(GetDisplayName(enemy), GetBehaviorTag(enemy)));
            }

            var builder = new StringBuilder("Wave: ");
            for (var i = 0; i < groups.Count; i++)
            {
                if (i > 0)
                    builder.Append(" · ");

                var entry = groups[i];
                if (entry.Count > 1)
                {
                    builder.Append(entry.Count);
                    builder.Append("× ");
                }

                builder.Append(entry.DisplayName);

                if (!string.IsNullOrEmpty(entry.Tag))
                {
                    builder.Append(" (");
                    builder.Append(entry.Tag);
                    builder.Append(')');
                }
            }

            return builder.ToString();
        }

        private static string GetDisplayName(EnemyData enemy)
        {
            return string.IsNullOrWhiteSpace(enemy.DisplayName) ? enemy.EnemyId : enemy.DisplayName;
        }

        private static string GetBehaviorTag(EnemyData enemy)
        {
            return enemy.BehaviorTag ?? string.Empty;
        }

        private sealed class RosterEntry
        {
            public string DisplayName;
            public string Tag;
            public int Count;

            public RosterEntry(string displayName, string tag)
            {
                DisplayName = displayName;
                Tag = tag;
                Count = 1;
            }
        }
    }
}
