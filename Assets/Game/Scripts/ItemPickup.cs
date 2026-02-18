using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("basic setting")]
    // The Items model in the player's hand (initially hidden)
    public GameObject playerHandTool;
    // How long to hold the 'E' key
    public float holdDuration = 1.0f;

    [Header("visual setting")]
    // The renderer of this pickup item (to change its color)
    public Renderer targetRenderer;
    [ColorUsage(true, true)]
    public Color glowColor = new Color(1f, 1f, 0f, 1f);
    public float glowIntensity = 2f;

    private float currentHoldTime = 0f;
    private bool isPlayerInZone = false;
    private Material targetMat;
    private Color defaultEmissionColor = Color.black;

    void Start()
    {
        // Auto-find the Renderer if not assigned
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        if (targetRenderer != null)
        {
            targetMat = targetRenderer.material;

            // Important: Enable "Emission" so the material can glow
            targetMat.EnableKeyword("_EMISSION");

            // Save the original color (usually black/no glow) to restore later
            if (targetMat.HasProperty("_EmissionColor"))
            {
                defaultEmissionColor = targetMat.GetColor("_EmissionColor");
            }
        }
    }

    void Update()
    {
        // Only allow pickup if player is standing nearby
        if (isPlayerInZone)
        {
            // Player holds 'E' key
            if (Input.GetKey(KeyCode.E))
            {
                currentHoldTime += Time.deltaTime; // Increase timer

                // Timer finished -> Pickup!
                if (currentHoldTime >= holdDuration)
                {
                    PickUpSuccess();
                }
            }
            else
            {
                // Reset timer if player lets go of the key
                currentHoldTime = 0f;
            }
        }
    }

    void PickUpSuccess()
    {
        Debug.Log("pickup success");

        if (playerHandTool != null)
        {
            ItemManage manager = playerHandTool.GetComponentInParent<ItemManage>();

            if (manager != null)
            {
                // Equip the Items using the manager logic
                manager.EquipItems(playerHandTool);
            }
            else
            {
                // Simple mode: just show the Items in hand
                playerHandTool.SetActive(true);
            }
        }

        // Destroy the item on the ground since we picked it up
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log($"Trigger Enter: {other.name}, Tag: {other.tag}");

        // Player walked into range
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            // Turn ON the glow effect
            SetGlow(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {


        // Player walked away
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            currentHoldTime = 0f;
            // Turn OFF the glow effect
            SetGlow(false);
        }
    }

    void SetGlow(bool isGlowing)
    {
        if (targetMat == null || !targetMat.HasProperty("_EmissionColor")) return;

        if (isGlowing)
        {
            // Make it bright: Color * Intensity
            Color finalGlow = glowColor * glowIntensity;
            targetMat.SetColor("_EmissionColor", finalGlow);
        }
        else
        {
            // Reset to original color (stop glowing)
            targetMat.SetColor("_EmissionColor", defaultEmissionColor);
        }
    }
}