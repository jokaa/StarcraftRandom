using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;

using Starcraft2.ReplayParser;

namespace StarcraftRandom
{
  public class Challenge
  {
    public string Race
    {
      get;
      private set;
    }

    public string ConditionText
    {
      get;
      private set;
    }

    public bool Complete
    {
      get
      {
        return this.count <= 0;
      }
    }

    public Challenge(string race, string conditionText, int count, WinLoseCondition winLose = WinLoseCondition.None, string opponentRace = "")
    {
      this.Race = race;
      this.ConditionText = conditionText;
      
      this.count = count;
      this.opponentRace = opponentRace;
      this.winLose = winLose;
    }
    
    public void OnNewReplay(object sender, ReplayEventArgs args)
    {
      string playerName = Properties.Settings.Default.PlayerName;

      Player me = args.Replay.Players.FirstOrDefault(p => p != null && p.Name == playerName);
      Player opponent = args.Replay.Players.FirstOrDefault(p => p != null && p.Name != playerName);

      if (me != null && opponent != null)
      {
        if (me.Race == this.Race &&
          (string.IsNullOrEmpty(this.opponentRace) || this.opponentRace == opponent.Race) &&
          ((me.IsWinner && this.winLose == WinLoseCondition.Win) || (!me.IsWinner && this.winLose == WinLoseCondition.Lose) || this.winLose == WinLoseCondition.None))
        {
          this.count--;

          if (this.Complete)
          {
            var player = new SoundPlayer(Properties.Resources.success);
            player.Play();
          }
        }
      }
    }

    private int count;
    private readonly string opponentRace;
    private readonly WinLoseCondition winLose;
  }
}
