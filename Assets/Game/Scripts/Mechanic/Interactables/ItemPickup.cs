using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Basic Setting")]
    public string itemName = "";
    public float holdDuration = 1.0f;

    private float currentHoldTime = 0f;
    private bool isPlayerInZone = false;

    void Update()
    {
        if (isPlayerInZone)
        {
            if (Input.GetKey(KeyCode.E))
            {
                currentHoldTime += Time.deltaTime;

                if (currentHoldTime >= holdDuration)
                {
                    PickUpSuccess();
                }
            }
            else
            {
                // Reset timer if player lets go of the key
                currentHoldTime = 0f;
            }
        }
    }

    void PickUpSuccess()
    {
        Debug.Log("Pickup success: " + itemName);

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemName);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
        }
        else
        {
            Debug.LogAssertion("Can't find Player Tag!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            currentHoldTime = 0f;
        }
        else
        {
             Debug.LogAssertion("Can't find Player Tag!");
        }
    
    }
}