using System;
using System.Collections.Generic;

namespace StarcraftRandom
{
  public static class ChallengeFactory
  {
    public static Challenge Create()
    {
      var dice = new Random();
      string race = Races[dice.Next(3)];

      switch (dice.Next(8))
      {
        case 1:
          return new Challenge(race, "For two games", 2);
        case 2:
          return new Challenge(race, "For three games", 3);
        case 3:
          return new Challenge(race, "Until you lose a game", 1, WinLoseCondition.Lose);
        case 4:
          return new Challenge(race, "Until you win a game", 1, WinLoseCondition.Win);
        case 5:
          return new Challenge(race, "Until you beat a terran", 1, WinLoseCondition.Win, Constants.Terran);
        case 6:
          return new Challenge(race, "Until you beat a zerg", 1, WinLoseCondition.Win, Constants.Zerg);
        case 7:
          return new Challenge(race, "Until you beat a protoss", 1, WinLoseCondition.Win, Constants.Protoss);
        default:
          return new Challenge(race, "For one game", 1);
      }
    }

    private static readonly Dictionary<int, string> Races = new Dictionary<int, string>
			{
				{ 0, Constants.Zerg },
				{ 1, Constants.Terran },
				{ 2, Constants.Protoss }
			};
  }
}
