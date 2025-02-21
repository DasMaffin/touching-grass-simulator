using UnityEngine;
using UnityEngine.EventSystems;

public class CreditController : MonoBehaviour, IPointerClickHandler
{
    public string webURI;

    private bool wasClicked = false;

    private void Awake()
    {
        if(PlayerPrefs.GetInt(webURI) == 1)
        {
            CreditsManager.Instance.ClickedCredits++;
            wasClicked = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!string.IsNullOrEmpty(webURI))
        {
            Application.OpenURL(webURI);
            CreditsManager.Instance.ClickedCredits += System.Convert.ToInt32(!wasClicked);
            wasClicked = true;
            PlayerPrefs.SetInt(webURI, 1);
        }
        else
        {
            Debug.LogWarning("webURI is not set or is empty.");
        }
    }
}
