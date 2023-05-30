using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialUIScript : TriggerScript
{
    public string textToChange = "New Text";
    public TMP_Text textObject;

    public override void ActionToTrigger()
    {
        textObject.text = textToChange;
        textObject.gameObject.SetActive(true);
    }

    public override void ActionToStop()
    {
        textObject.text = textToChange;
        textObject.gameObject.SetActive(false);
    }
}
