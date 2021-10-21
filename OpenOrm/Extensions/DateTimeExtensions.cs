using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extension
    {
        /// <summary>
        /// Gets a DateTime representing the first day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime First(this DateTime current)

        {
            DateTime first = current.AddDays(1 - current.Day);
            return first;
        }

        /// <summary>
        /// Gets a DateTime representing the first specified day in the current month
        /// </summary>
        /// <param name="current">The current day</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime First(this DateTime current, DayOfWeek dayOfWeek)
        {
            DateTime first = current.First();

            if (first.DayOfWeek != dayOfWeek)
            {
                first = first.Next(dayOfWeek);
            }

            return first;
        }

        /// <summary>
        /// Gets a DateTime representing the first monday in the current week
        /// </summary>
        /// <param name="current">The current day</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime FirstMondayOfWeek(this DateTime current)
        {
            int delta = DayOfWeek.Monday - current.DayOfWeek;
            DateTime monday = current.AddDays(delta);

            return monday;
        }

        /// <summary>
        /// Gets a DateTime representing the last day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime Last(this DateTime current)
        {
            int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);

            DateTime last = current.First().AddDays(daysInMonth - 1);
            return last;
        }

        /// <summary>
        /// Gets a DateTime representing the last specified day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime Last(this DateTime current, DayOfWeek dayOfWeek)
        {
            DateTime last = current.Last();

            last = last.AddDays(Math.Abs(dayOfWeek - last.DayOfWeek) * -1);
            return last;
        }

        /// <summary>
        /// Gets a DateTime representing the first date following the current date which falls on the given day of the week
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="dayOfWeek">The day of week for the next date to get</param>
        public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek)
        {
            int offsetDays = dayOfWeek - current.DayOfWeek;

            if (offsetDays <= 0)
            {
                offsetDays += 7;
            }

            DateTime result = current.AddDays(offsetDays);
            return result;
        }

        /// <summary>
        /// Gets a DateTime representing midnight on the current date
        /// </summary>
        /// <param name="current">The current date</param>
        public static DateTime Midnight(this DateTime current)
        {
            DateTime midnight = new DateTime(current.Year, current.Month, current.Day);
            return midnight;
        }

        /// <summary>
        /// Gets a DateTime representing noon on the current date
        /// </summary>
        /// <param name="current">The current date</param>
        public static DateTime Noon(this DateTime current)
        {
            DateTime noon = new DateTime(current.Year, current.Month, current.Day, 12, 0, 0);
            return noon;
        }
        /// <summary>
        /// Sets the time of the current date with minute precision
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="hour">The hour</param>
        /// <param name="minute">The minute</param>
        public static DateTime SetTime(this DateTime current, int hour, int minute)
        {
            return SetTime(current, hour, minute, 0, 0);
        }

        /// <summary>
        /// Sets the time of the current date with second precision
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="hour">The hour</param>
        /// <param name="minute">The minute</param>
        /// <param name="second">The second</param>
        /// <returns></returns>
        public static DateTime SetTime(this DateTime current, int hour, int minute, int second)
        {
            return SetTime(current, hour, minute, second, 0);
        }

        /// <summary>
        /// Sets the time of the current date with millisecond precision
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="hour">The hour</param>
        /// <param name="minute">The minute</param>
        /// <param name="second">The second</param>
        /// <param name="millisecond">The millisecond</param>
        /// <returns></returns>
        public static DateTime SetTime(this DateTime current, int hour, int minute, int second, int millisecond)
        {
            DateTime atTime = new DateTime(current.Year, current.Month, current.Day, hour, minute, second, millisecond);
            return atTime;
        }

        public static string ToShortString(this DateTime d)
        {
            return d.ToShortDateString() + " " + d.ToShortTimeString();
        }

        public static IEnumerable<DateTime> EachDayTo(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public static IEnumerable<DateTime> EachDayFrom(this DateTime to, DateTime from)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public static string TimeAgo(this DateTime date)
        {
            TimeSpan ts = DateTime.Now - date;

            double TotalSeconds = Math.Abs(ts.TotalSeconds);
            double TotalMinutes = Math.Abs(ts.TotalMinutes);
            double TotalHours = Math.Abs(ts.TotalHours);
            double TotalDays = Math.Abs(ts.TotalDays);

            if (DateTime.Now > date)
            {
                //date passée
                if (TotalSeconds < 10)
                {
                    return "A l'instant";
                }
                else if (TotalSeconds < 60)
                {
                    return $"Il y a {TotalSeconds.ToInt()} secondes";
                }
                else if (TotalMinutes < 60)
                {
                    if (TotalMinutes.ToInt() == 1)
                        return $"Il y a une minute";
                    else
                        return $"Il y a {TotalMinutes.ToInt()} minutes";
                }
                else if (TotalHours < 24)
                {
                    if (TotalHours.ToInt() == 1)
                        return $"Il y a une heure";
                    else
                        return $"Il y a {TotalHours.ToInt()} heures";
                }
                else if (TotalDays < 30)
                {
                    if (TotalDays.ToInt() == 1)
                        return $"Il y a 1 jour";
                    else
                        return $"Il y a {TotalDays.ToInt()} jours";
                }
                else if (TotalDays >= 30 && TotalDays < 365)
                {
                    double mois = TotalDays / 30d;
                    return $"Il y a {mois.ToInt()} mois";
                }
                else
                {
                    double annees = TotalDays / 365d;
                    return $"Il y a {Math.Round(annees, 1)} ans";
                }
            }
            else
            {
                //date future
                if (TotalSeconds < 10)
                {
                    return "Maintenant";
                }
                else if (TotalSeconds < 60)
                {
                    return $"Dans {TotalSeconds.ToInt()} secondes";
                }
                else if (TotalMinutes < 60)
                {
                    if (TotalMinutes.ToInt() == 1)
                        return $"Dans une minute";
                    else
                        return $"Dans {TotalMinutes.ToInt()} minutes";
                }
                else if (TotalHours < 24)
                {
                    if (TotalHours.ToInt() == 1)
                        return $"Dans une heure";
                    else
                        return $"Dans {Math.Round(TotalHours, 1)} heures";
                }
                else if (TotalDays < 30)
                {
                    if (TotalDays.ToInt() == 1)
                        return $"Dans 1 jour";
                    else
                        return $"Dans {Math.Round(TotalDays, 1)} jours";
                }
                else if (TotalDays >= 30 && TotalDays < 365)
                {
                    double mois = TotalDays / 30d;
                    return $"Dans {mois.ToInt()} mois";
                }
                else
                {
                    double annees = TotalDays / 365d;
                    return $"Dans {Math.Round(annees, 1)} ans";
                }
            }
        }
    }
}
