using System;
using System.Reflection;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Native;
using NativeUI;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Logic
{
    public class MenuManager : ScriptExtension
    {
        UIMenu mainMenu;

        MenuPool basePool;

        SessionManager sessionMgr = ScriptThread.GetOrAddExtension<SessionManager>();

        DisplayManager displayMgr = ScriptThread.GetOrAddExtension<DisplayManager>();

        public MenuManager()
        {  
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            mainMenu = new UIMenu("asp beta v" + assemblyName.Version.Revision, string.Empty);
            var item = new UIMenuItem("Start");
            item.Activated += (s, e) => ActivateScript();
            mainMenu.AddItem(item);
            basePool = new MenuPool();
            basePool.Add(mainMenu);
        }

        private void ActivateScript()
        {
            foreach (var vehicle in World.GetAllEntities())
            {
                vehicle.Delete();
            }

            displayMgr.ShowScoreboard();

            //    Utility.FadeScreenOut(900);

            var sessionMgr = ScriptThread.GetOrAddExtension<SessionManager>();

            sessionMgr.Initialize(0, 12, 2);

            Utility.FadeScreenIn(700);
        }

        public override void OnUpdate(int gameTime)
        {
            basePool.ProcessMenus();

            if (Game.IsControlJustPressed(0, Control.ReplayEndpoint))
            {
                mainMenu.Visible = !mainMenu.Visible;
            }

            base.OnUpdate(gameTime);
        }
    }
}
