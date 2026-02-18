using UnityEngine;

public class ItemManage : MonoBehaviour
{

    public GameObject[] allItem;

    // Function to switch the active items
    public void EquipItems(GameObject ItemtoShow)
    {
        // Hide ALL items first
        // This ensures we don't have multiple items visible at once
        foreach (GameObject w in allItem)
        {
            if (w != null)
            {
                w.SetActive(false);
            }
        }

        // Show ONLY the requested items
        if (ItemtoShow != null)
        {
            ItemtoShow.SetActive(true);
        }
    }
}