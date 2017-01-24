using System;
using System.Collections.Generic;
using System.Linq;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Helpers
{
    public sealed class Leaderboard : ManagedScaleform
    {
        private ManagedScaleform uiButtons;

        object[][] leaderboardData = new object[0][];

        int scrollIndex = 0;

        public Leaderboard() : base("sc_leaderboard")
        {
            uiButtons = new ManagedScaleform("instructional_buttons");
        }


        public void SetTitle(string text)
        {
            CallFunction("SET_MULTIPLAYER_TITLE", text);
        }

        public void SetColumns(params string[] columnNames)
        {
            CallFunction("SET_TITLE", columnNames);
        }

        public void AddItem(int index, SlotState state, params object[] columnValues)
        {
            if (leaderboardData.Length <= index)
                Array.Resize(ref leaderboardData, index + 1);

            var paramsObj = new List<object> { index, (int)state };

            paramsObj.AddRange(columnValues);

            var args = paramsObj.ToArray();

            if (leaderboardData.Length < 19)
                CallFunction("SET_SLOT", args);

            leaderboardData[index] = args;
        }

        public void SetSlotState(int index, SlotState state)
        {
            CallFunction("SET_SLOT_STATE", index, (int)state);
        }

        public void SetDisplayType(DisplayType type)
        {
            CallFunction("SET_DISPLAY_TYPE", (int)type);
        }

        public void HandleScrollUp()
        {
            if (scrollIndex == 0) return;

            scrollIndex -= 2;

            for (int i = 0; i < 19; i++)
            {
                // get item from the leaderboard data list based on the scroll index.
                var item = leaderboardData[i + scrollIndex];

                // get item data
                var index = (int)item[0];
                var state = item[1];
                var columnValues = item.Skip(2).ToArray();

                // use index 1- 19 to display the item and retain the state value.
                List<object> paramsObj = new List<object> { i, state };

                // re- add column data.
                paramsObj.AddRange(columnValues);

                // repack the items into an array and make the params function happy.
                var args = paramsObj.ToArray();

                CallFunction("SET_SLOT", args);
            }

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
        }

        public void HandleScrollDown()
        {
            if (scrollIndex > leaderboardData.Length - 21) return;

            scrollIndex += 2;

            for (int i = 0; i < 19; i++)
            {
                // get item from the leaderboard data list based on the scroll index.
                var item = leaderboardData[i + scrollIndex];

                var index = (int)item[0];
                var state = item[1];
                var columnValues = item.Skip(2).ToArray();

                // use index 1- 19 to display the item and retain the state value.
                List<object> paramsObj = new List<object> { i, state };

                // re- add column data.
                paramsObj.AddRange(columnValues);

                // repack the items into an array and make the params function happy.
                var args = paramsObj.ToArray();

                CallFunction("SET_SLOT", args);
            }

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
        }

        public void ClearAllSlots()
        {
            CallFunction("CLEAR_ALL_SLOTS");
            leaderboardData = new string[0][];
            scrollIndex = 0;
        }

        private void RenderIntructionalButtons()
        {
            uiButtons.CallFunction("CLEAR_ALL");
            uiButtons.CallFunction("TOGGLE_MOUSE_BUTTONS", false);

            string str = Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 202, 0);

            uiButtons.CallFunction("SET_DATA_SLOT", 4, str, "Exit");
            uiButtons.CallFunction("SET_BACKGROUND_COLOUR", 0, 0, 0, 80);
            uiButtons.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", 0);

            Function.Call((Hash)0x0DF606929C105BE1, uiButtons.BaseInstance.Handle, 255, 255, 255, 255);
        }

        public void Draw()
        {
            Render();

            RenderIntructionalButtons();
        }

        public enum DisplayType
        {
            Minigame,
            Multiplayer
        }

        public enum SlotState
        {
            Normal,
            Player,
            Outline,
            Selected,
            World,
            Friends,
            Crew,
            Description
        }
    }
}
