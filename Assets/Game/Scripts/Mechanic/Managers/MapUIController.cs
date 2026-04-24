using UnityEngine;

public class MapUIController : MonoBehaviour
{

    [Header("Map UI Settings")]
    public GameObject mapCanvas;
    public GameObject GamePlayUI;
 
    public KeyCode mapkey = KeyCode.M;

    private bool isMapOpen = false; 

    private bool gameplayUIActive = true;

    void Start()
    {
        if (mapCanvas != null) mapCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(mapkey))
        {
            
            ToggleMap();
        }
    }

    void ToggleMap()
    {
        if (mapCanvas == null) return;
        gameplayUIActive = !gameplayUIActive;
        GamePlayUI.SetActive(gameplayUIActive);
        isMapOpen = !isMapOpen;
        mapCanvas.SetActive(isMapOpen);
    }

}
