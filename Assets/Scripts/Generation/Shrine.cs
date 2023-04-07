using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Shrine : MonoBehaviour, IInteractable {

    [SerializeField] Light2D light2D;

    [SerializeField] GameObject puzzlePiece1;
    [SerializeField] GameObject puzzlePiece2;
    [SerializeField] GameObject puzzlePiece3;

    [SerializeField] float puzzlePieceHight = 1.5f;
    [SerializeField] float puzzlePieceDuration = 3f;
    [SerializeField] float endIntensity = 15f;

    public event Action<Shrine> OnInteract;

    bool activated = false;




    public void Interact(PlayerCharacter playerCharacter) {
        if (!activated) {
            activated = true;
            ShowActivated();
            OnInteract?.Invoke(this);
        }
    }

    public void ShowActivated() {
        Debug.Log("Showing activated");
        StartCoroutine(TurnOnLight());
    }

    public void ShowPuzzlePiece(int piece) {
        Debug.Log("Showing puzzle piece");
        GameObject puzzlePiece = null;
        switch (piece) {
            case 1:
                puzzlePiece1.SetActive(true);
                puzzlePiece = Instantiate(puzzlePiece1, transform.position, Quaternion.identity, transform);
                break;
            case 2:
                puzzlePiece2.SetActive(true);
                puzzlePiece = Instantiate(puzzlePiece2, transform.position, Quaternion.identity, transform);
                break;
            case 3:
                puzzlePiece3.SetActive(true);
                puzzlePiece = Instantiate(puzzlePiece3, transform.position, Quaternion.identity, transform);
                break;
        }
        StartCoroutine(DisplayPuzzlePiece(puzzlePiece));



    }

    public IEnumerator TurnOnLight() {
        float time = 0;
        float duration = 1;
        while (time < duration) {
            time += Time.deltaTime;
            light2D.intensity = Mathf.Lerp(0, endIntensity, time / duration);
            yield return null;
        }
    }

    public IEnumerator DisplayPuzzlePiece(GameObject puzzlePiece) {
        float time = 0;
        float startHight = transform.position.y;
        float endHight = transform.position.y + puzzlePieceHight;
        while (time < puzzlePieceDuration) {
            time += Time.deltaTime;
            puzzlePiece.transform.position = new Vector3(transform.position.x, Mathf.Lerp(startHight, endHight, time / puzzlePieceDuration), transform.position.z);
            yield return null;
        }

        while (time < puzzlePieceDuration + 2) {
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(puzzlePiece);


    }
}
