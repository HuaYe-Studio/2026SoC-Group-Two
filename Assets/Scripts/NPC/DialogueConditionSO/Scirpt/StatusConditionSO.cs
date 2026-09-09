using Status;
using UnityEngine;

namespace NPC
{
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