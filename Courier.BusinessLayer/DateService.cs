using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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

        static void Main(string[] args)
        {
            DateTime lastProcessed = DateTime.Now.Date;




            while (true)
            {

                var a = currentTime();
                var k = a.Date;

                if (lastProcessed != k)
                {
                    lastProcessed = k;
                    Console.WriteLine($"Hello World!  {a}  {a.Date}");
                }
                else
                {
                    Console.WriteLine($"alredy prcseedd at {lastProcessed}");
                }

                Thread.Sleep(1000);

            }

        }

    }
}
