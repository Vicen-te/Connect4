namespace Interaction
{
    public interface IColumInteraction
    {
        public delegate void Interaction(Column space);
        public event Interaction OnInteraction;
    }
}