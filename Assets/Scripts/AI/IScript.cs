using Board;

namespace AI
{
    public interface IScript
    {
       int ExecuteAlgorithm(BoardState boardState, int turn);
    }
}