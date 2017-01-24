using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using System.Drawing;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Types;
using System.IO;

namespace AirSuperiority.ScriptBase.Helpers
{
    public class TeamScoreboard
    {
        private UIContainer infoContainer;

        private static int[] teamScores = new int[Constants.MaxConcurrentTeams];
        private static string[] teamNames = new string[Constants.MaxConcurrentTeams];
        private static string[] teamImages = new string[Constants.MaxConcurrentTeams];

        public const int TeamImageXOffset = 10;

        public const int ProgressionBarXOffset = 52;

        public TeamScoreboard()
        {
            infoContainer = new UIContainer(new Point((int)0.00115f * Game.ScreenResolution.Width + 960, UI.HEIGHT / 6 - 20),
                new Size(180, Constants.MaxConcurrentTeams * 31),
                Color.FromArgb(180, Color.Black));
            PostInit();
        }

        private void PostInit()
        {
            for (int i = 0; i < Constants.MaxConcurrentTeams; i++)
            {
                // create progression bar...
                infoContainer.Items.Add(new UIRectangle(new Point(), new Size(0, 11), Color.FromArgb(180, Color.Orange)));
            }

            for (int i = 0; i < Constants.MaxConcurrentTeams; i++)
            {
                var color = (TeamColor)i;
                // add team color background
                infoContainer.Items.Add(new UIRectangle(new Point(22, (30 * i) + 12), new Size(21, 13), Color.FromArgb(180, color.ToSystemColor())));
            }
        }

        public void SetTeamAsset(int teamIdx, string friendlyName, string imageAssetPath)
        {
            teamNames[teamIdx] = friendlyName;
            teamImages[teamIdx] = imageAssetPath;
            teamScores[teamIdx] = 0;
        }

        public void Draw()
        {
            if (infoContainer != null)
            {
                for (int i = 0; i < Constants.MaxConcurrentTeams; i++) // indexes 0-3 represent hud progression bars
                {
                    if (infoContainer.Items[i] != null)
                    {
                        //update progress bar.
                        infoContainer.Items[i] = new UIRectangle(new Point(ProgressionBarXOffset, 12 + (30 * i)), new Size(teamScores[i], 11), Color.Orange);

                        if (teamImages[i] != null && File.Exists(Resources.BaseDirectory + teamImages[i]))
                        {
                            UI.DrawTexture(Resources.BaseDirectory + teamImages[i], 0, 0, 100,
                                new Point(infoContainer.Position.X + TeamImageXOffset, infoContainer.Position.Y + 2 + 30 * i),
                                new Size(24, 24));
                        }

                        else
                        {
                            if (teamNames[i] != null)
                            {
                                var text = new UIText(teamNames[i] + ":",
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

