using UnityEngine;

namespace NPC
{
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
}