namespace BlazorBattles.Shared;

public class BattleHistoryEntry
{
    public int BattleId { get; set; }
    public DateTime BattleDate { get; set; }
    public int AttackerId { get; set; }
    public int OpponentId { get; set; }
    public bool YouWon { get; set; }
    public string AttackerName { get; set; }
    public string OpponentName { get; set; }
    public int RoundFought { get; set; }
    public int WinnerDamageDealt { get; set; }
}