using System.Collections.Generic;
using Board;
using Core.Actor;

namespace AI
{
    /// <summary>
    /// Represents a node in the game tree used by the AI for decision-making.
    /// Each node represents a state of the game, including the board state, the player's turn, and potential moves.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// The current board state for this node.
        /// </summary>
        protected readonly BoardState BoardState;

        /// <summary>
        /// The actor's turn.
        /// </summary>
        protected static ActorTurn ActorTurn;

        // Properties to get the opponent and AI based on the current turn.
        private static int Opponent => ActorTurn.Opponent;
        private static int AI => ActorTurn.AI;

        /// <summary>
        /// The current player's turn for this node.
        /// </summary>
        protected readonly int Turn;

        /// <summary>
        /// The column selected for the move in this node.
        /// </summary>
        public int ColumnSelected { get; protected set; }

        /// <summary>
        /// The position of the selected disc in the game state.
        /// </summary>
        protected int Position;

        /// <summary>
        /// Sets the current actor's turn for all nodes.
        /// </summary>
        /// <param name="actorTurn">The actor's turn.</param>
        public static void SetActorTurn(ActorTurn actorTurn)
        {
            ActorTurn = actorTurn;
        }

        /// <summary>
        /// Initializes a new node with a given board state and player's turn.
        /// </summary>
        /// <param name="boardState">The current board state.</param>
        /// <param name="turn">The current player's turn.</param>
        public Node(BoardState boardState, int turn)
        {
            BoardState = new BoardState(boardState);
            Turn = turn;
        }

        /// <summary>
        /// Determines if the game has ended (either a win or a draw).
        /// </summary>
        /// <returns>True if the game has ended, otherwise false.</returns>
        public bool IsEndOfGame()
        {
            // Check if there is a winner or if the board is full (draw).
            bool draw = BoardState.Draw();
            bool winner = BoardState.HasWinner(Opponent) || BoardState.HasWinner(AI);
            return draw || winner;
        }

        /// <summary>
        /// Evaluates the current node (game state) for the AI's decision-making.
        /// </summary>
        /// <returns>The evaluation score of the node, with higher scores favoring the AI.</returns>
        public int Evaluate()
        {
            int player = BoardState.Evaluate(Opponent);
            int ai = BoardState.Evaluate(AI);
            
            const int multiplier = 100;
            // The AI prefers moves that result in higher evaluation scores.
            if(ai > player) return ai * multiplier;
            return -player * multiplier;
        }

        /// <summary>
        /// Generates a list of possible moves from the current game state.
        /// </summary>
        /// <returns>A list of nodes representing all possible moves.</returns>
        public List<Node> PossibleMoves()
        {
            List<Node> list = new List<Node>();

            for (int i = 0; i < BoardState.Columns; ++i)
            {
                bool empty = BoardState.IsColumnEmpty(i);
                if (!empty)
                {
                    list.Add(null);
                    continue;
                }
                Node node = GenerateNode(i);
                list.Add(node);
            }
            
            return list;
        }

        /// <summary>
        /// Generates a new node by placing a disc in the specified column.
        /// </summary>
        /// <param name="column">The column in which to place the disc.</param>
        /// <returns>A new node representing the game state after the move.</returns>
        private Node GenerateNode(int column)
        {
            int nextTurn = (Turn + 1) % 2;
            Node node = new Node(BoardState, nextTurn);
            node.Position = node.BoardState.GetNextAvailableDiscIndex(column);
            
            // Place the disc on the board for the new node's game state.
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            node.BoardState.PlaceDisc(node.Position, nextTurn);
            node.ColumnSelected = column;
            return node;
        }
    }
}
