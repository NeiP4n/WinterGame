using Sources.Code.Interfaces;
using Sources.Code.UI;
using UnityEngine;

public class BookInteractable : MonoBehaviour, IInteractable
{
    public bool CanInteract => true;

    public void Interact()
    {
        PopupSwitcher.Instance.Show<BookPopup>();
    }
}
