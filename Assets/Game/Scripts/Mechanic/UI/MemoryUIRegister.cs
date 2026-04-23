using UnityEngine;

public class MemoryUIRegister : MonoBehaviour
{
    [Header("UI References")]
    public GameObject boatIcon;

    private void OnEnable()
    {
        if (MemorySymbolManager.Instance != null)
        {
            // Pass BOTH the parent transform (for icons) AND the boat explicitly
            MemorySymbolManager.Instance.RegisterMemoryRoot(this.transform, boatIcon);
        }
        else
        {
            Debug.LogWarning("[UIRegister] MemorySymbolManager not found yet. Are you running from the main scene?");
        }
    }
}