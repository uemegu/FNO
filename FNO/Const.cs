using System;
namespace FNO
{
    public class Const
    {
        // Firebase
        public const string StoreUser = "";
        public const string StorePassword = "";
        public const string AdMobID = "";
        public const string AdMobAdID = "";

        // Config (setting may be chnaged from Remote Config
        public static int AttackPower_NORMAL = 10;
        public static int AttackPower_RARE = 15;
        public static int AttackPower_ACHIEVEMENT = 13;
        public static double StrongAttack_NearCenter = 3;
        public static double StrongAttack_NotNearCenter = 2;
        public static double AttackBounus_SameDistance = 2;
        public static double AttackBounus_NotSameDistance = 1.5;
        public static int AttackBounus_Critical = 2;
        public static int BattleAndPray_MaxCount = 5;
        public static int GetRareName_Rate = 3;
        public static int HistoryMaxCount = 30;

        // DEBUG
#if DEBUG
        public static bool Offline = true;
#else
        public static bool Offline = false;
#endif
    }
}
