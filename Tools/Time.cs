using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lilguy.Tools;

public class Time
{
    public int hour { get; set; } = 0;
    public int minute { get; set; } = 0;

    public Time() { }

    public Time(int hour, int minute)
    {
        this.hour = hour;
        this.minute = int.Clamp(minute, 0, 59);
    }

    public Time(string time)
    {
        string[] splittedTime = time.Split(':');
        this.hour = int.Parse(splittedTime[0]);
        this.minute = int.Parse(splittedTime[1]);
    }

    public DateTime SetTime(DateTime dateTime)
    {
        DateTime newDateTime = dateTime.Date;

        newDateTime = newDateTime.AddHours(hour);
        newDateTime = newDateTime.AddMinutes(minute);

        return newDateTime;
    }
}
