using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using UI;

namespace NPC
{
    public class DialogueUI:MonoBehaviour
    {
        public static DialogueUI Instance{get; private set;}
        
        [Header("UI面板")]
        public GameObject dialoguePanel;
        public TMP_Text speakerNameText;
        public TMP_Text dialogueText;
        public Image avatarImage;
        public Button dialogueContinueButton;
        
        [Header("选项")]
        public Transform optionContainer;
        public GameObject optionButtonTemplate;
        
        [Header("提示")]
        //按E交互的提示文本
        public TMP_Text interactText;
        private string interactPreText = "按";
        private string interactBackText = "对话";
        
        //点击继续的提示文本
        public GameObject continueText;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if(dialogueContinueButton != null)
                dialogueContinueButton.onClick.AddListener(ContinueDialogue);
        }

        private void Start()
        {
            if(DialogueManager.Instance == null)
                return;

            DialogueManager.Instance.OnDialogueStarted -= HandleDialogueStart;
            DialogueManager.Instance.OnLineChanged -= HandleLineChange;
            DialogueManager.Instance.OnOptionsRequested -= HandleOptionsRequested;
            DialogueManager.Instance.OnDialogueEnded -= HandleDialogueEnd;
            
            DialogueManager.Instance.OnDialogueStarted += HandleDialogueStart;
            DialogueManager.Instance.OnLineChanged += HandleLineChange;
            DialogueManager.Instance.OnOptionsRequested += HandleOptionsRequested;
            DialogueManager.Instance.OnDialogueEnded += HandleDialogueEnd;
        }

        private void OnDisable()
        {
            if(DialogueManager.Instance == null)
                return;

            DialogueManager.Instance.OnDialogueStarted -= HandleDialogueStart;
            DialogueManager.Instance.OnLineChanged -= HandleLineChange;
            DialogueManager.Instance.OnOptionsRequested -= HandleOptionsRequested;
            DialogueManager.Instance.OnDialogueEnded -= HandleDialogueEnd;
        }
        
        private void HandleDialogueStart()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                UIManager.Instance.OpenUI(dialoguePanel);
            }
            ClearOptions();
        }

        private void HandleLineChange(DialogueLine line)
        {
            if(speakerNameText != null)
                speakerNameText.text = line.speakerName;
            if (dialogueText != null)
                dialogueText.text = line.text;
            if (avatarImage != null)
            {
                avatarImage.sprite = line.npcSprite;
                avatarImage.gameObject.SetActive(line.npcSprite != null);
            }
            
            if(optionContainer != null)
                optionContainer.gameObject.SetActive(false);
            if(continueText!=null)
                continueText.SetActive(true);
            if (dialogueContinueButton != null)
                dialogueContinueButton.interactable = true;
        }

        private void HandleOptionsRequested(DialogueOption[] options)
        {
            if(continueText != null)
                continueText.SetActive(false);
            if(dialogueContinueButton != null)
                dialogueContinueButton.interactable = false;
            
            ClearOptions();
            if(optionContainer == null||optionButtonTemplate == null)
                return;
            
            optionContainer.gameObject.SetActive(true);
            for (int i = 0; i < options.Length; i++)
            {
                int index = i;
                
                GameObject newButton = Instantiate(optionButtonTemplate, optionContainer);
                newButton.SetActive(true);
                
                TMP_Text labelText = newButton.GetComponentInChildren<TMP_Text>();
                if (labelText != null)
                    labelText.text = options[i].optionText;
                
                Button button = newButton.GetComponent<Button>();
                if(button != null)
                    button.onClick.AddListener(() =>SelectDialogueOption(index));
            }
        }

        private void HandleDialogueEnd()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
                UIManager.Instance.CloseUI(dialoguePanel);
            }
            ClearOptions();
        }
        
        private void ClearOptions()
        {
            if (optionContainer == null)
                return;

            for (int i = optionContainer.childCount - 1; i >= 0; i--)
            {
                if(optionContainer.GetChild(i).gameObject == optionButtonTemplate)
                    continue;
                Destroy(optionContainer.GetChild(i).gameObject);
            }
        }

        private void SelectDialogueOption(int index)
        {
            if(DialogueManager.Instance != null)
                DialogueManager.Instance.SelectOption(index);
        }
        
        public void ShowPromptText(bool isShow)
        {
            if(interactText == null)
                return;
            if(KeyManager.Instance == null)
                return;
            interactText.gameObject.SetActive(isShow);
            if (isShow)
                interactText.text = $"{interactPreText}{KeyManager.Instance.player_Interact_key}{interactBackText}";
        }

        public void ContinueDialogue()
        {
            if(DialogueManager.Instance != null)
                DialogueManager.Instance.DisplayNextLine();
        }
    }
}