using UnityEngine;

public class ItemPickup : BaseInteractable
{
    [Header("Basic Setting")]
    public string itemName = "";

    public override void Interact()
    {
        PickUpSuccess();
    }
     

    private void PickUpSuccess()
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
}