using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform itemsGridParent;
    public GameObject itemSlotPrefab;

    private bool isInventoryOpen = false;

    private void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback += UpdateUI;

            UpdateUI();
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

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
        foreach (Transform child in itemsGridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (string itemName in InventoryManager.Instance.inventoryItems)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, itemsGridParent);

            TextMeshProUGUI slotText = newSlot.GetComponentInChildren<TextMeshProUGUI>();

            if (slotText != null)
            {
                slotText.text = itemName;
            }
            else
            {
                Debug.LogWarning("Cannot find TextMeshProUGUI on the instantiated slot!");
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
}