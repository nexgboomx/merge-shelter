using System.Collections.Generic;
using UnityEngine;

namespace MergeShelter.Analytics
{
    /// <summary>
    /// Sprint 1 debug analytics implementation.
    /// Replace with Firebase or another analytics SDK in MVP+.
    /// </summary>
    public sealed class DebugAnalyticsService : IAnalyticsService
    {
        public void Track(string eventName, Dictionary<string, object> parameters = null)
        {
            if (parameters == null || parameters.Count == 0)
            {
                Debug.Log($"[Analytics] {eventName}");
                return;
            }

            var pairs = new List<string>();
            foreach (var pair in parameters)
                pairs.Add($"{pair.Key}={pair.Value}");

            Debug.Log($"[Analytics] {eventName}: {string.Join(", ", pairs)}");
        }
    }
}
