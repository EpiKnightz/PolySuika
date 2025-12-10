using UnityEngine;

namespace Utilities
{
    public delegate void VoidEvent();
    public delegate void IntEvent(int value);
    public delegate void BoolEvent(bool value);
    public delegate void FloatEvent(float value);
    public delegate void DoubleEvent(double value);
    public delegate void Vector3Event(Vector3 value);

    public static class GConst
    {
        public const int TIER_RANK_1 = 3;
        public const int TIER_RANK_2 = 5;
        public const int TIER_RANK_3 = 7;
    }

    //TO-DO: Day3
    // Add VFX
    // Add SFX
    // Add Camera Shake

    //TO-DO: Day4
    // Add Menus
    // x Add Music
    // x Add Outline effect with post processing
    // + Score Manager

    //TO-DO: Day5
    // Multiplier apply to score
    // Add Music
    // Add Outline effect with post processing
}