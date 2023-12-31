using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class DialogManager : MonoBehaviour {

    [SerializeField] Dialog testDialog;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] float typingSpeed;
    [SerializeField] GameObject choiceButtonsPrefab;

    [SerializeField] AudioClip clickSound;

    public event Action<bool> OnDialogComplete;



    Queue<string> sentenceQueue;
    Dialog dialog;
    bool showChoice;
    bool choiceMade;
    bool showingChoice;
    PlayerController playerController;


    void Awake() {
        sentenceQueue = new Queue<string>();
        //SetDialog(testDialog, "Test");
        playerController = FindObjectOfType<PlayerController>();
        playerController.OnLeftClick += HandlePlayerClick;
    }

    void OnDestroy() {
        playerController.OnLeftClick -= HandlePlayerClick;
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

        while (sentenceQueue.Count > 0) {
            //hacky way to keep from displaying the choice buttons until the last sentence is displayed
            yield return StartCoroutine(TypeSentence(sentenceQueue.Dequeue(), sentenceQueue.Count == 0 && showChoice));
        }
        Debug.Log("Finished displaying dialog");
        if (showChoice) {
            //Bring up choice buttons
            ChoiceButtons choiceButtons = Instantiate(choiceButtonsPrefab).GetComponent<ChoiceButtons>();
            choiceButtons.OnChoiceMade += HandleChoiceMade;
            choiceButtons.OnChoiceMade += (x) => {
                Destroy(choiceButtons.gameObject);
                showingChoice = false;
            };
            //prevents the choicebuttons returning
            showChoice = false;
            showingChoice = true;
        } else {
            //Dialog is finished
            OnDialogComplete?.Invoke(choiceMade);
        }
    }

    IEnumerator TypeSentence(string sentence, bool showChoice) {
        dialogText.useMaxVisibleDescender = false;

        dialogText.text = Regex.Replace(sentence, @"\t|\n|\r", "");
        Debug.Log(dialogText.text);

        dialogText.pageToDisplay = 1;
        dialogText.maxVisibleCharacters = 0;
        //This is here because the page count on dialogText is not updated until the next frame
        yield return null;

        while (true) {
            //Exit coroutine when all pages are displayed
            if (dialogText.pageToDisplay >= dialogText.textInfo.pageCount && dialogText.maxVisibleCharacters >= dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex + 1) {
                //Weird logic, if there needs to be a choice, we exit but keep the last page being displayed so its visible with choice buttons, 
                //otherwise its the end of the dialog and needs an extra player click to push the page number up to exit
                if (dialogText.pageToDisplay > dialogText.textInfo.pageCount || showChoice) {
                    yield break;
                }

            }
            //Pauses dialog until player clicks
            else if (dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex + 1 == dialogText.maxVisibleCharacters) {
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
        //If showing dialog buttons, return
        if (showingChoice) return;

        //Play click sound
        if (clickSound != null)
            AudioSource.PlayClipAtPoint(clickSound, playerController.transform.position, GameManager.Instance.GetVolume());

        //If the player clicks and not all characters on page are displayed, display all characters on page
        if (dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex + 1 != dialogText.maxVisibleCharacters) {
            dialogText.maxVisibleCharacters = dialogText.textInfo.pageInfo[dialogText.pageToDisplay - 1].lastCharacterIndex + 1;
            return;
        }

        //Otherwise, display the next page

        dialogText.pageToDisplay++;

    }
}
