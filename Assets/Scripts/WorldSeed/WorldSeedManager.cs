using System;
using UnityEditor;
using UnityEngine;

namespace WorldSeed
{
    public class WorldSeedManager: MonoBehaviour
    {
        public static WorldSeedManager Instance{get; private set;}
        
        public string CurrentStringSeed{get; private set;}
        public int CurrentNumSeed{get; private set;}
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void InitializeWorldSeed(string inputSeed = "")
        {
            if (string.IsNullOrEmpty(inputSeed))
            {
                CurrentNumSeed = Guid.NewGuid().GetHashCode();
                CurrentStringSeed = CurrentNumSeed.ToString();
            }
            else
            {
                CurrentStringSeed = inputSeed;
                CurrentNumSeed= DeterministicHash.GetStringHash(inputSeed);
            }
            Debug.Log($"世界种子生成，字符串为{CurrentStringSeed},数字为{CurrentNumSeed}");
        }

        public System.Random GetRNGForSystem(string system)
        {
            int nameHash = DeterministicHash.GetStringHash(system);
            int derivedSeed = DeterministicHash.Combine(CurrentNumSeed, nameHash);
            
            return new System.Random(derivedSeed);
        }

        public System.Random GetRNGForChunk(int chunkX, int chunkY)
        {
            int derivedSeed = DeterministicHash.Combine(CurrentNumSeed, chunkX, chunkY);
            
            return new System.Random(derivedSeed);
        }
    }
}