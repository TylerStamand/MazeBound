using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/Dialog")]

public class Dialog : ScriptableObject {

    [TextArea(5, 5)]
    public List<string> Sentences;


    [field: SerializeField] public bool IsChoice { get; private set; }
    [ShowIf("IsChoice")]
    [ResizableTextArea]
    public List<string> YesDialog;
    [ShowIf("IsChoice")]
    [ResizableTextArea]
    public List<string> NoDialog;

}
