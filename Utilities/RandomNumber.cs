using System;

namespace Akiled.Utilities
{
    public static class RandomNumber
    {
        private static readonly Random r = new Random();
        private static readonly Object l = new Object();

        private static readonly Random globalRandom = new Random();
        [ThreadStatic]
        private static Random localRandom;

        public static int GenerateNewRandom(int min, int max)
        {
            return new Random().Next(min, max + 1);
        }

        public static int GenerateLockedRandom(int min, int max)
        {
            lock (l)
            {
                return r.Next(min, max);
            }
        }

        public static int GenerateRandom(int min, int max)
        {
            Random inst = localRandom;

            max++;

            if (inst == null)
            {
                int seed;
                lock (globalRandom)
                {
                    seed = globalRandom.Next();
                }
                localRandom = inst = new Random(seed);
            }

            return inst.Next(min, max);
        }
    }
}