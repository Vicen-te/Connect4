using System.Collections.Generic;
using Board;
using Core.Actor;
using UnityEngine;

namespace AI.MTD
{
    public class Mtd : MonoBehaviour, IScript
    {
        private ZobristKey _zobristKey;
        private TranspositionTable _transpositionTable;
        public int hashTableLength = 90000000;
        private int minimumExploredDepth = 0;
        private int globalGuess = int.MaxValue; // INFINITE
        public int maxIterations = 10;
        public int depth = 6;
        
        private int nodes;
        private readonly Average average = new();

        public void Initialize(int boardCapacity)
        {
            _zobristKey = new ZobristKey(boardCapacity, 2);
            _zobristKey.Print();
            _transpositionTable = new TranspositionTable(hashTableLength);
        }
        
        public int ExecuteAlgorithm(BoardState boardState, int turn)
        {
            nodes = 0;
            ActorTurn actorTurn = new ActorTurn(turn);            
            Node.SetActorTurn(actorTurn);

            if (_zobristKey == null)
            {
                Initialize(boardState.Capacity);
            }

            ZobristNode startNode = new ZobristNode(boardState, actorTurn.Opponent, _zobristKey);
            NodeMove result = MtdAlgorithm(startNode);
            
            average.Add(nodes);
            Debug.Log($"value: {-result.Score}, column: {result.Column}, nodes: {nodes}, media: {average.Value}");
            
            return result.Column;
        }
        
        private NodeMove MtdAlgorithm(ZobristNode currentNode)
        {
            int guess = globalGuess;
            NodeMove scoringMove = new NodeMove();
            minimumExploredDepth = 0;

            for (int i = 0; i < maxIterations; i++)
            {   
                int gamma = guess;
                ZobristNode iterateNode = currentNode;

                scoringMove = Test (iterateNode, depth-1, gamma - 1);
                guess = scoringMove.Score;
                
                if (gamma == guess)
                {
                    globalGuess = guess;
                    Debug.Log($"Guess: {guess}, found on iteration {i + 1}");
                    return scoringMove;
                }
                
                Debug.Log("Guess not found");
            }
            globalGuess = guess;
            return scoringMove;
        }
        
        private NodeMove Test (ZobristNode currentNode, int actualDepth, int gamma)
        {
            if (actualDepth < minimumExploredDepth) minimumExploredDepth = actualDepth;
            
            // Buscar si ya tenemos un registro del tablero en la tabla de trasposición.
            BoardRecord record = _transpositionTable.GetRecord (currentNode.HasValue);
            
            // Si tenemos un registro de este tablero.
            if (record != null)
            {
                // Si la profundidad es adecuada
                if (record.Depth > actualDepth)
                {
                    // Si el score se ajusta al valor gamma que arrastramos, entonces devolvemos la jugada adecuada.
                    if (record.MinScore > gamma)
                        return new NodeMove (record.MinScore, record.BestMove);
                    if (record.MaxScore < gamma)
                        return new NodeMove (record.MaxScore, record.BestMove);
                }
            }	
            // Si no tenemos un registro de este tablero, lo "inicializamos".
            else
            {
                record = new BoardRecord
                {
                    HashValue = currentNode.HasValue,
                    Depth = actualDepth,
                    MinScore = int.MinValue + 1,
                    MaxScore = int.MaxValue
                };
            }

            ++nodes;
            // Ahora que ya tenemos un registro empezamos a buscar jugada.
            // Si estamos al final de la recursión.
            if (currentNode.IsEndOfGame() || actualDepth == 0)
            {
                record.MaxScore = currentNode.Evaluate();
                if (actualDepth != 0)
                {
                    record.MaxScore = actualDepth % 2 == 0 ? record.MaxScore : -record.MaxScore;
                    record.MaxScore *= actualDepth+1;
                }
                record.MaxScore = depth % 2 == 0 ? -record.MaxScore : record.MaxScore;
                
                record.MinScore = record.MaxScore;
                _transpositionTable.SaveRecord(record);
                return new NodeMove (record.MaxScore, currentNode.ColumnSelected);
            }
            
            //No es estado final o suspensión  
            int maxEval = int.MinValue+1;
            int column = 0;
            
            List<ZobristNode> possibleMoves = currentNode.PossibleMoves();
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                if (possibleMoves[i] == null) continue;

                // Recursividad
                NodeMove currentMove = Test(possibleMoves[i], actualDepth-1, -gamma);
                currentMove.InverseScore();

                // Actualizar mejor score
                if (maxEval < currentMove.Score)
                {
                    column = possibleMoves[i].ColumnSelected;
                    record.BestMove = column;
                    maxEval = currentMove.Score;
                }

                if (maxEval < gamma) 
                    record.MaxScore = maxEval;
                else
                    record.MinScore = maxEval;
            }

            _transpositionTable.SaveRecord(record);
            return new NodeMove(maxEval, column); 
        }
    }
}