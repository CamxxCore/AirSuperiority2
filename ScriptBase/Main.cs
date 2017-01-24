using System;
using System.Windows.Forms;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Logic;

namespace AirSuperiority.ScriptBase
{
    public class Main : ScriptThread
    {
        public Main()
        {
            PreInit();
            KeyDown += OnKeyDown;
        }

        private void PreInit()
        {
            AddExtension("sess", new SessionManager());
            AddExtension("input", new InputManager());
            AddExtension("ui", new DisplayManager());
            AddExtension("map", new LevelManager());
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N)
            {
                var extension = GetExtension("sess") as SessionManager;
                extension.Initialize(0, 12);
            }
        }
    }
}
