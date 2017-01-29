using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GTA;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Helpers
{
    /// <summary>
    /// Fighter HUD related functions.
    /// </summary>
    public class FighterDisplay
    {
        private const int MaxAlpha = 200;

        private const int MinAlpha = 116;

        private UIContainer warningBackground;

        private UIText warningText;

        private bool pulseIn = false;

        public FighterDisplay()
        {
            var location = new Point((int)(UI.WIDTH / 2) - 150, 80);
            warningBackground = new UIContainer(location, new Size(300, 70), Color.FromArgb(130, 55 , 55, 55));
            CreateText();
        }

        private void CreateText()
        {
            warningText = new UIText(string.Empty, new Point(150, 11), 1f, Color.FromArgb(130, Color.Red), GTA.Font.Monospace, true);
            warningBackground.Items.Add(warningText);
        }

        public void ShowWarning(string text)
        {
            warningText.Caption = text;

            bool b = false;

            if (pulseIn)
            {
                /*   if (b |= (warningBackground.Color.A < MaxAlpha))
                   {
                       warningBackground.Color = Color.FromArgb(Math.Min(MaxAlpha, warningBackground.Color.A + 6), warningBackground.Color);
                   }*/
                if (b |= (warningText.Color.A < MaxAlpha))
                {
                    warningText.Color = Color.FromArgb(Math.Min(MaxAlpha, warningText.Color.A + 6), warningText.Color);
                }

                pulseIn = b;
            }

            else
            {
                /*    if (b |= (warningBackground.Color.A > MinAlpha))
                    {
                        warningBackground.Color = Color.FromArgb(Math.Max(MinAlpha, warningBackground.Color.A - 6), warningBackground.Color);
                    }
                    */
                if (b |= (warningText.Color.A > MinAlpha))
                {
                    warningText.Color = Color.FromArgb(Math.Max(MinAlpha, warningText.Color.A - 6), warningText.Color);
                }

                pulseIn = !b;
            }

            warningBackground.Draw();
        }
    }
}
