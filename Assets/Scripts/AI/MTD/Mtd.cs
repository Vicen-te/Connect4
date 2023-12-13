using System;
using System.Collections.Generic;
using Board;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        public int ExecuteAlgorithm(BoardState boardState)
        {
            if (_zobristKey == null)
            {
                _zobristKey = new ZobristKey(boardState.Capacity, 2);
                _zobristKey.Print();
                _transpositionTable = new TranspositionTable(hashTableLength);
            }
            
            ZobristNode startNode = new ZobristNode(boardState, (int)Actor.Player, _zobristKey);
            NodeMove result = MtdAlgorithm(startNode);
            Debug.Log($"Final:\n value: {-result.Score}, column: {result.Column}");
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
                    Debug.Log("guess encontrado en iteracion " + i + " guess: " + guess);
                    return scoringMove;
                }
                
                Debug.Log("guess no encontrado");
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
                if (record.depth > actualDepth)
                {
                    // Si el score se ajusta al valor gamma que arrastramos, entonces devolvemos la jugada adecuada.
                    if (record.minScore > gamma)
                        return new NodeMove (record.minScore, record.bestMove);
                    if (record.maxScore < gamma)
                        return new NodeMove (record.maxScore, record.bestMove);
                }
            }	
            // Si no tenemos un registro de este tablero, lo "inicializamos".
            else
            {
                record = new BoardRecord
                {
                    hashValue = currentNode.HasValue,
                    depth = actualDepth,
                    minScore = int.MinValue + 1,
                    maxScore = int.MaxValue
                };
            }

            // Ahora que ya tenemos un registro empezamos a buscar jugada.
            // Si estamos al final de la recursión.
            if (currentNode.IsEndOfGame() || actualDepth == 0)
            {
                record.maxScore = currentNode.Evaluate();
                if (actualDepth != 0)
                {
                    record.maxScore = actualDepth % 2 == 0 ? record.maxScore : -record.maxScore;
                    record.maxScore *= actualDepth+1;
                }
                record.maxScore = depth % 2 == 0 ? -record.maxScore : record.maxScore;
                
                record.minScore = record.maxScore;
                _transpositionTable.SaveRecord(record);
                return new NodeMove (record.maxScore, currentNode.ColumnSelected);
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
                    record.bestMove = column;
                    maxEval = currentMove.Score;
                }

                if (maxEval < gamma) 
                    record.maxScore = maxEval;
                else
                    record.minScore = maxEval;
            }

            _transpositionTable.SaveRecord(record);
            return new NodeMove(maxEval, column); 
        }
    }
}