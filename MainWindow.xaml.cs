// ////
// //// Jonathan Karlsson
// //// 2012-10-27
// ////

using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Timers;
using System.Windows.Threading;

using Starcraft2.ReplayParser;

using StarcraftRandom.Properties;

namespace StarcraftRandom
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var f9 = new KeyboardHandler(this, 0x78); // 0x78 = F9
      f9.KeyPressed += F9KeyPressed;

      this.ShowWinrates();

      this.replayHandler = new ReplayHandler();
      this.replayHandler.NewReplay += this.matchupsStatistics.OnNewReplay;
      this.replayHandler.ProgressUpdate += this.matchupsStatistics.OnProgressUpdate;

      InitialLoadDelegate del = this.replayHandler.LoadReplayFolder;
      del.BeginInvoke(null, null);
      
			this.timer = new Timer();
			this.timer.Elapsed += (obj, args) => this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(this.ShowWinrates));
			this.timer.Interval = 5000;
      this.timer.AutoReset = false;
    }
    
		private void F9KeyPressed(object sender, EventArgs e)
		{
      if (this.challengeDisplay.Challenge == null || this.challengeDisplay.Challenge.Complete)
      {
        if (this.challengeDisplay.Challenge != null)
        {
          this.replayHandler.NewReplay -= this.challengeDisplay.Challenge.OnNewReplay;
        }

        this.challengeDisplay.Challenge = ChallengeFactory.Create();
        this.replayHandler.NewReplay += this.challengeDisplay.Challenge.OnNewReplay;
      }

      this.ShowChallenge();

      this.timer.Stop();
			this.timer.Start();
		}

		private void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void OnExit(object sender, ExecutedRoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F8)
			{
				ShowWinrates();
			}
		}

		private void ShowWinrates()
		{
      this.challengeDisplay.Visibility = Visibility.Hidden;
      this.matchupsStatistics.Visibility = Visibility.Visible;
		}

    private void ShowChallenge()
    {
      this.matchupsStatistics.Visibility = Visibility.Hidden;
      this.challengeDisplay.Visibility = Visibility.Visible;
    }

    private delegate void InitialLoadDelegate();

    private readonly ReplayHandler replayHandler;
		private readonly Timer timer;
	}
}