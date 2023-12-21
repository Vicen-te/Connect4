using System.Collections;
using System.Collections.Generic;
using AI.MTD;
using Board;
using Core.Actor;
using Interaction;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private int _actors = 0;

        [Header("Settings")] [SerializeField] private int startPlayer = 0;

        [Header("References")] [SerializeField]
        private BoardManager boardManager;

        private BoardInfo BoardInfo => boardManager.BoardInfo;
        private BoardState _boardState;

        [Header("Actors")] [SerializeField] private List<ActorManager> actorList;

        [Header("AI")] 
        [SerializeField] private float waitSecondsInteraction = 0.2f;
        [SerializeField] private float waitSecondsPlays = 1f;
        [SerializeField] private int automatePlays = 100;
        private int automatePlaysRemain;

        // 0: FirstPlayer, 1: SecondPlayer, ...
        private int _turn;
    
        // private bool _isFinished;
        private ActorManager _currentActor;

        [Header("UI")] 
        [SerializeField] private UIManager uiManager;
        
        private void Start()
        {
            int aiCount = 0;
            foreach (ActorManager actorManager in actorList)
            {
                if (actorManager.GetType() == typeof(AIManager))
                {
                    AIManager aiManager = (AIManager)actorManager;
                    aiManager.GetClassWithInterface();
                    aiManager.OnInteraction += OnMoveDone;
                    aiManager.SetWaitSeconds(waitSecondsInteraction);
                    ++aiCount;
                }
            }
            
            // Actors Count
            _actors = actorList.Count;

            // Check AICount 
            if (aiCount > 2 || (_actors > 2 && aiCount > 0))
            {
                Debug.LogError("Only can be two AIs");
                return;
            }
            
            boardManager.SetupScene();
            automatePlaysRemain = automatePlays;
            StartValues();
        }

        private void StartValues()
        {
            // Minus one position, it will be added in the ChangeTurn method 
            _turn = startPlayer-1;

            _boardState = new BoardState(BoardInfo);
        
            ChangeTurn();
            CheckActor();
            
            uiManager.Initialize(_boardState, automatePlays);
        }
    
        private void ChangeTurn()
        {
            _turn = (_turn + 1) % _actors;
            
            // Set current actor
            _currentActor = actorList[_turn];
            
            // Execute AI algorithm
            _currentActor.OnGameTurnChange(BoardInfo, _boardState, _turn);
        }

        // Change settings (column interactions) 
        private void CheckActor()
        {
            // Turn column interaction on if it is the player's turn or off if it is the AI's turn
            ColumnsIteration(_currentActor.GetType() == typeof(PlayerManager));
        }
        
        private void ColumnsIteration(bool active)
        {
            if (active)
                boardManager.ForEachColumnAddInteraction(OnMoveDone);
            else
                boardManager.ForEachColumnRemoveInteraction(OnMoveDone);
        }

        private void OnMoveDone(Column column)
        {
            // BoardState
            int columnIndex = BoardInfo.ColumnIndex(column);
            
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
            uiManager.UpdateDiscs(_boardState);
            
            ColumnsIteration(false);
            
            // Results     
            if (_boardState.Winner(_turn))
            {
                // Debug.Log($"WIN: {_turn}!!!");
                if (_turn == 0)
                    uiManager.AddFirstWin(automatePlays);
                else
                    uiManager.AddSecondWin(automatePlays);
                
                Restart();
                _boardState.PrintResult($"Win {automatePlays-automatePlaysRemain} - Turn {_turn}");
            }
            else if (_boardState.Draw())
            {
                // Debug.Log("DRAW!!!");
                uiManager.AddDraw(automatePlays);
                
                Restart();
                _boardState.PrintResult($"Draw");
            }
            else
            {
                ChangeTurn();
                CheckActor();
            }
        }
        
        private void Restart()
        {
            // Subtract play
            --automatePlaysRemain;
            _boardState.PrintDiscs();

            if (automatePlaysRemain == 0)
            {
                // ExitGame();
                return;
            }

            foreach (ActorManager actorManager in actorList)
            {
                actorManager.TryGetComponent(out Mtd mtd);
                if (mtd != null)
                {
                    mtd.Initialize(_boardState.Capacity);
                }
            }

            StartCoroutine(ResetBoard());
        }

        // Reset Board
        private IEnumerator ResetBoard()
        {
            yield return new WaitForSeconds(waitSecondsPlays);
            boardManager.ResetScene();
            StartValues();
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
