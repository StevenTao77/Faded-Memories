using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform itemsGridParent;
    public GameObject itemSlotPrefab;

    [Header("Image Display Settings")]
    [Tooltip("Enable this to show item names as text")]
    public bool showItemText = false;

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

        foreach (InventoryItem item in InventoryManager.Instance.inventoryItems)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, itemsGridParent);

            // Set the item image if available
            Image slotImage = newSlot.GetComponentInChildren<Image>();
            if (slotImage != null && item.itemImage != null)
            {
                slotImage.sprite = Sprite.Create(item.itemImage, new Rect(0, 0, item.itemImage.width, item.itemImage.height), Vector2.one * 0.5f);
                slotImage.type = Image.Type.Simple;
                slotImage.preserveAspect = true;
            }

            // Set the item name text (optional)
            TextMeshProUGUI slotText = newSlot.GetComponentInChildren<TextMeshProUGUI>();
            if (slotText != null)
            {
                if (showItemText)
                {
                    slotText.text = item.itemName;
                }
                else
                {
                    slotText.gameObject.SetActive(false);
                }
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