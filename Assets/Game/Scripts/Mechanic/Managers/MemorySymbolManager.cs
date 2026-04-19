using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MemorySymbolManager : MonoBehaviour
{
    public static MemorySymbolManager Instance { get; private set; }

    [SerializeField] private GameObject boatSymbol;
    [SerializeField] private string memoryRootName = "MemoryObjects";

    private Dictionary<string, GameObject> symbolDict = new Dictionary<string, GameObject>();
    private HashSet<string> interactedSymbols = new HashSet<string>();

    private bool ready = false;
    private bool allSymbolsInteracted = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (boatSymbol != null)
            boatSymbol.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ready = false;
        StartCoroutine(RebuildAfterScene());
    }

    private IEnumerator RebuildAfterScene()
    {
        yield return null;
        yield return null;

        BuildDictionary();
        ApplyState();

        ready = true;
    }

    private void BuildDictionary()
    {
        symbolDict.Clear();

        GameObject root = GameObject.Find(memoryRootName);

        if (root == null)
        {
            Debug.LogWarning("MemoryObjects not found");
            return;
        }

        foreach (Transform child in root.transform)
        {
            string key = child.name.ToLower().Replace("symbol", "").Trim();
            symbolDict[key] = child.gameObject;
        }
    }

    private void ApplyState()
    {
        foreach (var key in interactedSymbols)
        {
            if (symbolDict.TryGetValue(key, out GameObject obj))
            {
                obj.SetActive(false);
            }
        }

        if (symbolDict.Count > 0 &&
            interactedSymbols.Count == symbolDict.Count)
        {
            ShowBoat();
        }
    }

    public void OnSymbolInteracted(string symbolName)
    {
        if (!ready) return;
        if (string.IsNullOrEmpty(symbolName)) return;

        string key = symbolName.ToLower();

        if (interactedSymbols.Contains(key))
            return;

        if (symbolDict.TryGetValue(key, out GameObject obj))
        {
            obj.SetActive(false);
        }

        interactedSymbols.Add(key);

        if (interactedSymbols.Count == symbolDict.Count)
        {
            ShowBoat();
        }
    }

    private void ShowBoat()
    {
        if (boatSymbol != null && !allSymbolsInteracted)
        {
            boatSymbol.SetActive(true);
            allSymbolsInteracted = true;
        }
    }

    public bool HasInteractedWithSymbol(string symbolName)
    {
        return interactedSymbols.Contains(symbolName.ToLower());
    }

    public bool AreAllSymbolsInteracted()
    {
        return allSymbolsInteracted;
    }
}