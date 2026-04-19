using UnityEngine;
using System.Collections.Generic;

public class MemorySymbolManager : MonoBehaviour
{
    public static MemorySymbolManager Instance { get; private set; }

    [SerializeField] private GameObject boatSymbol;

    private Dictionary<string, List<GameObject>> symbolDict = new Dictionary<string, List<GameObject>>();
    private HashSet<string> interactedSymbols = new HashSet<string>();

    private bool allSymbolsInteracted = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        transform.SetParent(null);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (boatSymbol != null) boatSymbol.SetActive(false);
    }

    public void RegisterMemoryRoot(Transform uiRoot)
    {
        foreach (Transform child in uiRoot)
        {
            string key = child.name.ToLower().Replace("symbol", "").Trim();

            if (!symbolDict.ContainsKey(key))
                symbolDict[key] = new List<GameObject>();

            if (!symbolDict[key].Contains(child.gameObject))
                symbolDict[key].Add(child.gameObject);
        }

        ApplyState();
    }

    private void ApplyState()
    {
        foreach (string key in interactedSymbols)
        {
            HideSymbolIcons(key);
        }

        CheckBoatCondition();
    }

    public void OnSymbolInteracted(string symbolName)
    {
        if (string.IsNullOrEmpty(symbolName)) return;

        string key = symbolName.ToLower();

        // Only process if it hasn't been interacted with yet
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
            if (boatSymbol != null) boatSymbol.SetActive(true);
            allSymbolsInteracted = true;
        }
    }

    public bool HasInteractedWithSymbol(string symbolName) => interactedSymbols.Contains(symbolName.ToLower());

    public bool AreAllSymbolsInteracted() => allSymbolsInteracted;
}