// ////
// //// Jonathan Karlsson
// //// 2012-10-27
// ////

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Timers;
using System.Windows.Threading;

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

			this.watcher = new FileSystemWatcher();
			watcher.Filter = "*.SC2Replay";
			watcher.Path = Properties.Settings.Default.ReplayFolder;
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Created += (obj, args) => this.matchupsStatistics1.NewReplay(args.FullPath);
			watcher.Changed += (obj, args) => this.matchupsStatistics1.NewReplay(args.FullPath);
			watcher.EnableRaisingEvents = true;

			this.timer = new Timer();
			this.timer.Elapsed += (obj, args) => this.matchupsStatistics1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(ShowWinrates));
			this.timer.Interval = 3000;
			this.timer.AutoReset = false;

			ShowWinrates();
		}

		private void F9KeyPressed(object sender, EventArgs e)
		{
			matchupsStatistics1.Visibility = Visibility.Hidden;
			label1.Visibility = Visibility.Visible;

			ShowImage();
			ShowRaceText();

			this.timer.Start();
		}

		private void ShowRaceText()
		{
			var dice = new Random();
			int roll = dice.Next(8);

			label1.Content = conditions[roll];

			var da2 = new DoubleAnimation
				{From = 0, To = 1, AccelerationRatio = 0.9, Duration = new Duration(TimeSpan.FromMilliseconds(1500))};
			label1.BeginAnimation(OpacityProperty, da2);
		}

		private void ShowImage()
		{
			var dice = new Random();
			int roll = dice.Next(3);

			zeImage.Visibility = Visibility.Visible;
			zeImage.Source =
				new BitmapImage(new Uri(@"/StarcraftRandom;component/Images/" + icons[roll], UriKind.RelativeOrAbsolute));
			var da = new DoubleAnimation {From = 0, To = 1, Duration = new Duration(TimeSpan.FromMilliseconds(1000))};
			zeImage.BeginAnimation(OpacityProperty, da);
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
			zeImage.Visibility = Visibility.Hidden;
			label1.Visibility = Visibility.Hidden;
			matchupsStatistics1.Visibility = Visibility.Visible;

			if (matchupsStatistics1.MathcupsStats == null)
			{
				matchupsStatistics1.UpdateMatchups();
			}
		}

		private readonly FileSystemWatcher watcher;
		private readonly Timer timer;

		private readonly Dictionary<int, string> icons = new Dictionary<int, string>
			{
				{ 0, "Zerg.jpg" },
				{ 1, "Terran.jpg" },
				{ 2, "Protoss.jpg" }
			};

		private readonly Dictionary<int, string> conditions = new Dictionary<int, string>
			{
				{ 0, "For 1 game" },
				{ 1, "For 2 games" },
				{ 2, "For 3 games" },
				{ 3, "Until you lose a game" },
				{ 4, "Until you win a game" },
				{ 5, "Until you beat a terran" },
				{ 6, "Until you beat a protoss" },
				{ 7, "Until you beat a zerg" },
			};
	}
}