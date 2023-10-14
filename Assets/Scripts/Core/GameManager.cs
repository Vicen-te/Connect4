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
        [SerializeField] private List<ActorManager> actorList; 
    
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
            _players = actorList.Count;
            boardManager.SetupScene();
            boardManager.ForEachColumn(OnMoveDone);

            // Minus one position, it will be added in the ChangeTurn method 
            _turn = startPlayer-1;
        
            ChangeTurn();
        }

        private void RemoveLastActor()
        {
            // Unlink last actor method
            OnActorTurnChange -= actorList[_turn].OnGameTurnChange;
        }
    
        private void ChangeTurn()
        {
            _turn = (_turn + 1) % _players;
        
            // Set current actor
            _currentActor = actorList[_turn];
        
            // Change settings (like disc color) 
            OnActorTurnChange += actorList[_turn].OnGameTurnChange;
        
            // execute actor delegate
            // ReSharper disable once PossibleNullReferenceException
            OnActorTurnChange.Invoke();
        }

        private bool CheckFinish()
        {
            // reformat: here will be the code of EvaluateWinCondition
            return _currentActor.EvaluateWinCondition();
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
            _currentActor.AddDiscToActor(disc);
        
            // Results     
            if (CheckFinish())
            {
                RemoveLastActor();
                boardManager.ForEachColumnRemove(OnMoveDone);
                Debug.Log("WINNN!");
            }
            else
            {
                RemoveLastActor();
                ChangeTurn();
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
