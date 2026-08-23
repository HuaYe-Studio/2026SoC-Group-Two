using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(menuName = "Dialogue/Actions/OpenContainer")]
    public class OpenContainerActionS0 : DialogueActionSO
    {
        public override void Execute(DialogueContext context)
        {
            DialogueEvents.Raise("container.open");
        }
    }
}