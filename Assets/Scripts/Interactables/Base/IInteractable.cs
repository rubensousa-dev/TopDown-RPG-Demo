
public interface IInteractable
{
    public bool IsInteractable();
    public bool Interact(PlayerInteractor interactor);
    void SetHighlight(bool active);
}
