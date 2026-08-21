using UnityEngine;
using System;
using System.Collections.Generic;
using Status;

namespace NPC
{
    public static class GameFlags
    {
        private static HashSet<string> flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);                                                                                                    
                                                                                                                                                                                   
        public static void Set(string key)    => flags.Add(key);                                                                                                       
        public static void Clear(string key)  => flags.Remove(key);                                                                                                         
        public static bool Has(string key)    => flags.Contains(key);                                                                                                 
    }
    
    public enum CompareType { Greater, GreaterEqual, Equal, Less, LessEqual, NotEqual }
    
    public abstract class DialogueConditionSO: ScriptableObject
    {
        public abstract bool Evaluate(DialogueContext context);
    }
    
    //用于字符串
     [CreateAssetMenu(menuName = "Dialogue/Conditions/Flag")] 
     public class FlagConditionSO : DialogueConditionSO                                                                                                                              
     {                                                                                                                                                                                
         public string flagName;                                                                                                                                                      
         public bool requireSet = true;                                                                                         

         public override bool Evaluate(DialogueContext context)
         {
             bool isSet = GameFlags.Has(flagName);                                                                                                                                      
             return requireSet ? isSet : !isSet;
         }
     }                                                                                                                                                                                
     
     //用于数值的示例
     [CreateAssetMenu(menuName = "Dialogue/Conditions/Status")]
     public class StatusConditionSO : DialogueConditionSO                                                                                                                            
     {                                                                                                                                                                                
         public StatusType targetStatus;                                                                                                 
         public CompareType compare;                                                                                                                             
         public float value;                                                                                                                                                
                                                                                                                                                                                   
         public override bool Evaluate(DialogueContext ctx)                                                                                                                           
         {                                                                                                                                                                            
             float currentStatusValue = StatusManager.Instance.GetStatusValue(targetStatus);
          
             return compare switch                                                                                                                                                    
             {                                                                                                                                                                        
                 CompareType.Greater       => currentStatusValue >  value,                                                                                                                       
                 CompareType.GreaterEqual  => currentStatusValue >= value,                                                                                                                       
                 CompareType.Equal         => Mathf.Approximately(currentStatusValue, value),                                                                                                    
                 CompareType.Less          => currentStatusValue <  value,                                                                                                                       
                 CompareType.LessEqual     => currentStatusValue <= value,                                                                                                                       
                 CompareType.NotEqual      => !Mathf.Approximately(currentStatusValue, value),                                                                                                   
                 _ => false                                                                                                                                                           
             };                                                                                                                                                                       
         }                                                                                                                                                                            
     }
}