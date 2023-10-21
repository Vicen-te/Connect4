using Board;
using Interaction;
using UnityEngine;

namespace AI
{
    public class MiniMax : MonoBehaviour, IScript
    {
        public Column ExecuteAlgorithm(BoardInfo boardInfo)
        {
            int random = UnityEngine.Random.Range(0, boardInfo.Columns.Count);
            
            /*  Expand 8 positions -> Create Candidates
             *  Example: 
             *      new new new 
             *      new act new
             *      new new new
             *
             *  for each position (check if it's not null)
             *  MiniMax algorithm
             */
            
            
            return boardInfo.Columns[random];
        }
    }
}