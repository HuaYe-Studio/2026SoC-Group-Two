using System;
using Status;
using UnityEngine;


namespace NPC
{
    //该类用于实现对话里的动作
    public abstract class DialogueActionSO : ScriptableObject
    {
        public abstract void Execute(DialogueContext context);
    }
    
    [CreateAssetMenu(menuName = "Dialogue/Actions/SetFlag")]                                                                                                                         
    public class SetFlagActionSO : DialogueActionSO                                                                                                                                  
    {                                                                                                                                                                                
        public string flagName;                                                                                                                                                      
        public override void Execute(DialogueContext ctx)                                                                                                                            
        { 
            GameFlags.Set(flagName);                                                                                                        
        }                                                                                                                                                                            
    }
    
    [CreateAssetMenu(menuName = "Dialogue/Actions/ClearFlag")]                                                                                                                         
    public class ClearFlagActionSO : DialogueActionSO                                                                                                                                  
    {                                                                                                                                                                                
        public string flagName;                                                                                                                                                      
        public override void Execute(DialogueContext ctx)                                                                                                                            
        { 
            GameFlags.Clear(flagName);                                                                                                        
        }                                                                                                                                                                            
    }
    
    //实现示例,简陋的修改属性
    [CreateAssetMenu(menuName = "Dialogue/Actions/ChangeStatus")]
    public class ChangeStatusActionSO : DialogueActionSO
    {
        public StatusType targetStatus;
        public float amount;
        
        public override void Execute(DialogueContext context)
        {
            //id暂且约定为system.action
            DialogueEvents.Raise("status.change",(targetStatus, amount));
        }
    }

    [CreateAssetMenu(menuName = "Dialogue/Actions/OpenContainer")]
    public class OpenContainerActionSO : DialogueActionSO
    {
        public override void Execute(DialogueContext context)
        {
            DialogueEvents.Raise("container.open");
        }
    }
}