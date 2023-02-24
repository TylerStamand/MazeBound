using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;
using System.Text.RegularExpressions;

public class DialogManager : MonoBehaviour {

    [SerializeField] Dialog testDialog;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] float typingSpeed;
    [SerializeField] GameObject choiceButtonsPrefab;

    public event Action<bool> OnDialogComplete;



    Queue<string> sentenceQueue;
    Dialog dialog;
    bool showChoice;
    bool choiceMade;
    PlayerController playerController;

    void Awake() {
        sentenceQueue = new Queue<string>();
        //SetDialog(testDialog, "Test");
        playerController = FindObjectOfType<PlayerController>();
        playerController.OnClick += HandlePlayerClick;
    }

    public void SetDialog(Dialog dialog, string name) {
        this.dialog = dialog;
        dialogText.text = name;
        showChoice = dialog.IsChoice;
        StartDialog(dialog.Sentences);
    }

    public void StartDialog(List<String> sentences) {

        sentenceQueue.Clear();
        dialog.Sentences.ForEach(x => sentenceQueue.Enqueue(x));
        StartCoroutine(DisplayDialog());

    }

    IEnumerator DisplayDialog() {

        yield return StartCoroutine(TypeSentence(sentenceQueue.Dequeue()));
        Debug.Log("Finished dispalying dialog");
        if (showChoice) {
            //Bring up choice buttons
            ChoiceButtons choiceButtons = Instantiate(choiceButtonsPrefab, transform).GetComponent<ChoiceButtons>();
            choiceButtons.OnChoiceMade += HandleChoiceMade;
            choiceButtons.OnChoiceMade += (x) => Destroy(choiceButtons.gameObject);

            //prevents the choicebuttons returning
            showChoice = false;
        } else {
            //Dialog is finished
            OnDialogComplete?.Invoke(choiceMade);
        }
    }

    IEnumerator TypeSentence(string sentence) {
        Debug.Log(sentence);
        dialogText.useMaxVisibleDescender = false;

        dialogText.text = Regex.Replace(sentence, @"\t|\n|\r", ""); ;
        dialogText.textInfo.pageInfo[0].lastCharacterIndex = 0;
        dialogText.maxVisibleCharacters = 0;

        for (int i = 0; i < sentence.Length;) {
            dialogText.maxVisibleCharacters++;
            if (dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex == dialogText.maxVisibleCharacters) {
                    Debug.Log("Waiting for player click");
                    yield return null;
            }else {
                i++;
            }
            Debug.Log("Waiting for typing speed");
            yield return new WaitForSeconds(typingSpeed);
        }

    }

    void HandleChoiceMade(bool choice) {
        choiceMade = choice;
        if (choice) {
            //Yes
            StartDialog(dialog.YesDialog);
        } else {
            //No
            StartDialog(dialog.NoDialog);
        }
    }

    void HandlePlayerClick() {
        Debug.Log("Handling player click on dialog");
        dialogText.maxVisibleCharacters = dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex;
        dialogText.pageToDisplay++;
        if (dialogText.pageToDisplay >= dialogText.textInfo.pageCount) {
            playerController.OnClick -= HandlePlayerClick;
        }
    }
}
