using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI_TriggerScript : TriggerScript
{
    [Header("References")]
    public TutorialUIManager manager;
    [SerializeField] private GameObject[] tutorialElements;
    private bool isIn = false;

    [Header("Settings")]
    [SerializeField] private int elementToCall;

    // Start is called before the first frame update
    void Awake()
    {
        tutorialElements = manager.UI_Elements;
    }

    private void Update()
    {
        if (PlayerController.ourPlayerState == PlayerState.inMenu && isIn ==true)
        {
            tutorialElements[elementToCall].SetActive(false);
        }
    }

    public override void ActionToTrigger()
    {
        tutorialElements[elementToCall].SetActive(true);
        isIn = true;
    }

    public override void ActionToStop()
    {
        tutorialElements[elementToCall].SetActive(false);
        isIn = false;
    }
}
