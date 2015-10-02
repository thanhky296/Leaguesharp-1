﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace MagicalGirlLux.Helpers
{
    internal class Drawings : Program
    {
        public static void DrawEvent()
        {
            Drawing.OnDraw += DamageIndicator;
            Drawing.OnDraw += Indicator;
            Drawing.OnEndScene += MiniMapDraw;
            Drawing.OnDraw += Eobject;
        }

        private static void Eobject(EventArgs args)
        {
            if (LuxEGameObject != null && Config.Item("LuxE.Indicator").GetValue<bool>())
            {
                if (LuxEGameObject == null) return;
                var pos1 = Drawing.WorldToScreen(player.Position);
                var pos2 = Drawing.WorldToScreen(LuxEGameObject.Position);


                Drawing.DrawLine(pos1, pos2, 1, System.Drawing.Color.LightBlue);
                Drawing.DrawCircle(LuxEGameObject.Position, 100, System.Drawing.Color.Gray);
            }
        }

        static void MiniMapDraw (EventArgs args)
        {
            bool drawMinimapR = Config.Item("drawMinimapR").GetValue<bool>();
            if (ObjectManager.Player.Level >= 6 && drawMinimapR)
                Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.DeepSkyBlue, 2, 30, true);
        }

        public static void Indicator(EventArgs args)
        {

            var enemies1 = HeroManager.Enemies.Where(e => !e.IsDead).ToList();
            var enemies2 = HeroManager.Enemies.Where(e => !e.IsDead && player.Distance(e.Position) < 3000).ToList();

            foreach (var enemy in enemies1.Where(enemy => enemy.Team != player.Team))
            {
                if (enemy.IsVisible && !enemy.IsDead)
                {
                    if (Config.Item("indicator").GetValue<bool>())
                    {
                        var pos = player.Position +
                                  Vector3.Normalize(enemy.Position - player.Position)*200;
                        var myPos = Drawing.WorldToScreen(pos);
                        pos = player.Position + Vector3.Normalize(enemy.Position - player.Position)*450;
                        var ePos = Drawing.WorldToScreen(pos);

                        var linecolor = Color.LawnGreen;
                        var linecolor2 = Color.LawnGreen;
                        if (enemy.Position.Distance(player.Position) > 3000)
                        {
                            linecolor = Color.LawnGreen;
                        }
                        else if (enemy.Position.Distance(player.Position) < 3000) 
                            linecolor = Color.Red;
                        if (enemies2.Count > 1)
                            linecolor2 = Color.Red;

                        if (Config.Item("indicator").GetValue<bool>())
                            Drawing.DrawLine(myPos.X, myPos.Y, ePos.X, ePos.Y, 2, linecolor);
                            Render.Circle.DrawCircle(player.Position, 200, linecolor2);
                        }
                    }
                }
            }
       

        public static void DamageIndicator(EventArgs args)
        {
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            var mode = Config.Item("dmgdrawer").GetValue<StringList>().SelectedIndex;
            if (mode == 0)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != player.Team))
                {
                    if (enemy.IsVisible && !enemy.IsDead)                                     
                        {

                            var combodamage = (Program.CalcDamage(enemy));

                            var PercentHPleftAfterCombo = (enemy.Health - combodamage)/enemy.MaxHealth;
                            var PercentHPleft = enemy.Health/enemy.MaxHealth;
                            if (PercentHPleftAfterCombo < 0)
                                PercentHPleftAfterCombo = 0;

                            var hpBarPos = enemy.HPBarPosition;
                            hpBarPos.X += 45;
                            hpBarPos.Y += 18;
                            double comboXPos = hpBarPos.X - 36 + (107*PercentHPleftAfterCombo);
                            double currentHpxPos = hpBarPos.X - 36 + (107*PercentHPleft);
                            var diff = currentHpxPos - comboXPos;
                            for (var i = 0; i < diff; i++)
                            {
                                Drawing.DrawLine(
                                    (float) comboXPos + i, hpBarPos.Y + 2, (float) comboXPos + i,
                                    hpBarPos.Y + 10, 1, Config.Item("dmgcolor").GetValue<Circle>().Color);
                                Utility.HpBarDamageIndicator.Enabled = false;
                            }

                        }
                    }
                }
            if (mode == 1)
            {
                    Utility.HpBarDamageIndicator.DamageToUnit = Program.CalcDamage;
                    Utility.HpBarDamageIndicator.Color = Config.Item("dmgcolor").GetValue<Circle>().Color;
                    Utility.HpBarDamageIndicator.Enabled = true;                
            }
            }        
        }
    }



