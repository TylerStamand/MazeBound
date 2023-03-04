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

    void OnDestroy() {
        playerController.OnClick -= HandlePlayerClick;
        StopAllCoroutines();
    }

    public void SetDialog(Dialog dialog, string name) {
        this.dialog = dialog;
        nameText.text = name;
        showChoice = dialog.IsChoice;
        StartDialog(dialog.Sentences);
    }

    public void StartDialog(List<String> sentences) {

        sentenceQueue.Clear();
        sentences.ForEach(x => sentenceQueue.Enqueue(x));
        StartCoroutine(DisplayDialog());

    }

    IEnumerator DisplayDialog() {

        yield return StartCoroutine(TypeSentence(sentenceQueue.Dequeue()));
        Debug.Log("Finished dispalying dialog");
        if (showChoice) {
            //Bring up choice buttons
            ChoiceButtons choiceButtons = Instantiate(choiceButtonsPrefab).GetComponent<ChoiceButtons>();
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

        dialogText.text = Regex.Replace(sentence, @"\t|\n|\r", "");
        dialogText.pageToDisplay = 1;
        dialogText.maxVisibleCharacters = 0;
        //This is here because the page count on dialogText is not updated until the next frame
        yield return null;

        while (dialogText.maxVisibleCharacters < sentence.Length) {

            //Exit coroutine when all pages are displayed
            if (dialogText.pageToDisplay > dialogText.textInfo.pageCount) {
                Debug.Log("Finished displaying all pages");
                yield break;

            }
            //Pauses dialog until player clicks
            else if (dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex == dialogText.maxVisibleCharacters) {
                yield return null;
            }
            //Increment the number of visible characters
            else {
                dialogText.maxVisibleCharacters++;
            }

            //Slow down the speed of the dialog
            yield return new WaitForSeconds(typingSpeed);
        }

    }

    void HandleChoiceMade(bool choice) {
        choiceMade = choice;
        Debug.Log("Choice made: " + choice);
        if (choice) {
            //Yes
            StartDialog(dialog.YesDialog);
        } else {
            //No
            StartDialog(dialog.NoDialog);
        }
    }

    void HandlePlayerClick() {
        //If the player clicks and not all characters on page are displayed, display all characters on page
        if (dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex != dialogText.maxVisibleCharacters) {
            dialogText.maxVisibleCharacters = dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex;
            return;
        }

        //Otherwise, display the next page
        dialogText.pageToDisplay++;

    }
}
