using Board;
using Interaction;

namespace AI
{
    public interface IScript
    {
       Column ExecuteAlgorithm(BoardInfo boardInfo);
    }
}