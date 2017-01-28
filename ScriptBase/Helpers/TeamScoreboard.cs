using System;
using System.IO;
using System.Drawing;
using AirSuperiority.ScriptBase.Types;
using GTA;

namespace AirSuperiority.ScriptBase.Helpers
{
    public class TeamScoreboard
    {
        private struct TeamScoreboardEntry
        {
            public int ScoreValue { get; set; }
            public string TeamName { get; set; }
            public string SpritePath { get; set; }
        }

        public const int MaxTeamSlots = 12;

        public const int TeamImageXOffset = 10;

        public const int ProgressionBarXOffset = 52;

        private UIContainer infoContainer;

        private TeamScoreboardEntry[] entries;

        private int numAvailableSlots = 0;

        public TeamScoreboard()
        {
            infoContainer = new UIContainer(new Point((int)0.00115f * Game.ScreenResolution.Width + 960, UI.HEIGHT / 6 - 20),
                new Size(180, 31),
                Color.FromArgb(180, Color.Black));
        }

        private void SetupScoreboard(int numSlots)
        {
            if (numSlots > numAvailableSlots)
            {
                infoContainer = new UIContainer(new Point((int)0.00115f * Game.ScreenResolution.Width + 960, UI.HEIGHT / 6 - 20), 
                    new Size(180, numSlots * 31), Color.FromArgb(180, Color.Black));

                UIRectangle[] containerItems = new UIRectangle[numSlots << 1];

                for (int i = 0; i < numSlots; i++)
                {
                    var color = (TeamColor)i;

                    containerItems[i] = new UIRectangle(new Point(), new Size(0, 11), Color.FromArgb(180, Color.Orange));

                    containerItems[i + numSlots] = new UIRectangle(new Point(22, (30 * i) + 12), new Size(21, 13), Color.FromArgb(180, color.ToSystemColor()));
                }

                infoContainer.Items.AddRange(containerItems);

                Array.Resize(ref entries, numSlots);

                numAvailableSlots = numSlots;
            }
        }

        /// <summary>
        /// Set leaderboard score for the given team.
        /// </summary>
        /// <param name="teamIdx"></param>
        /// <param name="score"></param>
        public void SetTeamScore(int teamIdx, int score)
        {
            if (teamIdx + 1 > Math.Min(numAvailableSlots, MaxTeamSlots)) return;
            entries[teamIdx].ScoreValue = score;
        }

        /// <summary>
        /// Sets the texture asset and team name for a slot on the leaderboard.
        /// The leaderboard is resized to accomodate the new item if ineeded.
        /// </summary>
        /// <param name="teamIdx"></param>
        /// <param name="friendlyName"></param>
        /// <param name="imageAssetPath"></param>
        public void SetTeamAsset(int teamIdx, string friendlyName, string imageAssetPath)
        {
            var slotsNeeded = teamIdx + 1;

            if (slotsNeeded > MaxTeamSlots) return;

            if (slotsNeeded > numAvailableSlots)
                SetupScoreboard(slotsNeeded);

            TeamScoreboardEntry entry = new TeamScoreboardEntry
            {
                ScoreValue = 0,
                TeamName = friendlyName,
                SpritePath = imageAssetPath,
            };

            entries[teamIdx] = entry;
        }

        public void Draw()
        {
            if (infoContainer != null)
            {
                for (int i = 0; i < numAvailableSlots; i++) // hud progression bars are in first part
                {
                    if (infoContainer.Items[i] != null)
                    {
                        //update progress bar.
                        infoContainer.Items[i] = new UIRectangle(new Point(ProgressionBarXOffset, 12 + (30 * i)), new Size(entries[i].ScoreValue, 11), Color.Orange);

                        if (entries[i].SpritePath != null && File.Exists(Resources.BaseDirectory + entries[i].SpritePath))
                        {
                            UI.DrawTexture(Resources.BaseDirectory + entries[i].SpritePath, 0, 0, 100,
                                new Point(infoContainer.Position.X + TeamImageXOffset, infoContainer.Position.Y + 2 + 30 * i),
                                new Size(24, 24));
                        }

                        else
                        {
                            if (entries[i].TeamName != null)
                            {
                                var text = new UIText(entries[i].TeamName + ":",
                                    new Point(infoContainer.Position.X + 10, infoContainer.Position.Y + 2 + 30 * i),
                                    0.4f, Color.White, GTA.Font.Monospace, false);

                                text.Draw();
                            }
                        }
                    }
                }

                infoContainer.Draw();
            }
        }
    }
}

