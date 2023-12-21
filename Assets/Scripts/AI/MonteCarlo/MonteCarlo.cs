using System;
using System.Collections.Generic;
using System.Linq;
using Board;
using Core.Actor;
using UnityEngine;

namespace AI.MonteCarlo
{
    public class MonteCarlo : MonoBehaviour, IScript
    {
        public int depth = 6;
        public int computationalBudget = 100;
        private int nodes;
        private readonly Average average = new();
        
        public int ExecuteAlgorithm(BoardState boardState, int turn)
        {
            nodes = 0;
            ActorTurn actorTurn = new ActorTurn(turn);
            Node.SetActorTurn(actorTurn);

            int currentDepth = depth;
            if (boardState.Capacity - boardState.DiscInUse < depth)
                currentDepth = boardState.Capacity - boardState.DiscInUse;
                
            // Creation of root node
            MctsNode startNode = new MctsNode(boardState, actorTurn.Opponent, currentDepth-1, null);
            
            // Algorithm
            MctsNode result = MonteCarloAlgorithm(startNode);
            average.Add(nodes);
            Debug.Log($"value: {result.RewardsCount/result.VisitCount}, column: {result.ColumnSelected}, nodes: {nodes}, media: {average.Value}");

            // result
            return result.ColumnSelected;
        }

        private MctsNode MonteCarloAlgorithm(MctsNode currentNode)
        {
            int currentComputationalBudget = computationalBudget;
            
            while (currentComputationalBudget > 0)
            {
                MctsNode childNode = TreePolicy(currentNode);
                float reward = DefaultPolicy(childNode);
                BackUp(childNode, reward);
                --currentComputationalBudget;
            }
            MctsNode selected = BestChild(currentNode, 0);
            return selected; 
        }

        private MctsNode TreePolicy(MctsNode node)
        {
            MctsNode selected = node;
            
            while (selected.NonTerminal)
            {
                if (!selected.IsFullyExpanded())
                {
                    ++nodes;
                    return Expand(ref selected);
                }
                selected = BestChild(selected, 0.75f);
            }
            return selected;
        }
        
        private MctsNode BestChild(MctsNode node, float constant)
        {
            float maxValue = Int32.MinValue;
            List<MctsNode> list = node.ChildNodes;
            list.RemoveAll(mctsNode => mctsNode == null);

            MctsNode selected = null;
            foreach (var childNode in list) 
            {
                float formula = childNode.RewardsCount/childNode.VisitCount +
                                constant * Mathf.Sqrt(2 * Mathf.Log10(node.VisitCount) / childNode.VisitCount);
                if (formula > maxValue)
                {
                    maxValue = formula;
                    selected = childNode;
                }
            }
            return selected;
        }

        private MctsNode Expand(ref MctsNode node)
        {
            List<MctsNode> possibleMoves = node.PossibleMoves();
            MctsNode.Shuffle(possibleMoves);
            
            MctsNode selected = possibleMoves.First();
            node.AddNode(selected);
            return selected;
        }

        private float DefaultPolicy(MctsNode node)
        {
            // Win or Lose condition
            // Simulate until the end randomly.
            while (node.NonTerminal)
            {
                List<MctsNode> children = node.PossibleMoves();
                MctsNode.Shuffle(children);
                node = children.First();
            }
            
            return node.GetTerminalReward;
        }

        // Backpropagation
        private void BackUp(MctsNode node, float reward)
        {
            MctsNode actualNode = node;
            
            while (actualNode != null)
            {
                actualNode.AddVisit();
                actualNode.UpdateReward(reward);
                actualNode = actualNode.Parent;
            }
        }
    }
}
