using System.Drawing;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Logic;
using GTA;
using GTA.Math;
using GTA.Native;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// Fighter jet engine extinguisher extension.
    /// </summary>
    public class EntityInfoOverlay : PlayerExtensionBase
    {
        SessionManager sessionMgr;

        public EntityInfoOverlay(ScriptThread thread, Player player) : base(thread, player)
        {
            sessionMgr = thread.Get<SessionManager>();
        }

        /// <summary>
        /// Draw entity heads up display.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="color"></param>
        /// <param name="subText"></param>
        /// <param name="sizeMultiplier"></param>
        /// <param name="withText"></param>
        /// <param name="drawRect"></param>
        private static void DrawSquare(Point location, Color color, string subText, float sizeMultiplier, bool withText, bool drawRect = true)
        {
            if (drawRect)
            {
                UIRectangle rect;
                rect = new UIRectangle(new Point((int)((location.X - 25) * sizeMultiplier), (int)(location.Y * sizeMultiplier)), new Size((int)(4 * sizeMultiplier), (int)(54 * sizeMultiplier)));
                rect.Color = color;
                rect.Draw();
                rect = new UIRectangle(new Point((int)((location.X + 25) * sizeMultiplier), (int)(location.Y * sizeMultiplier)), new Size((int)(4 * sizeMultiplier), (int)(54 * sizeMultiplier)));
                rect.Color = color;
                rect.Draw();
                rect = new UIRectangle(new Point((int)((location.X - 25) * sizeMultiplier), (int)((location.Y + 54) * sizeMultiplier)), new Size((int)(52 * sizeMultiplier), (int)(4 * sizeMultiplier)));
                rect.Color = color;
                rect.Draw();
                rect = new UIRectangle(new Point((int)((location.X - 25) * sizeMultiplier), (int)(location.Y * sizeMultiplier)), new Size((int)(52 * sizeMultiplier), (int)(4 * sizeMultiplier)));
                rect.Color = color;
                rect.Draw();
            }

            if (withText)
            {
                var text = new UIText(subText, new Point((int)((location.X - 5) * sizeMultiplier), (int)((location.Y + 60) * sizeMultiplier)), 0.3f, Color.White, GTA.Font.ChaletComprimeCologne, false);
                text.Draw();
            }
        }

        /// <summary>
        /// Get viewport camera position in world coords.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCameraPosition()
        {
            return (Function.Call<bool>(Hash.DOES_CAM_EXIST, World.RenderingCamera.Handle) == false ?
                GameplayCamera.Position :
                World.RenderingCamera.Position);
        }

        public override void OnUpdate(int gameTime)
        {
            var pos = Player.Position;

            float dist = (pos - GetCameraPosition()).Length();

            float scale = 6.0f / dist;

            Function.Call(Hash.SET_DRAW_ORIGIN, pos.X, pos.Y, pos.Z, 0);

          //  var color = sessionMgr.GetTeamByIndex(Player.Info.Sess.TeamNum).TeamColor;

            DrawSquare(new Point(0, -32), Color.Red,
                string.Format("{0} dist: {1} team: {2}", Player.Name, Player.Position.DistanceTo(Game.Player.Character.Position), Player.Info.Sess.TeamNum), scale * 0.37f,
                dist > 0.42f,
                dist > 0.58f);

            Function.Call(Hash.CLEAR_DRAW_ORIGIN);

            base.OnUpdate(gameTime);
        }

    }
}
