using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSelfDestruct : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Invoke(nameof(Die), 4f);
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
