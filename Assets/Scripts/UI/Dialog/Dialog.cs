using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/Dialog")]

public class Dialog : ScriptableObject {
    public bool IsChoice { get; private set; }

    [TextArea(5, 10)]
    public List<string> Sentences;

    public List<string> YesDialog;
    public List<string> NoDialog;

}
