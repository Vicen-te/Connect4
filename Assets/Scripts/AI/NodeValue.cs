namespace AI
{
   public enum Actor { Player, AI }
    public struct NodeValue
    {
        public int Value;
        public int Column;

        public NodeValue(int value, int column)
        {
            Value = value;
            Column = column;
        }
    }
}