using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.BusinessLayer
{
    public interface ITimeService
    {
        DateTime currentTime();
    }
    public class TimeService : ITimeService
    {
        public DateTime currentTime()
        {
            var start = new DateTime(2020, 11, 1);
            var now = DateTime.Now;

            var different = now - start;
            var timeSpan =  different.TotalSeconds * 60 *60;

            var newTimeSpan = TimeSpan.FromSeconds(timeSpan);

            var currentDate = start.Add(newTimeSpan);

            return currentDate;
        }
    }
}
