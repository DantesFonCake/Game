using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Model
{
    public static class Levels
    {

        public static IEnumerable<string[]> AllLevels
        {
            get
            {
                yield return Level1;
                yield return Level2;
            }
        }
        public static string[] Level1 = new[] 
        { 
            "S;S;S;S;S;S;S;S;S;S;S;S;S;S;S",
            "S; ; ;Ka;Hi;Ha;Na;S; ;S; ; ; ; ;S",
            "S; ; ; ; ; ; ;S; ; ; ; ; ; ;Ex",
            "S; ; ; ; ; ; ;S; ;S; ; ; ; ;S",
            "S; ;CI;S;S;S;S;S; ; ; ; ; ; ;S",
            "S; ; ; ; ; ;E; ; ; ; ; ; ; ;S",
            "S; ; ; ;S; ; ; ; ; ; ; ; ; ;S",
            "S; ; ; ;S; ; ; ; ;S; ; ; ; ;S",
            "S; ;S;S;S;CI; ; ; ;S; ; ; ; ;S",
            "S; ;S; ; ; ; ; ; ;S; ; ; ; ;S",
            "S; ;S; ; ; ; ; ; ; ; ; ; ; ;S",
            "S; ;S;S;S;S;S;S; ; ; ; ; ; ;S",
            "S; ; ; ; ; ; ;S; ;S; ; ; ; ;S",
            "S; ; ; ; ; ; ;S; ; ; ; ; ; ;S",
            "S;S;S;S;S;S;S;S;S;S;S;S;S;S;S",
        };

        public static string[] Level2 = new[]
        {
            "S;S;S;S;S;S;S;S;S;S;S;S;S;S;S",
           "Ex; ; ;S; ; ; ; ; ; ;Ka;Hi;Ha;Na;S",
            "S; ; ;S; ; ; ; ; ; ; ; ; ; ;S",
            "S; ; ;S; ; ; ; ; ; ; ; ; ; ;S",
            "S; ;S;S;S; ; ;S;S;S;S; ; ; ;S",
            "S; ; ; ; ; ; ; ; ;S; ; ; ; ;S",
            "S; ; ; ; ; ; ; ; ;S; ; ; ; ;S",
            "S; ; ; ; ;E; ; ; ;S; ; ; ; ;S",
            "S; ; ; ; ; ; ; ; ;S; ; ; ; ;S",
            "S;S;S;S;S; ;S;S;S;S; ; ; ; ;S",
            "S;CI; ; ; ; ;S; ;CI; ; ; ; ;E;S",
            "S; ; ; ; ; ;S; ; ; ; ; ; ; ;S",
            "S; ; ; ; ; ;S; ; ; ; ; ; ; ;S",
            "S; ; ;E; ; ; ; ; ; ; ; ;E; ;S",
            "S;S;S;S;S;S;S;S;S;S;S;S;S;S;S",
        };
    }
}
