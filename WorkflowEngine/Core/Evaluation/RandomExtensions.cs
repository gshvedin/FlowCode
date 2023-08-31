using System;
using System.Linq;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace WorkflowEngine.Core.Evaluation
{
    [ExcludeFromCodeCoverage]
    internal static class RandomExtensions
    {
        /// <summary>
        /// Set instead keys random values: ${{string:5}},${{int:12}},${{int:1-100}},${{dateTime:1d}},${{dateTime:2d-7d}},${{dateTime:2h-2d}}
        /// </summary>
        /// <param name="pattern">Pattern for generating random value</param>
        /// <returns>Random value</returns>
        internal static string GetRandom(string pattern)
        {
            _ = pattern ?? throw new ArgumentNullException(nameof(pattern));

            string[] keyVal = pattern.Split(":");

            if (keyVal.Length != 2)
            {
                throw new ArgumentException($"Incorrect argument for Random function. Should be typeName:param");
            }

            string rundomVal = keyVal[0].ToLower() switch
            {
                "long" => RandomDigits(keyVal[1]),
                "int" => RandomDigits(keyVal[1]),
                "string" => RandomString(int.Parse(keyVal[1])),
                "ip" => RandomIp(),
                "datetime" => RandomDate(keyVal[1]).ToString("yyyy-MM-ddTHH:mm:ss"),
                _ => throw new ArgumentException($"Incorrect argument for typeName of random function. Possible values:long,int,string,ip,datetime.")
            };

            return rundomVal;
        }

        private static string RandomDigits(string pattern)
        {
            string newDigits = string.Empty;
            Random random = new Random();
            Regex rx = new Regex(@"(^\d+)-(\d+$)");

            if (rx.IsMatch(pattern))
            {
                // digit between min-max
                GroupCollection key = rx.Match(pattern).Groups;

                int min = int.Parse(key[1].Value, CultureInfo.CurrentCulture);
                int max = int.Parse(key[2].Value, CultureInfo.CurrentCulture);

                newDigits = random.Next(min, max).ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                // certain length
                int digitLength = int.Parse(pattern, CultureInfo.CurrentCulture);
                for (int i = 0; i < digitLength; i++)
                {
                    newDigits += random.Next(10);
                }
            }

            return newDigits;
        }

        private static string RandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random random = new Random();

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static DateTime RandomDate(string timeDiapasone)
        {
            DateTime endDate = DateTime.Now;
            string startTimeDiapasone = timeDiapasone;

            Regex rx = new Regex(@"(^\w+)-(\w+$)");
            if (rx.IsMatch(timeDiapasone))
            {
                GroupCollection key = rx.Match(timeDiapasone).Groups;
                startTimeDiapasone = key[2].Value;
                endDate = DateTime.Now.AddHours(-1 * GetHoursRange(key[1].Value));
            }

            double startHoursDiapasone = GetHoursRange(startTimeDiapasone);

            Random randomTest = new Random();
            DateTime startDate = DateTime.Now.AddHours(-1 * startHoursDiapasone);
            TimeSpan timeSpan = timeDiapasone.StartsWith("-") ? new TimeSpan(0, 10, 0) : endDate - startDate;
            TimeSpan newSpan = new TimeSpan(0, randomTest.Next(5, (int)timeSpan.TotalMinutes), 0);

            return (startDate + newSpan).ToUniversalTime();
        }

        private static string RandomIp()
        {
            byte[] bytes = new byte[4];
            new Random().NextBytes(bytes);
            IPAddress ipv6Address = new IPAddress(bytes);

            return ipv6Address.ToString();
        }

        private static double GetHoursRange(string timeRange)
        {
            int diadasone = int.Parse(Regex.Replace(timeRange, @"[^\d]", string.Empty));

            return timeRange.ToLower() switch
            {
                string s when s.Contains("h") => 1 * diadasone,
                string s when s.Contains("d") => 24 * diadasone,
                _ => throw new NotImplementedException($"Error parsing key {timeRange}")
            };
        }
    }
}
