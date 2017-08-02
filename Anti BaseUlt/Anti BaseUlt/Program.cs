using System.Linq;
using Aimtec;
using Aimtec.SDK.Events;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu.Components;
using Anti_BaseUlt.Internal;

namespace Anti_BaseUlt
{
    internal class Program
    {
        private static int _recallingTick;

        private static void Main(string[] args)
        {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart()
        {
            UtilityManager.Print("Loaded!");

            MissileManager.Initialize();
            MenuManager.Initialize();

            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        private static void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, Obj_AI_BaseTeleportEventArgs e)
        {
            if (!MenuManager.Root["Enable"].As<MenuBool>().Enabled || sender != UtilityManager.Player ||
                !e.IsRecalling())
            {
                return;
            }

            _recallingTick = Game.TickCount + e.GetRecallDuration();
            UtilityManager.Print("Recall detected! (Actual Tick: " + Game.TickCount + " | End Tick: " + _recallingTick +
                                 ")");
        }

        private static void GameObject_OnCreate(GameObject sender)
        {
            if (!MenuManager.Root["Enable"].As<MenuBool>().Enabled || !UtilityManager.Player.IsRecalling() ||
                sender == null || !sender.IsValid || sender.Type != GameObjectType.MissileClient)
            {
                return;
            }

            var spell = (MissileClient) sender;
            if (!spell.SpellCaster.IsValid || spell.SpellCaster.IsAlly)
            {
                return;
            }

            if (!MissileManager.Missiles.Any(
                i =>
                    spell.SpellCaster.UnitSkinName.Contains(i.ChampionName) &&
                    i.MissileName.Equals(spell.SpellData.Name)))
            {
                return;
            }

            var fountainPos = UtilityManager.GetFountainPos();
            if (!fountainPos.IsLineCircleIntersection(500, spell.Position, spell.EndPosition))
            {
                UtilityManager.Print("BaseUlt not in fountain! (" + spell.SpellCaster.UnitSkinName + " - " +
                                     spell.SpellData.Name + " | Pos: " + spell.EndPosition + ")");
                return;
            }

            var tick = Game.TickCount +
                       spell.Position.Distance(fountainPos) /
                       MissileManager.Missiles.Single(i => spell.SpellCaster.UnitSkinName.Contains(i.ChampionName))
                           .Speed * 1000;
            if (1000 + _recallingTick < tick || _recallingTick - 1000 > tick)
            {
                UtilityManager.Print("BaseUlt not correctly timed! (" + spell.SpellCaster.UnitSkinName + " - " +
                                     spell.SpellData.Name + " | Tick: " + tick + ")");
                return;
            }

            UtilityManager.Player.IssueOrder(OrderType.MoveTo, UtilityManager.Player.Position.Extend(Vector3.Down, 1));
            UtilityManager.Print("BaseUlt prevented! (" + spell.SpellCaster.UnitSkinName + " | " + spell.SpellData.Name +
                                 ")");
        }
    }
}