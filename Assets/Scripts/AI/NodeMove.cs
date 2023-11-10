namespace AI
{
   public enum Actor { Player, AI }
    public struct NodeMove
    {
        public int Score;
        public int Column;

        public NodeMove(int value, int column)
        {
            Score = value;
            Column = column;
        }

        public void InverseScore()
        {
            Score = -Score;
        }
    }
}