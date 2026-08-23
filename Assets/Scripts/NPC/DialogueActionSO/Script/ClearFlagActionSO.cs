using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(menuName = "Dialogue/Actions/ClearFlag")]                                                                                                                         
    public class ClearFlagActionSO : DialogueActionSO                                                                                                                                  
    {                                                                                                                                                                                
        public string flagName;                                                                                                                                                      
        public override void Execute(DialogueContext ctx)                                                                                                                            
        { 
            GameFlags.Clear(flagName);                                                                                                        
        }                                                                                                                                                                            
    }
}