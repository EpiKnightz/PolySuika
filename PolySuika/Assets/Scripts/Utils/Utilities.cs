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

        public const int CLEAR_FINISHED_VALUE = -1;

        public static Dictionary<int, string> ColorDict = new()
        {
                {0, "001FFF" },
                {1, "44A8A8" },
                {2, "55D25A" },
                {3, "AEB000" },
                {4, "CD6A29" },
                {5, "FF4040" },
                {6, "B32D9A" },
                {7, "5229A5" },
                {8, "AAAAAA" }
            };
        public static Dictionary<ScoreMilestone, string> ScoreMilestoneColor = new()
        {
                { ScoreMilestone.NICE, "001FFF" },
                { ScoreMilestone.GOOD, "44A8A8" },
                { ScoreMilestone.GREAT, "55D25A" },
                { ScoreMilestone.SUPER, "AEB000" },
                { ScoreMilestone.UNREAL, "CD6A29" },
                { ScoreMilestone.INSANE, "FF4040" },
                { ScoreMilestone.MAGICAL, "B32D9A" },
                { ScoreMilestone.EXTREME, "5229A5" },
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
}