public class NoPressureGameManager : BaseGameManager
{
    
    public override bool GameIsEnded()
    {
        return LastManStandingMinigameTypeWinCondition() /*false*/;
    }
}