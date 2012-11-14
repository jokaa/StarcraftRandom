using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Starcraft2.ReplayParser;

namespace StarcraftRandom
{
  public class ReplayHandler
  {
    public event EventHandler<ReplayEventArgs> NewReplay;
    public event EventHandler<ProgressEventArgs> ProgressUpdate;
    
    public ReplayHandler()
    {
      this.watcher = new FileSystemWatcher
      {
        Filter = "*.SC2Replay",
        Path = Properties.Settings.Default.ReplayFolder,
        NotifyFilter = NotifyFilters.LastWrite
      };

      this.watcher.Created += OnNewReplayFile;
      this.watcher.Changed += OnNewReplayFile;
      this.watcher.EnableRaisingEvents = true;

      this.filtered = 0;
      this.progress = 0;
      this.alreadyParsed = new List<string>();
    }

    public void LoadReplayFolder()
    {
      var replayFiles = Directory.GetFiles(Properties.Settings.Default.ReplayFolder, "*.SC2Replay", SearchOption.AllDirectories);
      
      double totalReplaysToParse = replayFiles.Length;
      double replaysParsed = 0;

      foreach (string replayFile in replayFiles)
      {
        this.progress = 100 * ++replaysParsed / totalReplaysToParse;
        this.ParseReplay(replayFile);
      }
    }
    
    private void OnNewReplayFile(object sender, FileSystemEventArgs args)
    {
      this.progress = 100;
      this.ParseReplay(args.FullPath);
    }

    private void ParseReplay(string path)
    {
      if (!this.alreadyParsed.Contains(path))
      {
        this.alreadyParsed.Add(path);

        if (this.NewReplay != null)
        {
          Replay replay = Replay.Parse(path);

          if (Filter(replay))
          {
            this.filtered++;
          }
          else
          {
            this.NewReplay(this, new ReplayEventArgs(path, replay));
          }
        }

        if (this.ProgressUpdate != null)
        {
          this.ProgressUpdate(this, new ProgressEventArgs(this.progress, this.alreadyParsed.Count, this.filtered));
        }
      }
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

    private double progress;
    private int filtered;
    private readonly List<string> alreadyParsed;
    private readonly FileSystemWatcher watcher;
  }
}
