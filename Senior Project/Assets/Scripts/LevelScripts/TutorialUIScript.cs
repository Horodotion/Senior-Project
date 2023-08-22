using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialUIScript : TriggerScript
{
    public string textToChange = "New Text";

    public override void ActionToTrigger()
    {
        if (PlayerUI.instance != null)
        {
            PlayerUI.instance.ActivateTutorialPanel(textToChange);
        }
    }

    public override void ActionToStop()
    {
        if (PlayerUI.instance != null)
        {
            PlayerUI.instance.DeactivateTutorialPanel();
        }
    }
}
