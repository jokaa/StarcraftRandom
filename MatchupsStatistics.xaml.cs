// ////
// //// Jonathan Karlsson
// //// 2012-10-27
// ////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Starcraft2.ReplayParser;

namespace StarcraftRandom
{
	/// <summary>
	/// Interaction logic for MatchupsStatistics.xaml
	/// </summary>
	public partial class MatchupsStatistics
	{
    public Dictionary<string, Dictionary<string, Tuple<double, double>>> MathcupsStats
    {
      get;
      set;
    }

		public MatchupsStatistics()
		{
			InitializeComponent();
      this.MathcupsStats = new Dictionary<string, Dictionary<string, Tuple<double, double>>>();
      this.totalReplays = 0;
      this.filteredReplays = 0;
		}

    public void OnNewReplay(object sender, ReplayEventArgs args)
    {
      string playerName = Properties.Settings.Default.PlayerName;

      Player me = args.Replay.Players.FirstOrDefault(p => p != null && p.Name == playerName);
      Player opponent = args.Replay.Players.FirstOrDefault(p => p != null && p.Name != playerName);

      if (me != null && opponent != null)
      {
        if (!this.MathcupsStats.ContainsKey(me.Race))
        {
          this.MathcupsStats[me.Race] = new Dictionary<string, Tuple<double, double>>();
        }

        if (!this.MathcupsStats[me.Race].ContainsKey(opponent.Race))
        {
          this.MathcupsStats[me.Race][opponent.Race] = Tuple.Create(0.0, 0.0);
        }

        double wins = this.MathcupsStats[me.Race][opponent.Race].Item1 + (me.IsWinner ? 1 : 0);
        double total = this.MathcupsStats[me.Race][opponent.Race].Item2 + 1;

        this.MathcupsStats[me.Race][opponent.Race] = Tuple.Create(wins, total);
      }
		}

    public void OnProgressUpdate(object sender, ProgressEventArgs args)
    {
      this.filteredReplays = args.Filtered;
      this.totalReplays = args.Total - args.Filtered;
      this.progressBar1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<double>(this.UpdateProgressBar), args.Progress);
      this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(this.Refresh));
    }

		private void UpdateProgressBar(double value)
		{
		  this.progressBar1.Visibility = value < 100 ? Visibility.Visible : Visibility.Hidden;
      this.progressBar1.Value = value;
		}

	  private void Refresh()
		{
      tvtLabel.Content = CalcWinrate(Constants.Terran, Constants.Terran).ToString(CultureInfo.InvariantCulture) + "%";
      tvzLabel.Content = CalcWinrate(Constants.Terran, Constants.Zerg).ToString(CultureInfo.InvariantCulture) + "%";
      tvpLabel.Content = CalcWinrate(Constants.Terran, Constants.Protoss).ToString(CultureInfo.InvariantCulture) + "%";
      zvtLabel.Content = CalcWinrate(Constants.Zerg, Constants.Terran).ToString(CultureInfo.InvariantCulture) + "%";
      zvzLabel.Content = CalcWinrate(Constants.Zerg, Constants.Zerg).ToString(CultureInfo.InvariantCulture) + "%";
      zvpLabel.Content = CalcWinrate(Constants.Zerg, Constants.Protoss).ToString(CultureInfo.InvariantCulture) + "%";
      pvtLabel.Content = CalcWinrate(Constants.Protoss, Constants.Terran).ToString(CultureInfo.InvariantCulture) + "%";
      pvzLabel.Content = CalcWinrate(Constants.Protoss, Constants.Zerg).ToString(CultureInfo.InvariantCulture) + "%";
      pvpLabel.Content = CalcWinrate(Constants.Protoss, Constants.Protoss).ToString(CultureInfo.InvariantCulture) + "%";

      tTotalLabel.Content = CalcTotalWinrate(Constants.Terran).ToString(CultureInfo.InvariantCulture) + "%";
      zTotalLabel.Content = CalcTotalWinrate(Constants.Zerg).ToString(CultureInfo.InvariantCulture) + "%";
      pTotalLabel.Content = CalcTotalWinrate(Constants.Protoss).ToString(CultureInfo.InvariantCulture) + "%";
			
			filteredLabel.Content = "Parsed: " + this.totalReplays + "  Filtered: " + this.filteredReplays;
		}

		private double CalcWinrate(string race, string opponentRace)
		{
			if (!MathcupsStats.ContainsKey(race) || !MathcupsStats[race].ContainsKey(opponentRace) || (MathcupsStats[race][opponentRace].Item2 == 0.0))
			{
				return 0;
			}

			return Math.Round(100 * MathcupsStats[race][opponentRace].Item1 / MathcupsStats[race][opponentRace].Item2, 1);
		}

		private double CalcTotalWinrate(string race)
		{
			double total = CalcTotal(race);

			if (total == 0.0)
			{
				return 0;
			}

			double wins = 0;

			if (MathcupsStats.ContainsKey(race))
			{
        if (MathcupsStats[race].ContainsKey(Constants.Terran))
				{
          wins += MathcupsStats[race][Constants.Terran].Item1;
				}
        if (MathcupsStats[race].ContainsKey(Constants.Zerg))
				{
          wins += MathcupsStats[race][Constants.Zerg].Item1;
				}
        if (MathcupsStats[race].ContainsKey(Constants.Protoss))
				{
          wins += MathcupsStats[race][Constants.Protoss].Item1;
				}
			}

			return Math.Round(100 * wins / total, 1);
		}

		private double CalcTotal(string race)
		{
			double total = 0;

			if (MathcupsStats.ContainsKey(race))
			{
        if (MathcupsStats[race].ContainsKey(Constants.Terran))
				{
          total += MathcupsStats[race][Constants.Terran].Item2;
				}
        if (MathcupsStats[race].ContainsKey(Constants.Zerg))
				{
          total += MathcupsStats[race][Constants.Zerg].Item2;
				}
        if (MathcupsStats[race].ContainsKey(Constants.Protoss))
				{
          total += MathcupsStats[race][Constants.Protoss].Item2;
				}
			}

			return total;
		}

    private int totalReplays;
    private int filteredReplays;
	}
}