using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Event.Application.Helpers
{
    public class Util
    {
        /// <summary>
        /// check if an email provided is valid
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            if (email != null)
            {
                return Regex.IsMatch(email, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                    && Regex.IsMatch(email, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
            }

            return false;
        }

        public static string GenerateRandomDigits(int length)
        {
            Random random = new();
            const string chars = "0123456789";
            string result = new(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }

        public static string GenerateRandomString(int length, bool specialChar = false)
        {
            Random random = new();

            string characters = specialChar ? "@$ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz@_!$" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            string result = new(Enumerable.Repeat(characters, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return result.ToUpper();
        }

        public static string RelativeTime(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);
            if (delta < 1 * MINUTE) return ts.TotalSeconds == 1 ? "one second ago" : $"{ts.Seconds} seconds ago";

            if (delta < 2 * MINUTE) return "a minute ago";

            if (delta < 45 * MINUTE) return $"{ts.Minutes} minutes ago";

            if (delta < 90 * MINUTE) return "an hour ago";

            if (delta < 24 * HOUR) return $"{ts.Hours} hours ago";

            if (delta < 48 * HOUR) return "yesterday";

            if (delta < 30 * DAY) return $"{ts.Days} days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : $"{months} months ago";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : $"{years} years ago";
        }


    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

    }

}
