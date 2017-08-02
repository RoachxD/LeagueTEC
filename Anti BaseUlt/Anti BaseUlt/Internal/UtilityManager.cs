using System;
using Aimtec;

namespace Anti_BaseUlt.Internal
{
    internal static class UtilityManager
    {
        public static Obj_AI_Hero Player = ObjectManager.GetLocalPlayer();

        public static Vector3 GetFountainPos()
        {
            if (Game.MapId == GameMapId.SummonersRift)
            {
                return Player.Team == GameObjectTeam.Order
                    ? new Vector3(396, 185.1325f, 462)
                    : new Vector3(14340, 171.9777f, 14390);
            }

            return Vector3.Zero;
        }

        public static int GetRecallDuration(this Obj_AI_BaseTeleportEventArgs args)
        {
            if (args.Name.Equals("recall"))
            {
                return 8000;
            }

            if (args.Name.Equals("RecallImproved"))
            {
                return 7000;
            }

            if (args.Name.Equals("OdinRecall"))
            {
                return 4500;
            }

            if (args.Name.Equals("OdinRecallImproved") || args.Name.Equals("SuperRecall") ||
                args.Name.Equals("SuperRecallImproved"))
            {
                return 4000;
            }

            return 8000;
        }

        public static bool IsLineCircleIntersection(this Vector3 circle, float radius, Vector3 v1, Vector3 v2)
        {
            var toLineEnd = v2 - v1;
            var toCircle = circle - v1;
            var theta = (toCircle.X * toLineEnd.X + toCircle.Y * toLineEnd.Y) /
                        (toLineEnd.X * toLineEnd.X + toLineEnd.Y * toLineEnd.Y);
            theta = theta <= 0 ? 0 : 1;

            var closest = v1 + new Vector3(toLineEnd.X * theta, toLineEnd.Y * theta, toLineEnd.Z * theta);
            var d = circle - closest;
            var dist = d.X * d.X + d.Y * d.Y;
            return dist <= radius * radius;
        }

        public static bool IsRecalling(this Obj_AI_BaseTeleportEventArgs args)
        {
            if (args.Sender != Player)
            {
                return false;
            }

            return args.Name.Equals("recall") || args.Name.Equals("RecallImproved") || args.Name.Equals("OdinRecall") ||
                   args.Name.Equals("OdinRecallImproved") || args.Name.Equals("SuperRecall") ||
                   args.Name.Equals("SuperRecallImproved");
        }

        public static void Print(string format)
        {
            Console.WriteLine("Anti BaseUlt: " + format);
        }
    }
}