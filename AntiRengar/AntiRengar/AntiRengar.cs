using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AntiRengar
{
    class AntiRengar
    {
        public static AIHeroClient Player = ObjectManager.Player;

        public static Menu Config;
        private static AIHeroClient _rengo;
        private static int lastcasted;
        public static void AddBool(Menu menu, string displayName, string name, bool value = true)
        {
            menu.Add(name, new CheckBox(displayName, value));
        }
        internal static void OnLoad(EventArgs args)
        {
            
            Config = MainMenu.AddMenu("Anti Rengar", "Anti Rengar");
            //Config.AddLabel(" __   __  _______  _______  _______ ");
            //Config.AddLabel("|  | |  ||       ||       ||       |");
            //Config.AddLabel("|  |_|  ||   _   ||    ___||  _____|");
            //Config.AddLabel("|       ||  | |  ||   |___ | |_____ ");
            //Config.AddLabel("|       ||  |_|  ||    ___||_____  |");
            //Config.AddLabel("|   _   ||       ||   |___  _____| |");
            //Config.AddLabel("|__| |__||_______||_______||_______|");

            Config.AddLabel("Made By Hoes TM <3", 50);
            Config.Add("Enable", new CheckBox("Enable", true));
         
            switch(Player.ChampionName)
            {
                case "Vayne":
                    AddBool(Config, "Use E", "usee");
                    AddBool(Config, "Use Q", "useq");
                    break;
                case "Ahri":
                    AddBool(Config, "Use R", "user");
                    break;
                case "Alistar":
                    AddBool(Config, "Use W", "usew");
                    break;
            }

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreateObject;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            GameObject.OnDelete += OnDeleteObject;

        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy")
            {
                Core.DelayAction(() => _rengo = null, 300);
            }
        }
        private static AIHeroClient _target;
        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "randomnamehere")
            {
                if (args.Target.IsAlly || args.Target.IsMe)
                {
                    _target = (AIHeroClient)args.Target;
                }
            }
        }

        public static Vector3 BackWardsCast(float range)
        {
            if (_rengo == null) return new Vector3();

            var rengopos = _rengo.Position;
            return  (Vector3) rengopos.Extend(Player.Position, +_rengo.Position.Distance(Player.Position) + range);
        }

        public static Vector2 BackWardsCast2(float range)
        {
            if (_rengo == null) return new Vector2();

            var rengopos = _rengo.Position;
            return Player.Position.Extend(rengopos, +_rengo.Position.Distance(Player.Position) + range);
        }

        public static void ReadyCast(float range, SpellSlot slot, Vector3 position = new Vector3(), bool targetted = false)
        {
            var spellbook = Player.Spellbook;
            var spell = spellbook.GetSpell(slot);
            if (!spell.IsReady) return;
            if (_rengo == null) return;
            if (_rengo.Position.Distance(Player.Position) > range) return;
            spellbook.CastSpell(slot, true);
            
            if (!targetted)
                Player.Spellbook.CastSpell(slot, position);
            else
                spellbook.CastSpell(slot, _rengo);
        }

        public static bool Ready(SpellSlot slot)
        {
            var spellbook = Player.Spellbook;
            var spell = spellbook.GetSpell(slot);
            return !spell.IsReady;
        }

        public static void SelfCast(float range, SpellSlot slot)
        {
            var spellbook = Player.Spellbook;
            var spell = spellbook.GetSpell(slot);
            if (!spell.IsReady) return;
            if (!_rengo.IsValidTarget(range)) return;

            spellbook.CastSpell(slot);

        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!sender.IsEnemy) return;

            if (sender.Name != "Rengar_LeapSound.troy") return;

            foreach (var enemy in
                EntityManager.Enemies.FindAll(hero => hero.IsValidTarget(1500) && hero.Name == "Rengar"))
            {
                _rengo = (AIHeroClient)enemy;
                lastcasted = Environment.TickCount;
            }
        }


        private static void OnUpdate(EventArgs args)
        {
       
            if (Config["Enable"].Cast<CheckBox>().CurrentValue) return;

            if (_rengo == null) return;
            if ((_rengo.IsDead) || Environment.TickCount - lastcasted > 8 * 10 * 10 * 10)
            {
                _rengo = null;
            }
            if (_rengo == null) return;
            var rengopos = _rengo.Position;
            var spellbook = Player.Spellbook;
            switch (Player.ChampionName.ToLower())
            {
                case "aatrox":
                    {
                        ReadyCast(400, SpellSlot.Q, BackWardsCast(500));
                    }
                    break;
                case "akali":
                    {
                        ReadyCast(1200, SpellSlot.W, Player.Position, false);
                    }
                    break;

                case "braum":
                    {
                        ReadyCast(1200, SpellSlot.R, default(Vector3), true);
                    }
                    break;

                case "cassiopeia":
                    {
                        ReadyCast(1200, SpellSlot.R, default(Vector3), true);
                    }
                    break;

                case "corki":
                    {
                        ReadyCast(500, SpellSlot.W, BackWardsCast(600));
                    }
                    break;

                case "galio":
                    {
                        ReadyCast(1200, SpellSlot.R, default(Vector3), false);
                    }
                    break;

                case "graves":
                    {
                        ReadyCast(500, SpellSlot.E, BackWardsCast(500));
                    }
                    break;

                case "hecarim":
                    {
                        ReadyCast(300, SpellSlot.E, default(Vector3), true);
                    }
                    break;

                case "vayne":
                    if (Config["useq"].Cast<CheckBox>().CurrentValue)
                        ReadyCast(500, SpellSlot.Q, BackWardsCast(500));

                    if (!Ready(SpellSlot.Q) || Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
                    {
                        if (Config["usee"].Cast<CheckBox>().CurrentValue)
                            ReadyCast(500, SpellSlot.E, default(Vector3), true);
                    }

                    if (Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, _rengo);
                    break;

                case "ahri":
                    if (!Player.HasBuff("AhriTumble"))
                    {
                        if (Config["user"].Cast<CheckBox>().CurrentValue)
                            ReadyCast(500, SpellSlot.R, BackWardsCast(430));
                    }
                    if (Player.HasBuff("AhriTumble") || !Player.Spellbook.GetSpell(SpellSlot.R).IsReady)
                        Core.DelayAction(() => ReadyCast(500, SpellSlot.E, default(Vector3)), 200);
                    break;

                case "alistar":
                    SelfCast(1000, SpellSlot.Q);

                    if (!Player.Spellbook.GetSpell(SpellSlot.Q).IsReady)
                    {
                        if (Config["usew"].Cast<CheckBox>().CurrentValue)
                            Core.DelayAction(() => ReadyCast(500, SpellSlot.W, default(Vector3), true), 200);
                    }
                    break;

                case "caitlyn":
                    ReadyCast(500, SpellSlot.E, (Vector3) BackWardsCast2(430));
                    break;

                case "annie":
                    if (Player.HasBuff("pyromaniaparticle"))
                        ReadyCast(500, SpellSlot.Q, default(Vector3), true);
                    break;

                case "ashe":
                    Core.DelayAction(() => ReadyCast(1000, SpellSlot.R, default(Vector3)), 200);
                    break;

                case "azir":
                    ReadyCast(700, SpellSlot.R,(Vector3) Player.Position.Extend(_rengo.Position, Player.BoundingRadius + 100));
                    break;

                case "blitzcrank":
                    SelfCast(500, SpellSlot.R);
                    break;

                case "chogath":
                    ReadyCast(800, SpellSlot.Q, Player.Position);
                    ReadyCast(800, SpellSlot.W, default(Vector3), true);
                    break;

                case "diana":
                    SelfCast(500, SpellSlot.E);
                    break;

                case "draven":
                    Core.DelayAction(() => ReadyCast(600, SpellSlot.E, _rengo.Position), 300);
                    break;

                case "elise":
                    Core.DelayAction(() => ReadyCast(500, SpellSlot.E, default(Vector3)), 200);
                    break;

                case "ezreal":
                    ReadyCast(600, SpellSlot.E, BackWardsCast(500));
                    break;

                case "fiddlesticks":
                    ReadyCast(600, SpellSlot.E, default(Vector3), true);
                    ReadyCast(600, SpellSlot.Q, default(Vector3), true);
                    break;

                case "fizz":
                    ReadyCast(800, SpellSlot.E, BackWardsCast(500));
                    break;

                case "garen":
                    SelfCast(Player.AttackRange, SpellSlot.Q);
                    if (Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed &&
                        _rengo.Position.Distance(Player.Position) < Player.AttackRange + Player.BoundingRadius + _rengo.BoundingRadius)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, _rengo);
                    }
                    break;

                case "gragas":
                    ReadyCast(700, SpellSlot.E, default(Vector3), true);
                    break;

                case "irelia":
                    ReadyCast(400, SpellSlot.E, default(Vector3), true);
                    break;

                case "janna":
                    ReadyCast(800, SpellSlot.Q, default(Vector3), true);
                    break;

                case "jayce":
                    ReadyCast(500, SpellSlot.E, default(Vector3), true);
                    break;

                case "jinx":
                    ReadyCast(500, SpellSlot.E, Player.Position);
                    break;

                case "leblanc":
                    ReadyCast(300, SpellSlot.E, default(Vector3), true);
                    break;

                case "lulu":
                    ReadyCast(500, SpellSlot.W, default(Vector3), true);
                    break;

                case "lux":
                    ReadyCast(600, SpellSlot.Q, default(Vector3), true);
                    break;

                case "nami":
                    Core.DelayAction(() => ReadyCast(400, SpellSlot.Q, rengopos), 300);
                    break;

                case "quinn":
                    ReadyCast(500, SpellSlot.Q, _rengo.Position);
                    ReadyCast(500, SpellSlot.E, default(Vector3), true);
                    break;

                case "riven":
                    SelfCast(1000, SpellSlot.W);
                    break;

                case "shaco":
                    ReadyCast(Player.AttackRange, SpellSlot.Q, BackWardsCast(500));
                    break;

                case "soraka":
                    Core.DelayAction(() => ReadyCast(1000, SpellSlot.E, _rengo.Position), 200);
                    break;

                case "swain":
                    Core.DelayAction(() => ReadyCast(1000, SpellSlot.E, _rengo.Position), 200);
                    break;

                case "thresh":
                    Core.DelayAction(() => ReadyCast(1000, SpellSlot.E, _rengo.Position), 200);
                    break;

                case "tristana":
                    ReadyCast(500, SpellSlot.R, default(Vector3), true);
                    break;

                case "velkoz":
                    Core.DelayAction(() => ReadyCast(1000, SpellSlot.E, Player.Position), 200);
                    break;

                case "viktor":
                    ReadyCast(1000, SpellSlot.W, Player.Position);
                    break;

                case "monkeyking":
                    SelfCast(400, SpellSlot.W);
                    break;

                case "zyra":
                    ReadyCast(800, SpellSlot.E, default(Vector3), true);
                    break;

                case "xerath":
                    ReadyCast(800, SpellSlot.E, _rengo.Position);
                    break;

            }
        }
    }
}
