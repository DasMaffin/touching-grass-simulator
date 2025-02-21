using UnityEngine;
using TMPro;

public class ErrorListenerTextController : MonoBehaviour
{
    private MinimizeUIController minimizeUIController;
    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        minimizeUIController = transform.parent.GetComponentInChildren<MinimizeUIController>();
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // Subscribe to the OnError event if a controller is assigned
        if(minimizeUIController != null)
        {
            minimizeUIController.OnError += UpdateErrorText;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnError event
        if(minimizeUIController != null)
        {
            minimizeUIController.OnError -= UpdateErrorText;
        }
    }

    private void UpdateErrorText(string message)
    {
        // Set the received error message to the TextMeshPro component
        textComponent.text = message;
    }
}
