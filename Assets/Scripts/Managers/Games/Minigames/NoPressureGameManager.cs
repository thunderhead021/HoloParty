public class NoPressureGameManager : BaseGameManager
{
	public override void Countdown()
	{
		CountdownWithPlayerCards();
	}

	public override bool GameIsEnded()
    {
        return LastManStandingMinigameTypeWinCondition() /*false*/;
    }
}