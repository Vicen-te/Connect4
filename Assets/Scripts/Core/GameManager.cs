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
        private BoardState _boardState;
        
        [Header("Actors")] 
        [SerializeField] private List<PlayerManager> playerList; 
        [SerializeField] private List<AIManager> aiList; 
        [SerializeField] private float waitSeconds = 0.2f; 
        private List<ActorManager> _actorList; 
        
        // 0: FirstPlayer, 1: SecondPlayer, ...
        private int _turn;
    
        // private bool _isFinished;
        private ActorManager _currentActor;
        
        private void Start()
        {
            foreach (AIManager aiManger in aiList)
            {
                aiManger.GetClassWithInterface();
                aiManger.OnInteraction += OnMoveDone;
                aiManger.SetWaitSeconds(waitSeconds);
            }
            
            _actorList = new List<ActorManager>(playerList.Count + aiList.Count);
            _actorList.AddRange(playerList);
            _actorList.AddRange(aiList);

            _players = _actorList.Count;
            boardManager.SetupScene();

            // Minus one position, it will be added in the ChangeTurn method 
            _turn = startPlayer-1;

            _boardState = new BoardState(BoardInfo);
        
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
            _currentActor.OnGameTurnChange(BoardInfo, _boardState);
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

        private void OnMoveDone(Column column)
        {
            // BoardState
            int columnIndex = BoardInfo.ColumnIndex(column);

            if (_boardState.Draw())
            {
                _boardState.PrintDiscs();
                Debug.Log("DRAW!!!");
                // ExitGame();
                return;
            }
            
            // Execute the last turn again
            if (!_boardState.IsColumnEmpty(columnIndex))
            {
                _turn -= 1;
                ChangeTurn();
                return;
            }
            
            int discIndex = _boardState.FirstDiscInColumn(columnIndex);
            _boardState.AddDisc(discIndex, _turn);
            
            // Reference to Disc
            Disc disc = BoardInfo.GetDisc(discIndex);
            
            // Change disc values
            disc.SetVisibility(true);
            // disc.StartAnimation();
            disc.SetColor(_currentActor.GetColor());
            
            ColumnsIteration(false);
            
            // Results     
            if (_boardState.Winner(_turn))
            {
                _boardState.PrintDiscs();
                Debug.Log($"WIN: {_turn}!!!");
                // ExitGame();
            }
            else
            {
                ChangeTurn();
                CheckActor();
            }
        }

        private void ExitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBPLAYER
                Application.OpenURL(webplayerQuitURL);
            #else
                Application.Quit();
            #endif
        }
    }
}
