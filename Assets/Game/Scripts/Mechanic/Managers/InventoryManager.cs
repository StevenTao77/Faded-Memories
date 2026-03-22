using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<string> inventoryItems = new List<string>();

    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChangedCallback;

    public string currentlyEquippedItem = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string itemName)
    {
        inventoryItems.Add(itemName);

        if (onInventoryChangedCallback != null)
        {
            onInventoryChangedCallback.Invoke();
        }
    }
}