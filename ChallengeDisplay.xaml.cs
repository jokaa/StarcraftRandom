using System;
using System.Windows.Media.Imaging;

namespace StarcraftRandom
{
  /// <summary>
  /// Interaction logic for ChallengeDisplay.xaml
  /// </summary>
  public partial class ChallengeDisplay
  {
    public Challenge Challenge
    {
      get
      {
        return this.challenge;
      }
      set
      {
        this.challenge = value;
        this.zeImage.Source = new BitmapImage(new Uri(@"/StarcraftRandom;component/Images/" + this.challenge.Race + ".jpg", UriKind.RelativeOrAbsolute));
        this.zeLabel.Content = this.challenge.ConditionText;
      }
    }

    public ChallengeDisplay()
    {
      InitializeComponent();
    }

    private Challenge challenge;
  }
}
