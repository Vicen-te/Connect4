using Board;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Wins")]
    [SerializeField] private TextMeshProUGUI wins;
    [SerializeField] private TextMeshProUGUI firstWins;
    [SerializeField] private TextMeshProUGUI secondWins;
    
    [Header("Discs")]
    [SerializeField] private TextMeshProUGUI discs;
    [SerializeField] private TextMeshProUGUI firstDiscs;
    [SerializeField] private TextMeshProUGUI secondDiscs;

    [Header("Draws")]
    [SerializeField] private TextMeshProUGUI draws;

    private int drawsValue;
    private int winsValue;
    private int _firstWinsValue;
    private int _secondWinsValue;
    

    public void Initialize(BoardState boardState, int totalPlays)
    {
        UpdateDraws(totalPlays);
        UpdateWins(totalPlays);
        UpdateFirstWins();
        UpdateSecondWins();
        UpdateDiscs(boardState);
    }
    
    public void UpdateDiscs(BoardState boardState)
    {
        discs.text = $"Discs - {boardState.DiscInUse}/{boardState.Capacity}";
        firstDiscs.text = $"First: {boardState.OpponentDisc}";
        secondDiscs.text = $"Second: {boardState.AIDisc}";
    }
    
    public void AddDraw(int totalPlays)
    {
        ++drawsValue;
        UpdateDraws(totalPlays);
    }
    
    private void UpdateDraws(int totalPlays)
    {
        draws.text = $"Draws - {drawsValue}/{totalPlays}";
    }
    
    public void AddFirstWin(int totalPlays)
    {
        ++_firstWinsValue;
        ++winsValue;
        UpdateWins(totalPlays);
        UpdateFirstWins();
    }
    
    public void AddSecondWin(int totalWins)
    {
        ++_secondWinsValue;
        ++winsValue;
        UpdateWins(totalWins);
        UpdateSecondWins();
    }
    
    private void UpdateWins(int totalPlays)
    {
        wins.text = $"Wins - {winsValue}/{totalPlays}";
    }

    private void UpdateFirstWins()
    {
        firstWins.text = $"First: {_firstWinsValue}";
    }
    
    private void UpdateSecondWins()
    {
        secondWins.text = $"Second: {_secondWinsValue}";
    }
}
