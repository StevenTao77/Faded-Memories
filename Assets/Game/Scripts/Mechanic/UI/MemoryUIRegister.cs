using UnityEngine;

public class MemoryUIRegister : MonoBehaviour
{
    private void Start()
    {
        
        if (MemorySymbolManager.Instance != null)
        {
            MemorySymbolManager.Instance.RegisterMemoryRoot(this.transform);
        }
        else
        {
            Debug.LogWarning("UI tried to register, but MemorySymbolManager does not exist.");
        }
    }
}