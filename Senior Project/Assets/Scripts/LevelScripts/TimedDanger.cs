using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TimedDanger : MonoBehaviour
{
    private bool activated;

    // Timer
    [SerializeField] private float activeTime = 2.5f, inactiveTime = 2.5f;
    private float timer;

    // Temperature affectation
    [SerializeField] private float tempPerSecond;
    private PlayerPuppet target;

    // Aesthetic
    [SerializeField] private MeshRenderer ownRenderer;

    private void Awake()
    {
        if (ownRenderer == null) TryGetComponent(out ownRenderer);
    }

    void Update()
    {
        // Count timer down- when it reaches zero, toggle activity and reset timer
        timer -= Time.deltaTime;
        if (timer <= 0) /// Called only one frame
        {
            if (activated)
            {
                // All effects that happen when the object deactivates
                ownRenderer.enabled = false;

                // Reset timer and toggle activity state
                timer = inactiveTime;
                activated = false;
            }
            else
            {
                // All effects that happen when the object activates
                ownRenderer.enabled = true;

                // Reset timer and toggle activity state
                timer = activeTime;
                activated = true;
            }
        }

        if (activated) /// Called every frame the object is active or inactive respectively
        {
            // If this object is targeting the player, affect the player's temperature by the specified amount
            if (target != null)
            {
                target.ChangeTemperature(tempPerSecond * Time.deltaTime);
            }
        }
        else
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent(out target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            target = null;
        }
    }
}
