using System;
using GTA;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Helpers
{
    public delegate void RankBarRankEvent(RankBar sender, UIRankBarEventArgs e);

    public class RankBar : GlobalScaleform
    {
        private int duration;
        private int animSpeed;
        private int newXP;
        private int colour;
        private int currentRank;
        private int currentRankXP;
        private bool rankOverflow;
        private int rankOverflowTick;

        public event RankBarRankEvent RankedUp;

        public RankBar() : base(19)
        { }

        /// <summary>
        /// Loads the rank bar to be used next function call.
        /// </summary>
        protected void LoadRankBar()
        {
            Function.Call((Hash)0x9304881D6F6537EA, 19); //REQUEST_HUD_SCALEFORM

            while (!Function.Call<bool>(Hash._HAS_HUD_SCALEFORM_LOADED, 19))
            {
                Script.Wait(0);
            }
        }

        /// <summary>
        /// Display the rank bar with the specified arguments.
        /// </summary>
        /// <param name="currentRank"></param>
        /// <param name="currentXP">The target XP for the next rank</param>
        /// <param name="nextRank"></param>
        /// <param name="nextXP"></param>
        /// <param name="colour"></param>
        /// <param name="duration"></param>
        /// <param name="animationSpeed"></param>
        public void Show(int currentRank, int currentXP, int newXP, int colour = 116, int duration = 1000, int animationSpeed = 1000)
        {
            LoadRankBar();
            ResetBarText();
            SetColour(colour);
            SetDuration(duration);
            SetAnimationSpeed(animationSpeed);
            var rankLimit = RankData[currentRank + 2];
            SetRankScores(RankData[currentRank], rankLimit, currentXP, currentXP + newXP > rankLimit ? rankLimit - 1 : currentXP + newXP, currentRank, 100, currentRank + 1);
            Show();

            if (currentXP + newXP >= rankLimit)
            {
                this.currentRank = currentRank + 1; //rank to display on the left side
                this.currentRankXP = rankLimit; //xp floor for the next rank
                this.newXP = newXP; //xp floor for the next rank plus new xp to get the remainder
                this.rankOverflowTick = (Game.GameTime + duration) + 1000;
                this.duration = duration;
                this.animSpeed = animationSpeed;
                this.colour = colour;
                this.rankOverflow = true;
            }
        }

        protected virtual void OnRankedUp(UIRankBarEventArgs e)
        {
            Game.PlaySound("MEDAL_UP", "HUD_MINI_GAME_SOUNDSET");
            Game.PlaySound("RANK_UP", "HUD_AWARDS");
            RankedUp?.Invoke(this, e);
        }

        /// <summary>
        /// Shows the rank bar.
        /// </summary>
        protected void Show()
        {
            CallFunction("SHOW");
        }

        /// <summary>
        /// Sets the hud colour for the rank bar.
        /// </summary>
        /// <param name="colour"></param>
        protected void SetColour(int colour)
        {
            CallFunction("SET_COLOUR", colour);
        }

        /// <summary>
        /// Sets the duration the rank bar is on screen.
        /// </summary>
        /// <param name="duration"></param>
        protected void SetDuration(int duration)
        {
            CallFunction("OVERRIDE_ONSCREEN_DURATION", duration);
        }

        /// <summary>
        /// Sets the speed of the rank up animation.
        /// </summary>
        protected void SetAnimationSpeed(int speed)
        {
            CallFunction("OVERRIDE_ANIMATION_SPEED", speed);
        }

        /// <summary>
        /// Sets the speed of the rank up animation.
        /// </summary>
        protected void FadeBarOut(bool remove = false)
        {
            CallFunction("FADE_BAR_OUT", remove);
        }

        /// <summary>
        /// Sets the speed of the rank up animation.
        /// </summary>
        protected void ResetBarText()
        {
            CallFunction("RESET_BAR_TEXT");
        }

        /// <summary>
        /// Set the score values for the rank bar.
        /// </summary>
        /// <param name="currentRankLimit"></param>
        /// <param name="nextRankLimit"></param>
        /// <param name="playerPreviousXP"></param>
        /// <param name="playerCurrentXP"></param>
        /// <param name="rank"></param>
        /// <param name="alpha"></param>
        /// <param name="nextRank"></param>
        protected void SetRankScores(int currentRankLimit, int nextRankLimit, int playerPreviousXP, int playerCurrentXP, int rank, int alpha, int nextRank)
        {
            CallFunction("SET_RANK_SCORES", currentRankLimit, nextRankLimit, playerPreviousXP, playerCurrentXP, rank, alpha, nextRank);
        }

        public void Update()
        {
            if (rankOverflow && Game.GameTime > rankOverflowTick)
            {
                rankOverflow = false;
                OnRankedUp(new UIRankBarEventArgs(currentRank, currentRank + 1));
                ResetBarText();
                CallFunction("STAY_ON_SCREEN");
                Show(currentRank, currentRankXP, newXP, colour, duration, animSpeed);
            }
        }

        public static readonly int[] RankData = new int[]
        {
            0, 89, //0
            90, 199, //1
            200, 349, //2
            350, 499, //3
            500, 739, //4
            740, 999, //5
            1000, 1599, //6
            1600, 1999, //7
            2000, 2699, //8
            2700, 3499, //9
            3500, 4199, //10
            4200, 5399, //11
            5400, 6599, //12
            6600, 7999, //13
            8000, 9299,  //14
            9300, 10899, //15
            10900, 12199, //16
            12200, 13499, //17
            13500, 13899, //18
            13900, 14299, //19
            14300, 14799, //20
            14800, 15299, //21
            15300, 15799, //22
            15800, 16399, //23
            16400, 16999, //24
            17000, 18399, //25
            18400, 18999,  //26
            19000, 24299, //27
            24300, 24999, //28
            25000, 29999, //29
            30000, 100000,
            100000, 100000
        };
    }

    /// <summary>
    /// Event args for UIRankBar rank up events
    /// </summary>
    public class UIRankBarEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize the class
        /// </summary>
        public UIRankBarEventArgs(int previousRank, int newRank)
        {
            PreviousRank = previousRank;
            NewRank = newRank;
        }

        /// <summary>
        /// The previous rank
        /// </summary>
        public int PreviousRank { get; private set; }

        /// <summary>
        /// The rank that was reached
        /// </summary>
        public int NewRank { get; private set; }
    }
}
