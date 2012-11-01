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
using System.Windows.Controls;
using System.Windows.Threading;
using Starcraft2.ReplayParser;

namespace StarcraftRandom
{
	/// <summary>
	/// Interaction logic for MatchupsStatistics.xaml
	/// </summary>
	public partial class MatchupsStatistics : UserControl
	{
		public Dictionary<string, Dictionary<string, Tuple<double, double>>> MathcupsStats { get; set; }

		public MatchupsStatistics()
		{
			InitializeComponent();
			this.alreadyProcessed = new List<string>();
		}

		public void UpdateMatchups()
		{
			this.MathcupsStats = new Dictionary<string, Dictionary<string, Tuple<double, double>>>();

			UpdateDelegate del = UpdateMatchupsConcrete;
			progressBar1.Visibility = Visibility.Visible;

			del.BeginInvoke(iar => progressBar1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(Refresh)), null);
		}

		public void NewReplay(string path)
		{
			if (this.MathcupsStats == null)
			{
				UpdateMatchups();
			}

			ProgressReplayFile(path, Properties.Settings.Default.PlayerName);
			progressBar1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(Refresh));
		}

		private void UpdateMatchupsConcrete()
		{
			string folder = Properties.Settings.Default.ReplayFolder;
			string playerName = Properties.Settings.Default.PlayerName;

			var replayFiles = Directory.GetFiles(folder, "*.SC2Replay", SearchOption.AllDirectories);

			double totalFiles = replayFiles.Length;
			double progressed = 0;

			foreach (string replayFile in replayFiles)
			{
				ProgressReplayFile(replayFile, playerName);
				progressed++;
				progressBar1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<double>(UpdateProgressBar), 100 * progressed / totalFiles);
			}
		}

		private void UpdateProgressBar(double value)
		{
			progressBar1.Value = value;
		}

		private void ProgressReplayFile(string replayFile, string playerName)
		{
			if (this.alreadyProcessed.Contains(replayFile))
			{
				return;
			}

			this.alreadyProcessed.Add(replayFile);

			Replay replay = Replay.Parse(replayFile);

			if (!Filter(replay))
			{
				Player me = replay.Players.FirstOrDefault(p => p != null && p.Name == playerName);
				Player opponent = replay.Players.FirstOrDefault(p => p != null && p.Name != playerName);

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
			else
			{
				this.filteredReplays++;
			}

			this.totalReplays++;
		}

		private static bool Filter(Replay replay)
		{
			bool ok = 
				replay.TeamSize == "1v1" &&
				replay.GameType == GameType.Open &&
				(replay.GameLength >= Properties.Settings.Default.MinimumLength || replay.GameLength == TimeSpan.Zero) &&
				replay.Timestamp >= Properties.Settings.Default.FromDate &&
				replay.Players.Any(p => p != null && p.Name == Properties.Settings.Default.PlayerName);

			return !ok;
		}

		private void Refresh()
		{
			progressBar1.Visibility = Visibility.Hidden;

			tvtLabel.Content = CalcWinrate(Terran, Terran).ToString(CultureInfo.InvariantCulture) + "%";
			tvzLabel.Content = CalcWinrate(Terran, Zerg).ToString(CultureInfo.InvariantCulture) + "%";
			tvpLabel.Content = CalcWinrate(Terran, Protoss).ToString(CultureInfo.InvariantCulture) + "%";
			zvtLabel.Content = CalcWinrate(Zerg, Terran).ToString(CultureInfo.InvariantCulture) + "%";
			zvzLabel.Content = CalcWinrate(Zerg, Zerg).ToString(CultureInfo.InvariantCulture) + "%";
			zvpLabel.Content = CalcWinrate(Zerg, Protoss).ToString(CultureInfo.InvariantCulture) + "%";
			pvtLabel.Content = CalcWinrate(Protoss, Terran).ToString(CultureInfo.InvariantCulture) + "%";
			pvzLabel.Content = CalcWinrate(Protoss, Zerg).ToString(CultureInfo.InvariantCulture) + "%";
			pvpLabel.Content = CalcWinrate(Protoss, Protoss).ToString(CultureInfo.InvariantCulture) + "%";

			tTotalLabel.Content = CalcTotalWinrate(Terran).ToString(CultureInfo.InvariantCulture) + "%";
			zTotalLabel.Content = CalcTotalWinrate(Zerg).ToString(CultureInfo.InvariantCulture) + "%";
			pTotalLabel.Content = CalcTotalWinrate(Protoss).ToString(CultureInfo.InvariantCulture) + "%";
			
			filteredLabel.Content = "Total: " + this.totalReplays + "  Filtered: " + this.filteredReplays;
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
				if (MathcupsStats[race].ContainsKey(Terran))
				{
					wins += MathcupsStats[race][Terran].Item1;
				}
				if (MathcupsStats[race].ContainsKey(Zerg))
				{
					wins += MathcupsStats[race][Zerg].Item1;
				}
				if (MathcupsStats[race].ContainsKey(Protoss))
				{
					wins += MathcupsStats[race][Protoss].Item1;
				}
			}

			return Math.Round(100 * wins / total, 1);
		}

		private double CalcTotal(string race)
		{
			double total = 0;

			if (MathcupsStats.ContainsKey(race))
			{
				if (MathcupsStats[race].ContainsKey(Terran))
				{
					total += MathcupsStats[race][Terran].Item2;
				}
				if (MathcupsStats[race].ContainsKey(Zerg))
				{
					total += MathcupsStats[race][Zerg].Item2;
				}
				if (MathcupsStats[race].ContainsKey(Protoss))
				{
					total += MathcupsStats[race][Protoss].Item2;
				}
			}

			return total;
		}

		private delegate void UpdateDelegate();

		private const string Zerg = "Zerg";
		private const string Protoss = "Protoss";
		private const string Terran = "Terran";
		private readonly List<string> alreadyProcessed = new List<string>();
		private int filteredReplays = 0;
		private int totalReplays = 0;
	}
}