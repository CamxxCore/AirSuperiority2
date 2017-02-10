using System;
using System.Drawing;
using System.IO;
using GTA;

namespace AirSuperiority.ScriptBase.Helpers
{
    public class KillPopup : UIContainer
    {
        private string imageAsset;

        private int popupShowTime = 0;

        private int popupVisibleInterval = 0;

        public KillPopup() : base(new Point((UI.WIDTH / 2) - 100, UI.HEIGHT - 78), new Size(200, 60), Color.FromArgb(240, Color.Black))
        { }

        public void SetPlayerInfo(string name, string assetPath, Color splashColor)
        {
            Items.Clear();
            Items.Add(new UIText("AI Player", new Point(14, 16), 0.4f, Color.White, GTA.Font.ChaletComprimeCologne, false));
            Items.Add(new UIText(name, new Point(14, 35), 0.4f, Color.White, GTA.Font.ChaletComprimeCologne, false));
            Items.Add(new UIText("KILLED", new Point(78, 5), 0.34f, Color.White, GTA.Font.Monospace, false));
            Items.Add(new UIRectangle(new Point(166, 40), new Size(21, 13), Color.FromArgb(180, splashColor)));
            imageAsset = assetPath;
        }

        /// <summary>
        /// Show the kill popup with a specified timeout.
        /// </summary>
        /// <param name="timeout">The amount of time the popup will be shown.</param>
        public void ShowTimed(int timeout = 4500)
        {
            popupVisibleInterval = timeout;
            popupShowTime = Game.GameTime;
        }

        /// <summary>
        /// Draw the container.
        /// </summary>
        public override void Draw()
        {
            if (Game.GameTime - popupShowTime < popupVisibleInterval)
            {
                if (File.Exists(Resources.BaseDirectory + imageAsset))
                {
                    UI.DrawTexture(Resources.BaseDirectory + imageAsset, 0, 0, 100, new Point(Position.X + 158, Position.Y + 28), new Size(24, 24));
                }

                base.Draw();
            }
        }
    }
}
