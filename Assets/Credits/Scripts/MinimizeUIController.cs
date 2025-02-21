using UnityEngine;
using UnityEngine.EventSystems;

public class MinimizeUIController : MonoBehaviour, IPointerClickHandler
{
    public bool CanBeMinimized = true;

    [HideInInspector] public Delegates.StringDelegate OnError;
    [HideInInspector] public Animator animator;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!CanBeMinimized)
        {
            OnError?.Invoke($"Open at least {CreditsManager.Instance.MinimumCreditsToClick} different people's credits to be able to minimize this!");
            return;
        }
        OnError?.Invoke("Credits\n(Click us!)");
        bool currentValue = animator.GetBool("BreedingUIIsMaximized");

        animator.SetBool("BreedingUIIsMaximized", !currentValue);
    }
}
