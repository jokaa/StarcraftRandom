using System;

namespace StarcraftRandom
{
  public class ProgressEventArgs : EventArgs
  {
    public double Progress
    {
      get;
      private set;
    }

    public int Total
    {
      get;
      private set;
    }

    public int Filtered
    {
      get;
      private set;
    }

    public ProgressEventArgs(double progress, int total, int filtered)
    {
      this.Progress = progress;
      this.Total = total;
      this.Filtered = filtered;
    }
  }
}
