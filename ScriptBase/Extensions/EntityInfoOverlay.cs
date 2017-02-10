using System.Drawing;
using AirSuperiority.Core;
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

        PilotAIController aiController;

        public EntityInfoOverlay(Player player) : base(player)
        {
            sessionMgr = ScriptThread.GetOrAddExtension<SessionManager>();
        }

        public override void OnPlayerAttached(Player player)
        {
            aiController = player.GetExtension<PilotAIController>();

            base.OnPlayerAttached(player);
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
                string.Format("{0} dist: {1} team: {2} state: {3}", Player.Name, Player.Position.DistanceTo(Game.Player.Character.Position), Player.Info.Sess.TeamNum, aiController.State.Status.ToString()), scale * 0.37f,
                dist > 0.42f,
                dist > 0.58f);

            Function.Call(Hash.CLEAR_DRAW_ORIGIN);

            if (Player.ActiveTarget != null )
            {
                var tp = Player.ActiveTarget.Position;

                var direction = tp - pos;

                Vector3 horz = Vector3.Cross(direction, new Vector3(0, 0, 1.0f));

                Vector3 posA = tp + horz * 0.0034f;

                Vector3 posB = tp + -horz * 0.0034f;

                Function.Call(Hash.DRAW_LINE, pos.X, pos.Y, pos.Z, tp.X, tp.Y, tp.Z, 255, 0, 0, 255);

                Function.Call(Hash.DRAW_LINE, posA.X, posA.Y, posA.Z, posB.X, posB.Y, posB.Z, 0, 255, 0, 255);
            }

            base.OnUpdate(gameTime);
        }

    }
}
