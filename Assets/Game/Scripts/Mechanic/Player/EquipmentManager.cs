using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipableItem
{
    public string itemName;
    public GameObject handModel;
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
         
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryEquipItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryEquipItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryEquipItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TryEquipItem(3);

        if (Input.GetKeyDown(KeyCode.X)) UnequipAll();
    }

    private void TryEquipItem(int slotIndex)
    {
        if (InventoryManager.Instance == null) return;

        List<string> currentInv = InventoryManager.Instance.inventoryItems;

        if (slotIndex >= 0 && slotIndex < currentInv.Count)
        {
            string targetItemName = currentInv[slotIndex];
            EquipByName(targetItemName);
        }
        else
        {
            Debug.Log("Slot " + (slotIndex + 1) + " is empty!");
        }
    }

    public void EquipByName(string targetName)
    {
        bool found = false;
        foreach (EquipableItem item in allHandItems)
        {
            if (item.itemName == targetName && !item.handModel.activeSelf  )
            {
                item.handModel.SetActive(true);
                found = true;
            }
            else
            {
                item.handModel.SetActive(false);
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
            item.handModel.SetActive(false);
        }

        
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.currentlyEquippedItem = "";
        }
    }
}