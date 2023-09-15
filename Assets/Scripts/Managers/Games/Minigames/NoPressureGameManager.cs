using Mirror;

public class NoPressureGameManager : BaseGameManager
{
	public override void Countdown()
	{
		CountdownWithPlayerCards();
	}

	public override void GameIsEnded()
    {
        LastManStandingMinigameTypeWinCondition();
    }

	public override void ShowGameResult()
	{
		ShowResultHelper();
	}
}