using System;
using System.Collections;
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
        
        [Header("Actors")] 
        [SerializeField] private List<PlayerManager> playerList; 
        [SerializeField] private List<AIManager> aiList; 
        private List<ActorManager> _actorList; 
        
        // 0: FirstPlayer, 1: SecondPlayer, ...
        private int _turn;
    
        // private bool _isFinished;
        private ActorManager _currentActor;
    
        // delegate to OnGameTurnChange ActorManager
        public delegate void GameTurnChange();
        public event GameTurnChange OnActorTurnChange;
    

        // Start is called before the first frame update
        private void Start()
        {
            _actorList = new List<ActorManager>(playerList.Count + aiList.Count);
            _actorList.AddRange(playerList);
            _actorList.AddRange(aiList);

            _players = _actorList.Count;
            boardManager.SetupScene();
            boardManager.ForEachColumn(OnMoveDone);

            // Minus one position, it will be added in the ChangeTurn method 
            _turn = startPlayer-1;
        
            ChangeTurn();
        }

        private void RemoveLastActorToDelegate()
        {
            // Unlink last actor method
            OnActorTurnChange -= _actorList[_turn].OnGameTurnChange;
        }
    
        private void ChangeTurn()
        {
            _turn = (_turn + 1) % _players;
        
            // Set current actor
            _currentActor = _actorList[_turn];
        
            // Change settings (like disc color) 
            OnActorTurnChange += _actorList[_turn].OnGameTurnChange;
        
            // execute actor delegate
            // ReSharper disable once PossibleNullReferenceException
            OnActorTurnChange.Invoke();
        }

        private bool CheckActorOwner(Disc disc)
        {
            return disc.ActorOwner == _turn && disc.Visibility;
        }
        
        private bool CheckFinish()
        {
            // check if its full the board
            bool enoughSpace;
            
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
            // Don't continue
            // if(_isFinished) return;
        
            // Get Space
            // Check for free spaces
            // Space spaceOfColumn = boardManager.FirstSpaceInColumn(column);
            // Debug.Log($"{spaceOfColumn.name}");
        
            // Reference to Disc
            Disc disc = boardManager.FirstDiscInColumn(column);
            if(disc == null) return;

            // Send disc to _currentActor
            // Save disc position in another Disc list
            // _currentActor.AddDiscToActor(disc);
            disc.SetActorOwner(_turn);
            disc.SetColor(_currentActor.GetColor());

            RemoveLastActorToDelegate();

            // Results     
            if (CheckFinish())
            {
                boardManager.ForEachColumnRemove(OnMoveDone);
                Debug.Log("WINNN!");
            }
            else
            {
                ChangeTurn();
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
