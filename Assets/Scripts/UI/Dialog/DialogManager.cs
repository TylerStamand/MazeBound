using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class DialogManager : MonoBehaviour {

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] float typingSpeed;

    Queue<string> sentenceQueue;
    Dialog dialog;

    PlayerController playerController;

    void Awake() {
        sentenceQueue = new Queue<string>();
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
        dialogText.text = sentence;
        dialogText.maxVisibleCharacters = 0;
        foreach (char letter in sentence.ToCharArray()) {
            dialogText.maxVisibleCharacters++;
            if (dialogText.firstOverflowCharacterIndex == dialogText.maxVisibleCharacters) {
                dialogText.pageToDisplay++;
                dialogText.maxVisibleCharacters = 0;
            }
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
