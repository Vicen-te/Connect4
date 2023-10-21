using System.Collections.Generic;
using Board;
using Interaction;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private int _players = 0;
    
        [Header("Settings")] 
        [SerializeField] private int startPlayer = 0;
    
        [Header("References")] 
        [SerializeField] private BoardManager boardManager;
        private BoardInfo BoardInfo => boardManager.BoardInfo;
        
        [Header("Actors")] 
        [SerializeField] private List<PlayerManager> playerList; 
        [SerializeField] private List<AIManager> aiList; 
        private List<ActorManager> _actorList; 
        
        // 0: FirstPlayer, 1: SecondPlayer, ...
        private int _turn;
    
        // private bool _isFinished;
        private ActorManager _currentActor;
    
        // Not enough space
        private bool _full;
        
        // Start is called before the first frame update
        private void Start()
        {
            foreach (AIManager aiManger in aiList)
            {
                aiManger.GetClassWithInterface();
                aiManger.OnInteraction += OnMoveDone;
            }
            
            _actorList = new List<ActorManager>(playerList.Count + aiList.Count);
            _actorList.AddRange(playerList);
            _actorList.AddRange(aiList);

            _players = _actorList.Count;
            boardManager.SetupScene();

            // Minus one position, it will be added in the ChangeTurn method 
            _turn = startPlayer-1;
        
            ChangeTurn();
            CheckActor();
        }

        private void ColumnsIteration(bool active)
        {
            if (active)
                boardManager.ForEachColumnAddInteraction(OnMoveDone);
            else
                boardManager.ForEachColumnRemoveInteraction(OnMoveDone);
        }
    
        private void ChangeTurn()
        {
            _turn = (_turn + 1) % _players;
        
            // Set current actor
            _currentActor = _actorList[_turn];
        
            // Change settings (like column interactions) 
            _currentActor.OnGameTurnChange(BoardInfo);
        }

        private void CheckActor()
        {
            // Turn column off if it is the AI's turn
            if (_currentActor.GetType() == typeof(AIManager))
            {
                ColumnsIteration(false);
            }
            // Turn column interaction on if it is the player's turn
            else
            {
                ColumnsIteration(true);
            }
        }

        private bool CheckActorOwner(Disc disc)
        {
            return disc.ActorOwner == _turn && disc.Visibility;
        }
        
        private bool CheckFinish()
        {
            // check if its full the board
            int amount = 0;
            foreach (Disc disc in BoardInfo.Discs)
            {
                if (disc.Visibility) ++amount;
            }

            if (amount == BoardInfo.Discs.Count)
            {
                return _full = true;
            }
            
            // Vertical Check 
            if (boardManager.CheckConnect4
                (
                    new KeyValuePair<int, int>(0, 1),
                    new KeyValuePair<int, int>(0, 2),
                    new KeyValuePair<int, int>(0, 3),
                    CheckActorOwner,
                    (disc1, disc2, disc3, disc4) =>
                    {
                        return (disc2.GamePosition.Key == disc1.GamePosition.Key &&
                                disc3.GamePosition.Key == disc1.GamePosition.Key &&
                                disc4.GamePosition.Key == disc1.GamePosition.Key &&
            
                                disc2.GamePosition.Value == disc1.GamePosition.Value + 1 &&
                                disc3.GamePosition.Value == disc1.GamePosition.Value + 2 &&
                                disc4.GamePosition.Value == disc1.GamePosition.Value + 3);
                    }
                )
               )
                return true;

            // Horizontal Check 
            if (boardManager.CheckConnect4
                (   new KeyValuePair<int, int>(1, 0),
                    new KeyValuePair<int, int>(2, 0),
                    new KeyValuePair<int, int>(3, 0),
                    CheckActorOwner,
                    (disc1, disc2, disc3, disc4) =>
                    {
                        return (disc2.GamePosition.Key == disc1.GamePosition.Key + 1 &&
                                disc3.GamePosition.Key == disc1.GamePosition.Key + 2 &&
                                disc4.GamePosition.Key == disc1.GamePosition.Key + 3 &&
            
                                disc2.GamePosition.Value == disc1.GamePosition.Value &&
                                disc3.GamePosition.Value == disc1.GamePosition.Value &&
                                disc4.GamePosition.Value == disc1.GamePosition.Value);
                    }
                )
               )
                return true;
            
            // Ascending Diagonal Check 
            if (boardManager.CheckConnect4
                (
                    new KeyValuePair<int, int>(-1, 1),
                    new KeyValuePair<int, int>(-2, 2),
                    new KeyValuePair<int, int>(-3, 3),
                    CheckActorOwner,
                    (disc1, disc2, disc3, disc4) =>
                    {
                        return (disc2.GamePosition.Key == disc1.GamePosition.Key - 1 &&
                                disc3.GamePosition.Key == disc1.GamePosition.Key - 2 &&
                                disc4.GamePosition.Key == disc1.GamePosition.Key - 3 &&
            
                                disc2.GamePosition.Value == disc1.GamePosition.Value + 1 &&
                                disc3.GamePosition.Value == disc1.GamePosition.Value + 2 &&
                                disc4.GamePosition.Value == disc1.GamePosition.Value + 3);
                    }
                )
               )
                return true;
            
            // Descending Diagonal Check
            if (boardManager.CheckConnect4
                (
                    new KeyValuePair<int, int>(-1, -1),
                    new KeyValuePair<int, int>(-2, -2),
                    new KeyValuePair<int, int>(-3, -3),
                    CheckActorOwner,
                    (disc1, disc2, disc3, disc4) =>
                    {
                        return (disc2.GamePosition.Key == disc1.GamePosition.Key - 1 &&
                                disc3.GamePosition.Key == disc1.GamePosition.Key - 2 &&
                                disc4.GamePosition.Key == disc1.GamePosition.Key - 3 &&
            
                                disc2.GamePosition.Value == disc1.GamePosition.Value - 1 &&
                                disc3.GamePosition.Value == disc1.GamePosition.Value - 2 &&
                                disc4.GamePosition.Value == disc1.GamePosition.Value - 3);
                    }
                )
               )
                return true;
            
            return false;
        }

        private void OnMoveDone(Column column)
        {
            // Reference to Disc
            Disc disc = BoardInfo.FirstDiscInColumn(column);
            
            // Execute the last turn again
            if (!disc)
            {
                _turn -= 1;
                ChangeTurn();
                return;
            }
            
            // Change disc values
            disc.SetVisibility(true);
            // disc.StartAnimation();
            disc.SetActorOwner(_turn);
            disc.SetColor(_currentActor.GetColor());

            ColumnsIteration(false);

            // Results     
            if (CheckFinish())
            {
                if(_full)
                    Debug.Log("FULL!");
                else
                    Debug.Log("WINNN!");
            }
            else
            {
                ChangeTurn();
                CheckActor();
            }
        }
    }
}
