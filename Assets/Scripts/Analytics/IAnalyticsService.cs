using System.Collections.Generic;

namespace MergeShelter.Analytics
{
    public interface IAnalyticsService
    {
        void Track(string eventName, Dictionary<string, object> parameters = null);
    }
}
