using UnityEngine;
using System.Collections.Generic;

public class MemorySymbolManager : MonoBehaviour
{
    public static MemorySymbolManager Instance { get; private set; }

    private GameObject boatSymbol;
   // private GameObject boatpromt;
    private Dictionary<string, List<GameObject>> symbolDict = new Dictionary<string, List<GameObject>>();
    private HashSet<string> interactedSymbols = new HashSet<string>();
    private bool allSymbolsInteracted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }


    //public void ResetProgress()
    //{
    //    // Assuming 'interactedSymbols' is a List<string> or HashSet<string>
    //    if (interactedSymbols != null)
    //    {
    //        interactedSymbols.Clear();
    //    }
    //}

    public void RegisterMemoryRoot(Transform uiRoot, GameObject registeredBoat)
    {
        boatSymbol = registeredBoat;

        
        if (boatSymbol != null) boatSymbol.SetActive(false);

        foreach (Transform child in uiRoot)
        {
            if (child.gameObject == boatSymbol) continue;

            string key = child.name.ToLower().Replace("symbol", "").Trim();
            if (!symbolDict.ContainsKey(key)) symbolDict[key] = new List<GameObject>();
            if (!symbolDict[key].Contains(child.gameObject)) symbolDict[key].Add(child.gameObject);
        }

         
        ApplyState();
    }

    private void ApplyState()
    {
         
        foreach (string key in interactedSymbols)
        {
            HideSymbolIcons(key);
        }

         
        if (allSymbolsInteracted)
        {
            if (boatSymbol != null) boatSymbol.SetActive(true);
        }
        else
        {
             
            CheckBoatCondition();
        }
    }

    public void OnSymbolInteracted(string symbolName)
    {
        if (string.IsNullOrEmpty(symbolName)) return;
        string key = symbolName.ToLower();

        if (!interactedSymbols.Contains(key))
        {
            interactedSymbols.Add(key);
            HideSymbolIcons(key);
            CheckBoatCondition();
        }
    }

    private void HideSymbolIcons(string key)
    {
        if (symbolDict.TryGetValue(key, out List<GameObject> objects))
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
    }

    private void CheckBoatCondition()
    {
        
        if (!allSymbolsInteracted && symbolDict.Count > 0 && interactedSymbols.Count == symbolDict.Count)
        {
            allSymbolsInteracted = true;
            if (boatSymbol != null) boatSymbol.SetActive(true);
           // if (boatpromt != null) boatpromt.SetActive(true);
            Debug.Log("[MemoryManager] got it");
        }
    }

    public bool HasInteractedWithSymbol(string symbolName) => interactedSymbols.Contains(symbolName.ToLower());
    public bool AreAllSymbolsInteracted() => allSymbolsInteracted;
}