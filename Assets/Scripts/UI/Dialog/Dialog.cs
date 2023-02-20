using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/Dialog")]

public class Dialog : ScriptableObject {
    [TextArea(5, 10)]
    public List<string> Sentences;

}
