using Mirror;

public class NoPressureGameManager : BaseGameManager
{
	[ServerCallback]
	public override void Countdown()
	{
		CountdownWithPlayerCards();
	}

	[ServerCallback]
	public override void GameIsEnded()
    {
        LastManStandingMinigameTypeWinCondition();
    }

	[ServerCallback]
	public override void ShowGameResult()
	{
		ShowResultHelper();
	}
}