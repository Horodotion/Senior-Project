using UnityEngine;
using TMPro;

public abstract class MenuScript : MonoBehaviour
{
    public virtual void Start()
    {
        if (GetComponent<CanvasGroup>() != null)
        {
            GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    public static void SwapToMenu(GameObject menuToGoTo, GameObject menuToTurnOff)
    {
        menuToGoTo.SetActive(true);
        menuToTurnOff.SetActive(false);
    }

    public static void ToggleMenu(GameObject menu)
    {
        if (menu.activeInHierarchy)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
    }
}
