using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("UI")]
    public TMPro.TextMeshProUGUI promptText;
    
    [Header("Raycast Settings")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if(cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (promptText != null)
            promptText.text = string.Empty;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, interactDistance, interactableLayer))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (promptText != null)
                    promptText.text = interactable.promptMessage;
                
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
