using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicStream.Extensions
{

    public static class Extension
    {
        static Random rnd = new Random();

        public static IEnumerable<T> PickRandom<T>(this IList<T> source, int count)
        {
            return source.OrderBy(x => rnd.Next()).Take(count);
        }
    }
}
