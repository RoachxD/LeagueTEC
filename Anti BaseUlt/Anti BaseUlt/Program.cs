using System.Linq;
using Aimtec;
using Aimtec.SDK.Events;
using Aimtec.SDK.Extensions;
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
            if (Game.MapId != GameMapId.SummonersRift || Game.MapId != GameMapId.TwistedTreeline)
            {
                UtilityManager.Print("Map not supported, assembly not loading.");
                return;
            }

            if (
                !ObjectManager.Get<Obj_AI_Hero>()
                    .Any(
                        c =>
                            c.IsEnemy &&
                            (c.ChampionName.Equals("Ashe") || c.ChampionName.Equals("Draven") ||
                             c.ChampionName.Equals("Ezreal") || c.ChampionName.Equals("Jinx"))))
            {
                UtilityManager.Print("No championss with global ults, assembly not loading.");
                return;
            }

            UtilityManager.Print("Loaded!");

            MissileManager.Initialize();
            MenuManager.Initialize();

            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        private static void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, Obj_AI_BaseTeleportEventArgs e)
        {
            if (!MenuManager.Root["Enable"].Enabled || sender != UtilityManager.Player ||
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
            if (!MenuManager.Root["Enable"].Enabled || !UtilityManager.Player.IsRecalling() ||
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
            var endPos = spell.Position.Extend(spell.EndPosition, spell.Position.Distance(fountainPos));
            if (endPos.Distance(fountainPos) > 500)
            {
                UtilityManager.Print("BaseUlt not in fountain! (" + ((Obj_AI_Hero) spell.SpellCaster).ChampionName +
                                     " - " +
                                     spell.SpellData.Name + " | Pos: " + spell.EndPosition + ")");
                return;
            }

            var tick = Game.TickCount +
                       spell.Position.Distance(fountainPos) /
                       MissileManager.Missiles.Single(
                           i => ((Obj_AI_Hero) spell.SpellCaster).ChampionName.Equals(i.ChampionName)).Speed * 1000;
            if (1000 + _recallingTick < tick || _recallingTick - 1000 > tick)
            {
                UtilityManager.Print("BaseUlt not correctly timed! (" + ((Obj_AI_Hero) spell.SpellCaster).ChampionName +
                                     " - " + spell.SpellData.Name + " | Tick: " + tick + ")");
                return;
            }

            UtilityManager.Player.IssueOrder(OrderType.MoveTo, UtilityManager.Player.Position.Extend(Vector3.Down, 1));
            UtilityManager.Print("BaseUlt prevented! (" + ((Obj_AI_Hero) spell.SpellCaster).ChampionName + " | " +
                                 spell.SpellData.Name + ")");
        }
    }
}