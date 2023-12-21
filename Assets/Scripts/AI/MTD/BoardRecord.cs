namespace AI.MTD
{
    public class BoardRecord
    {
        public int HashValue;
        public int MinScore;
        public int MaxScore;
        public int BestMove;
        public int Depth;

        public BoardRecord()
        {
            HashValue = 0;
            MinScore = 0;
            MaxScore = 0; 
            BestMove = 0;
            Depth = 0;
        }
    }
}