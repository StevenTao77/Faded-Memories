using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipableItem
{
    public string itemName;
    public GameObject handModel;
    public Texture2D itemImage;
}

public class EquipmentManager : MonoBehaviour
{
    [Header("All Possible Hand Models")]
    public List<EquipableItem> allHandItems;

    private void Start()
    {
        foreach (EquipableItem item in allHandItems)
        {
            if (item.handModel != null)
            {
                item.handModel.SetActive(false);
            }
        }

        if (InventoryManager.Instance != null && !string.IsNullOrEmpty(InventoryManager.Instance.currentlyEquippedItem))
        {
            string lastItem = InventoryManager.Instance.currentlyEquippedItem;
            EquipByName(lastItem);
        }
    }

    private void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryEquipSpecificItem("torch");
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryEquipSpecificItem("Axe");

        if (Input.GetKeyDown(KeyCode.X)) UnequipAll();
    }
     
    private void TryEquipSpecificItem(string targetItemName)
    {
        if (InventoryManager.Instance == null) return;

        bool hasItem = false;
        List<InventoryItem> currentInv = InventoryManager.Instance.inventoryItems;
         
        foreach (InventoryItem item in currentInv)
        {
            if (item.itemName == targetItemName)
            {
                hasItem = true;
                break;
            }
        }

        if (hasItem)
        {
            EquipByName(targetItemName);
        }
        else
        {
            Debug.Log("You do not have " + targetItemName + " in your inventory!");
        }
    }

    public void EquipByName(string targetName)
    {
        bool found = false;
        foreach (EquipableItem item in allHandItems)
        {
            if (item.itemName == targetName && !item.handModel.activeSelf)
            {
                item.handModel.SetActive(true);
                found = true;
            }
            else
            {
                if (item.handModel != null)
                {
                    item.handModel.SetActive(false);
                }
            }
        }

        if (found && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.currentlyEquippedItem = targetName;
        }
    }

    private void UnequipAll()
    {
        foreach (EquipableItem item in allHandItems)
        {
            if (item.handModel != null)
            {
                item.handModel.SetActive(false);
            }
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.currentlyEquippedItem = "";
        }
    }
}