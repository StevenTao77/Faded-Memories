using UnityEngine;
using UnityEngine.UI; // If using legacy Text
// using TMPro; // Uncomment this if using TextMeshPro for your slot text

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform itemsGridParent;
    public GameObject itemSlotPrefab;

    private bool isInventoryOpen = false;

    private void Start()
    {
        // Make sure the inventory is closed when the game starts
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        // Listen to the manager's event
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback += UpdateUI;
        }
    }

    private void Update()
    {
        // Press B to open/close
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        // Manage mouse cursor visibility
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void UpdateUI()
    {
        // 1. Destroy all existing slots to avoid duplicates
        foreach (Transform child in itemsGridParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Create a new slot for each item in the list
        foreach (string itemName in InventoryManager.Instance.inventoryItems)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, itemsGridParent);

            // Find the Text component inside the new slot
            Text slotText = newSlot.GetComponentInChildren<Text>();
            // TextMeshProUGUI slotText = newSlot.GetComponentInChildren<TextMeshProUGUI>(); // Use this if using TMPro

            if (slotText != null)
            {
                slotText.text = itemName;
            }
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback -= UpdateUI;
        }
    }

    //private void OnDisable()
    //{
    //    if(InventoryManager.Instance != null)
    //    {
    //        InventoryManager.Instance.onInventoryChangedCallback -= UpdateUI;
    //    }
    //}
}