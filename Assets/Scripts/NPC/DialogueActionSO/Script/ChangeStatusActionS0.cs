using Status;
using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(menuName = "Dialogue/Actions/ChangeStatus")]
    public class ChangeStatusActionS0 : DialogueActionSO
    {
        public StatusType targetStatus;
        public float amount;
        
        public override void Execute(DialogueContext context)
        {
            //id暂且约定为system.action
            DialogueEvents.Raise("status.change",(targetStatus, amount));
        }
    }
}