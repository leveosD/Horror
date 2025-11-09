namespace ItemModule
{
    public class ZeroItem : TakeableItem
    {
        public override IInteractable Interact(IInteractable item)
        {
            return item;
        }
        
        public override bool Activate(IInteractable item)
        {
            return true;
        }
    }
}