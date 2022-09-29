using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Text Box")]
public class TextBoxText : ScriptableObject
{
    public List<string> textLines;
    public int currentLine = 0;
    public bool stacking = false;
}
