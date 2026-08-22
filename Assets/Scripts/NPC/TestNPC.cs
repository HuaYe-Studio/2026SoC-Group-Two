using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    public class TestNPC: MonoBehaviour
    {
        [Tooltip("开始对话的节点")] public DialogueNodeSO dialogueNode;
        [SerializeField]private float interactRange = 2.5f;
        [SerializeField]private GameObject player;
        
        private bool isInRange;

        private void Start()
        {
            if(player == null)
                player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            CheckIsInRange();
            UpdatePromptText();
            TryStartDialogue();
        }

        private void CheckIsInRange()
        {
            if (player == null)
            {
                isInRange = false;
                return;
            }
            
            float sqrDistance =(player.transform.position - transform.position).sqrMagnitude;
            isInRange = sqrDistance <= interactRange*interactRange;
        }

        private void TryStartDialogue()
        {
            if(!isInRange)
                return;
            if(dialogueNode == null)
                return;
            if(DialogueManager.Instance == null || DialogueManager.Instance.IsDialogueActive)
                return;

            if (Keyboard.current?[KeyManager.Instance.player_Interact_key].wasPressedThisFrame ?? false)
                DialogueManager.Instance.StartDialogue(dialogueNode,player,gameObject);
        }

        private void UpdatePromptText()
        {
            
            bool shouldShow = isInRange && !(DialogueManager.Instance?.IsDialogueActive ?? false);
            if(DialogueUI.Instance !=null)
                DialogueUI.Instance.ShowPromptText(shouldShow);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}