using System;

namespace UrlShortener.Utilities
{
    public class Clock
    {
        public virtual DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }
}