using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class DialogManager : MonoBehaviour {

    [SerializeField] Dialog testDialog;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] float typingSpeed;

    Queue<string> sentenceQueue;
    Dialog dialog;

    PlayerController playerController;

    void Awake() {
        sentenceQueue = new Queue<string>();
        StartDialog(testDialog);
        DisplayNextSentence();
        // playerController.onClick += DisplayNextSentence();
    }

    public void StartDialog(Dialog dialog) {
        this.dialog = dialog;

        sentenceQueue.Clear();

        dialog.Sentences.ForEach(x => sentenceQueue.Enqueue(x));
    }

    public void DisplayNextSentence() {
        if (sentenceQueue.Count == 0) {
            //End
            return;
        }
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentenceQueue.Dequeue()));

    }

    IEnumerator TypeSentence(string sentence) {
        Debug.Log(sentence);    
        dialogText.useMaxVisibleDescender = false;

        dialogText.text = sentence;
        dialogText.textInfo.pageInfo[0].lastCharacterIndex = 0;
        dialogText.maxVisibleCharacters = 0;

        foreach (char letter in sentence.ToCharArray()) {
            Debug.Log(dialogText.maxVisibleCharacters);
            dialogText.maxVisibleCharacters++;
            if (dialogText.textInfo.pageInfo[dialogText.pageToDisplay -1].lastCharacterIndex == dialogText.maxVisibleCharacters) {
                dialogText.pageToDisplay++;
            }
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
