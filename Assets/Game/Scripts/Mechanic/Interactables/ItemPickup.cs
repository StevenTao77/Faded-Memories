using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Basic Setting")]
    public string itemName = "";

    [Header("UI Prompt")]
     
    public GameObject promptVisual;
    public KeyCode InteractKey => KeyCode.E;

    private void Start()
    {
          
        if (promptVisual != null) promptVisual.SetActive(false);
    }
    public void Interact()
    {
        PickUpSuccess();
    }

    public void TogglePrompt(bool show)
    {
        if (promptVisual != null)
        {
            promptVisual.SetActive(show);
        }
    }

    void PickUpSuccess()
    {
                Debug.Log("Pickup success: " + itemName);
        if (InventoryManager.Instance != null)
        {
            // Try to find the item image from EquipmentManager
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
    
   
}