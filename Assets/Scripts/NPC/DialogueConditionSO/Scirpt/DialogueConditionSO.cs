using UnityEngine;
using System;
using System.Collections.Generic;

namespace NPC
{
    public abstract class DialogueConditionSO: ScriptableObject
    {
        public abstract bool Evaluate(DialogueContext context);
    }
    
    public static class GameFlags
    {
        private static HashSet<string> flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);                                                                                                    
                                                                                                                                                                                   
        public static void Set(string key)    => flags.Add(key);                                                                                                       
        public static void Clear(string key)  => flags.Remove(key);                                                                                                         
        public static bool Has(string key)    => flags.Contains(key);                                                                                                 
    }
    
    public enum CompareType { Greater, GreaterEqual, Equal, Less, LessEqual, NotEqual }
}