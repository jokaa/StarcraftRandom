using System;
using Starcraft2.ReplayParser;

namespace StarcraftRandom
{
  public class ReplayEventArgs : EventArgs
  {
    public string Path
    {
      get;
      private set;
    }

    public Replay Replay
    {
      get;
      private set;
    }

    public ReplayEventArgs(string path, Replay replay)
    {
      this.Path = path;
      this.Replay = replay;
    }
  }
}
