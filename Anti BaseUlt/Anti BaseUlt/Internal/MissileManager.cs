using System.Collections.Generic;

namespace Anti_BaseUlt.Internal
{
    internal class MissileManager
    {
        public static List<MissileData> Missiles => MissileData.GetMissileList();

        public static void Initialize()
        {
            new MissileData("Ashe", "EnchantedCrystalArrow", 1600).Add();
            new MissileData("Draven", "DravenR", 2000).Add();
            new MissileData("Ezreal", "EzrealTrueshotBarrage", 1600).Add();
            new MissileData("Jinx", "JinxR", 1600).Add();
        }

        public class MissileData
        {
            private static readonly List<MissileData> MissileList = new List<MissileData>();

            public string ChampionName;

            public string MissileName;

            public int Speed;

            public MissileData(string championName, string missileName, int speed)
            {
                ChampionName = championName;
                MissileName = missileName;
                Speed = speed;
            }

            public static List<MissileData> GetMissileList()
            {
                return MissileList;
            }

            public void Add()
            {
                MissileList.Add(this);
            }
        }
    }
}