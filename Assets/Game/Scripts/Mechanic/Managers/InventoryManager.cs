using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Texture2D itemImage;

    public InventoryItem(string name, Texture2D image)
    {
        itemName = name;
        itemImage = image;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

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

    public void AddItem(string itemName, Texture2D itemImage = null)
    {
        inventoryItems.Add(new InventoryItem(itemName, itemImage));

        if (onInventoryChangedCallback != null)
        {
            onInventoryChangedCallback.Invoke();
        }
    }
}