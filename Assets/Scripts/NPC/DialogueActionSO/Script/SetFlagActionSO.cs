using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(menuName = "Dialogue/Actions/SetFlag")]                                                                                                                         
    public class SetFlagActionSO : DialogueActionSO                                                                                                                                  
    {                                                                                                                                                                                
        public string flagName;                                                                                                                                                      
        public override void Execute(DialogueContext ctx)                                                                                                                            
        { 
            GameFlags.Set(flagName);                                                                                                        
        }                                                                                                                                                                            
    }
}