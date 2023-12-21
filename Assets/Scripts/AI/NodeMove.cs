namespace AI
{
    public struct NodeMove
    {
        public int Score;
        public readonly int Column;

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