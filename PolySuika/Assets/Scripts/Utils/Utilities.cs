using System.Collections.Generic;

namespace Utilities
{
    public enum ScoreMilestone
    {
        NONE = 0,
        NICE = 8,
        GOOD = 16,
        GREAT = 32,
        SUPER = 64,
        UNREAL = 100,
        INSANE = 1000,
        MAGICAL = 5000,
        EXTREME = 10000,
        ULTIMATE = 100000,
    }

    public static class GConst
    {
        public const int BASE_SCREEN_HEIGHT = 2400;

        public const int TIER_RANK_1 = 2;
        public const int TIER_RANK_2 = 4;
        public const int TIER_RANK_3 = 7;
        public const int NICE_SCORE = (int)ScoreMilestone.NICE;
        public const int GOOD_SCORE = (int)ScoreMilestone.GOOD;
        public const int GREAT_SCORE = (int)ScoreMilestone.GREAT;
        public const int SUPER_SCORE = (int)ScoreMilestone.SUPER;
        public const int UNREAL_SCORE = (int)ScoreMilestone.UNREAL;
        public const int INSANE_SCORE = (int)ScoreMilestone.INSANE;
        public const int MAGICAL_SCORE = (int)ScoreMilestone.MAGICAL;
        public const int EXTREME_SCORE = (int)ScoreMilestone.EXTREME;
        public const int ULTIMATE_SCORE = (int)ScoreMilestone.ULTIMATE;

        public static Dictionary<int, string> ColorDict = new()
        {
                {0, "052B98" },
                {1, "007A98" },
                {2, "067A00" },
                {3, "919800" },
                {4, "AD4500" },
                {5, "982100" },
                {6, "930098" },
                {7, "2700FF" },
                {8, "AAAAAA" }
            };
        public static Dictionary<ScoreMilestone, string> ScoreMilestoneColor = new()
        {
                { ScoreMilestone.NICE, "052B98" },
                { ScoreMilestone.GOOD, "007A98" },
                { ScoreMilestone.GREAT, "067A00" },
                { ScoreMilestone.SUPER, "919800" },
                { ScoreMilestone.UNREAL, "AD4500" },
                { ScoreMilestone.INSANE, "982100" },
                { ScoreMilestone.MAGICAL, "930098" },
                { ScoreMilestone.EXTREME, "2700FF" },
                { ScoreMilestone.ULTIMATE, "AAAAAA" }
            };
    }

    public static class TextUtilities
    {
        public static string RainbowString(string input)
        {
            string result = "";
            for (int i = 0; i < input.Length; i++)
            {
                string hexColor = GConst.ColorDict.TryGetValue(i, out hexColor) ? hexColor : "EEEEEE";
                result += "<color=#" + hexColor + ">" + input[i] + "</color>";
            }
            return result;
        }

        public static string ScoreMilestoneToColoredText(ScoreMilestone milestone)
        {
            return milestone == ScoreMilestone.ULTIMATE
                ? RainbowString(milestone.ToString() + "!")
                : GConst.ScoreMilestoneColor.TryGetValue(milestone, out string hexColor)
                ? "<color=#" + hexColor + ">" + milestone.ToString() + "!</color>"
                : milestone.ToString();
        }

        public static string RankToColoredText(int rank, string input)
        {
            return GConst.ColorDict.TryGetValue(rank, out string hexColor) ? "<color=#" + hexColor + ">" + input + "</color>" : input;
        }
    }

    public static class ListUtilities
    {
        public static int RepeatIndex<T>(int index, ICollection<T> collection)
        {
            return index >= collection.Count ? 0
                : index < 0 ? collection.Count - 1
            : index;
        }
    }

    // Music CC:
    // Alex McCulloch chill-guitar >< chill-chiptune

    //TO-DO: Day3
    // v Add VFX
    // v Add SFX
    // v Add Camera Shake

    //TO-DO: Day4
    // v Add Music
    // v Add Outline effect with post processing
    // v Score Manager

    //TO-DO: Day5
    // v Multiplier apply to score
    // v Camera scaler according to resolution
    // v Add Music
    // v Add Outline effect with post processing
    // v Settings up builds

    //TO-DO: Day6
    // Scriptable Object for
    //   v LevelSet
    //   GameSettings
    // Local Leaderboard
    // Add Menus: 
    //   v Play Button
    //   v List Buttons
    //   p Restart Button
    //   p Leaderboard Button

    //TO-DO: Day7 (& a few more wishlist)
    // v Local Leaderboard
    // Restart the game
    // Scale/Anchor of World Space button in different resolution
    // w Extra: Make mergables poolable
    // UI Element/Image HDR Material
    // w Change set

    // TO-DO: Day8
    // v Local Leaderboard flow
    // v Restart the game

    // TO-DO: Day11
    // Change BG shop based on selected set
    // Add new Christmas set

    // TO-DO: Day18
    // New game mode: Endless mode

    // Side works:
    // v Cloud Displacement shader
    // v Vibration on mobile (only slightly)

    // Add new set check list:
    // - Collider and Renderer on main object
    // - Central pivot
    // - Location 0,0,1 -> to be rework
    // - Layer = Mergables
    // - Add rigid body
    // - Add Mergable script

    // Day 21
    // Optimization

    // Day 22
    // Polish UI
}