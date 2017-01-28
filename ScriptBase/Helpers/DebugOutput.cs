using System;
using GTA;
using System.Drawing;

namespace AirSuperiority.ScriptBase.Helpers
{
    /// <summary>
    /// Lightweight class for writing debug output to the screen.
    /// </summary>
    public class DebugOutput
    {
        const int TextActiveTime = 5000;

        private int textAddedTime = 0;

        private int linesCount;

        private UIContainer backsplash;

        private string[] messageQueue = new string[10];

        private UIText[] text = new UIText[10];

        /// <summary>
        /// Add a new line to the message queue.
        /// </summary>
        /// <param name="text"></param>
        public void AddLine(string text)
        {
            backsplash.Color = Color.FromArgb(140, Color.Black);

            SetTextColor(Color.White);

            for (int i = messageQueue.Length - 1; i > 0; i--)
                messageQueue[i] = messageQueue[i - 1];

            messageQueue[0] = string.Format("~4~~y~[{0}]   ~w~{1}", DateTime.Now.ToShortTimeString(), text);

            linesCount = Math.Min(linesCount + 1, messageQueue.Length);

            textAddedTime = Game.GameTime;
        }

        private void SetTextColor(Color color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i].Color = color;
            }
        }

        public DebugOutput()
        {
            backsplash = new UIContainer(new Point(600, 500), new Size(600, 200), Color.Empty);
            CreateText();
        }

        private void CreateText()
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = new UIText(string.Empty, new Point(14, 11 + (18 * i)), 0.3f, Color.Empty);
            }

            backsplash.Items.AddRange(text);
        }

        public void Update()
        {
            if (Game.GameTime > textAddedTime + TextActiveTime)
            {
                if (backsplash.Color.A > 0)
                    backsplash.Color = Color.FromArgb(Math.Max(0, backsplash.Color.A - 2), backsplash.Color);

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i].Color.A > 0)
                    {
                        text[i].Color = Color.FromArgb(Math.Max(0, text[i].Color.A - 4), text[i].Color);
                    }
                }
            }

            else
            {
                for (int i = text.Length - 1; i > -1; i--)
                {
                    text[i].Caption = messageQueue[(messageQueue.Length - 1) - i] ?? string.Empty;
                }

            }

            backsplash.Draw();
        }
    }
}
