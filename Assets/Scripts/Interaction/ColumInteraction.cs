namespace Interaction
{
    public interface IColumInteraction
    {
        public delegate void Interaction(Column column);
        public event Interaction OnInteraction;
    }
}