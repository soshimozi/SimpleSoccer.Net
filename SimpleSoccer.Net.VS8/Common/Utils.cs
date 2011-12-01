using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    public static class Utils
    {
        public static class Math
        {
            public static class Constants
            {
                public static double Pi = 3.14159;
                public static double TwoPi = Pi * 2;
                public static double HalfPi = Pi / 2;
                public static double QuarterPi = Pi / 4;
            }

            public static double RandomInRange(Random randomGenerator, double low, double high)
            {
                return low + randomGenerator.NextDouble() * (high - low);
            }

            public static int RandomInt(Random randomGenerator, int low, int high)
            {
                return randomGenerator.Next(low, high);
            }

            public static double RandomClamped(Random randomGenerator)
            {
                return randomGenerator.NextDouble() - randomGenerator.NextDouble();
            }
        }
    }
}
