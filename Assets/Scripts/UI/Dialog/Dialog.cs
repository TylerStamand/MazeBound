using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/Dialog")]

public class Dialog : ScriptableObject {

    [TextArea(5, 5)]
    public List<string> Sentences;

    [field: SerializeField] public bool IsChoice { get; private set; }
    [TextArea(5, 5)]
    public List<string> YesDialog;
    [TextArea(5, 5)]
    public List<string> NoDialog;

}
