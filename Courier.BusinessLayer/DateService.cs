using System;

namespace Courier.BusinessLayer
{
    class DateService
    {
        public static DateTime currentTime()
        {
            var start = new DateTime(2020, 11, 1);
            var now = DateTime.Now;

            var timeSpan = now - start;
            var totalSeconds = timeSpan.TotalSeconds * 60 * 60;

            var newTimeSpan = TimeSpan.FromSeconds(totalSeconds);

            var currentDate = start.Add(newTimeSpan);

            return currentDate;
        }
    }
}
