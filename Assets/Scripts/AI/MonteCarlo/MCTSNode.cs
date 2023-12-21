using System.Collections.Generic;
using System.Linq;
using Board;

namespace AI.MonteCarlo
{
    public class MctsNode : Node
    {
        public readonly int Depth;
        public float VisitCount { get; private set; } 
        public float RewardsCount { get; private set; } 
        
        // double linked list
        public List<MctsNode> ChildNodes { get; private set; }
        public MctsNode Parent { get; private set; } = null;
        
        /// <summary>
        /// Normalize the value from -1 (lose) to 1(win), 0(draw).
        /// </summary>
        public float GetTerminalReward => (float)Evaluate() / 600;
        public bool NonTerminal => Depth >= 0 && !IsEndOfGame();

        /// <summary>
        /// Check if one child is null and if can place a disc on the remaining column (is not full) 
        /// </summary>
        public bool IsFullyExpanded()
        {
            bool expanded = ChildNodes.Count(node => node != null) == BoardState.Columns;
            if (expanded) return true;
            
            bool canMove = PossibleMoves().Count > 0;
            return !canMove;
        }
        
        public void UpdateReward(float reward)
        {
            RewardsCount += reward;
        }
        public void AddVisit()
        {
            ++VisitCount;
        }
        public MctsNode(BoardState boardState, int turn, int depth, MctsNode parent) : base(boardState, turn)
        {
            Depth = depth;
            ChildNodes = new List<MctsNode>(new MctsNode[boardState.Columns]);
            Parent = parent;
        }
        
        public new List<MctsNode> PossibleMoves()
        {
            List<MctsNode> list = new List<MctsNode>();
            
            for (int i = 0; i < BoardState.Columns; ++i)
            {
                bool empty = BoardState.IsColumnEmpty(i);
                if (!empty || ChildNodes.Count <= i || ChildNodes[i] != null) continue;
                MctsNode node = CreateNode(i);
                list.Add(node);
            }
            return list;
        }
        
        private MctsNode CreateNode(int column)
        {
            int nextTurn = (Turn + 1) % 2;
            MctsNode node = new MctsNode(BoardState, nextTurn, Depth - 1, this);
            node.Position = node.BoardState.FirstDiscInColumn(column);
            node.BoardState.AddDisc(node.Position, nextTurn);
            node.ColumnSelected = column;
            return node;
        }
        public void AddNode(MctsNode node)
        {
            ChildNodes[node.ColumnSelected] = node;
        }
        public static void Shuffle<T>(IList<T> list)
        {
            System.Random random = new System.Random();
            int n = list.Count;  
            
            while (n > 1) {  
                n--;  
                int k = random.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
    }
}
