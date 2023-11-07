using Board;
using Interaction;

namespace AI
{
    public interface IScript
    {
       int ExecuteAlgorithm(BoardState boardState);
    }
}