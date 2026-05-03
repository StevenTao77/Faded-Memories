using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Basic Setting")]
    public string itemName = "";
    public float holdDuration = 1.0f;

    private float currentHoldTime = 0f;
    private bool isPlayerInZone = false;

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
            foreach (InventoryItem item in InventoryManager.Instance.inventoryItems)
            {
                if (item.itemName.ToLower() == itemName.ToLower())
                {
                    gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

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
                 
                currentHoldTime = 0f;
            }
        }
    }

    void PickUpSuccess()
    {
        Debug.Log("Pickup success: " + itemName);

        if (InventoryManager.Instance != null)
        { 
            Texture2D itemImage = null;
            EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();

            if (equipmentManager != null)
            {
                foreach (EquipableItem item in equipmentManager.allHandItems)
                {
                    if (item.itemName == itemName)
                    {
                        itemImage = item.itemImage;
                        break;
                    }
                }
            }

            InventoryManager.Instance.AddItem(itemName, itemImage);
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