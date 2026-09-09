using UnityEngine;

namespace WorldSeed
{
    public class DeterministicHash
    {
        private const uint FnvOffsetBasis = 2166136261;
        private const uint FnvPrime = 16777619;

        public static int GetStringHash(string str)
        {
            if(string.IsNullOrEmpty(str))
                return 0;
            uint hash = FnvOffsetBasis;
            foreach (char ch in str)
            {
                hash ^= ch;
                hash *= FnvPrime;
            } 
            return unchecked((int)hash);
        }

        public static int Combine(int seed, int value)
        {
            uint uSeed = unchecked((uint)seed);
            uint uValue = unchecked((uint)value);
            uSeed ^= uValue + 0x9e3779b9 + (uSeed << 6) + (uSeed >> 2);
            return unchecked((int)uSeed);
        }

        public static int Combine(int v1, int v2, int v3)
        {
            int hash = Combine(v1, v2);
            return Combine(hash, v3);
        }
    }
}