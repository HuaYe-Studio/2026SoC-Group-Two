using UnityEngine;
using System;

namespace NPC
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private DialogueNodeSO currentNode;
        private DialogueContext currentContext;
        private int currentLineIndex = 0;
        
        private DialogueOption[] availableOptions;
        
        public bool IsDialogueActive { get; private set; } = false;
        
        //提供给UI
        public event Action OnDialogueStarted;
        public event Action<DialogueLine> OnLineChanged;
        public event Action<DialogueOption[]> OnOptionsRequested;
        public event Action OnDialogueEnded;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void StartDialogue(DialogueNodeSO startNode , GameObject initiator,GameObject targetNPC)
        {
            if(IsDialogueActive)
                return;
            
            IsDialogueActive = true;
            if(GameFlowManager.Instance!=null)
                GameFlowManager.Instance.PauseGameplay("Dialogue");

            currentContext = new DialogueContext
            {
                Initiator = initiator,
                Target = targetNPC
            };

            currentNode = startNode;
            currentLineIndex = 0;
            
            OnDialogueStarted?.Invoke();
            DisplayNextLine();
        }
        
        public void DisplayNextLine()
        {
            if(!IsDialogueActive || currentNode == null)
                return;

            if (currentLineIndex < currentNode.lines.Length)
            {
                DialogueLine nextLine = currentNode.lines[currentLineIndex];
                currentLineIndex++;
                
                OnLineChanged?.Invoke(nextLine);
            }
            else
            {
                if (currentNode.options != null && currentNode.options.Length > 0)
                {
                    availableOptions = GetAvailableOptions();
                    if (availableOptions.Length == 0)                                                                                                                                                
                    {                                                                                                                                                                                
                        EndDialogue();                                                                                                                  
                        return;                                                                                                                                                                      
                    }
                    OnOptionsRequested?.Invoke(availableOptions);
                }
                else
                {
                    EndDialogue();
                }
            }
        }

        public void EndDialogue()
        {
            if(!IsDialogueActive)
                return;
            
            if(GameFlowManager.Instance != null)
                GameFlowManager.Instance.ResumeGameplay("Dialogue");
            
            IsDialogueActive = false;
            currentNode = null;
            currentContext = null;
            
            OnDialogueEnded?.Invoke();
        }

        public void SelectOption(int optionIndex)
        {
            if(!IsDialogueActive || currentNode == null||availableOptions==null)
                return;
            
            if (optionIndex < 0 || optionIndex >= availableOptions.Length) 
                return;
            
            DialogueOption selectedOption = availableOptions[optionIndex];

            if (selectedOption.onSelect != null)
            {
                foreach (var action in selectedOption.onSelect)
                {
                    action.Execute(currentContext);
                }
            }

            if (selectedOption.nextNode != null)
            {
                currentNode = selectedOption.nextNode;
                currentLineIndex = 0;
                DisplayNextLine();
            }
            else
            {
                EndDialogue();
            }
        }
        
        private DialogueOption[] GetAvailableOptions()
        {
            if (currentNode.options == null || currentNode.options.Length == 0)
                return Array.Empty<DialogueOption>();

            return Array.FindAll(currentNode.options, IsOptionAvailable);
        }
        
        private bool IsOptionAvailable(DialogueOption option)
        {
            if (option.conditions == null || option.conditions.Length == 0)
                return true;  

            foreach (DialogueConditionSO condition in option.conditions)
            {
                if (!condition.Evaluate(currentContext))
                    return false;   
            }
            return true;
        }
    }
}