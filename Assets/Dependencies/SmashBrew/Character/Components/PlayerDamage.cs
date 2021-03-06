// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using HouraiTeahouse.SmashBrew.Util;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public class DamageType {
        public static readonly DamageType Percent = new DamageType {
            Change = (currentDamage, delta) => currentDamage + delta,
            Suffix = "%",
            MaxDamage = 999f,
            MinDamage = 0f
        };

        public static readonly DamageType Stamina = new DamageType {
            Change = (currentDamage, delta) => currentDamage + delta,
            Suffix = "HP",
            MaxDamage = 999f,
            MinDamage = 0f
        };

        Func<float, float, float> Change;

        DamageType() { }
        public string Suffix { get; private set; }
        public float MinDamage { get; private set; }
        public float MaxDamage { get; private set; }

        public float Damage(float currentDamage, float delta) {
            return Mathf.Clamp(Change(currentDamage, Mathf.Abs(delta)),
                MinDamage,
                MaxDamage);
        }

        public float Heal(float currentDamage, float delta) {
            return Mathf.Clamp(Change(currentDamage, -Mathf.Abs(delta)),
                MinDamage,
                MaxDamage);
        }
    }

    /// <summary> A MonoBehaviour that handles all of the damage dealt and recieved by a character. </summary>
    public sealed class PlayerDamage : HouraiBehaviour, IResettable {
        /// <summary> The current internal damage value. Used for knockback calculations. </summary>
        public float CurrentDamage { get; set; }

        public float DefaultDamage { get; set; }

        public DamageType Type { get; set; }
        public ModifierGroup<object> DamageModifiers { get; private set; }
        public ModifierGroup<object> HealingModifiers { get; private set; }

        public void OnReset() { CurrentDamage = DefaultDamage; }

        public static implicit operator float(PlayerDamage damage) {
            return damage == null ? 0f : damage.CurrentDamage;
        }

        protected override void Awake() {
            base.Awake();
            DamageModifiers = new ModifierGroup<object>();
            HealingModifiers = new ModifierGroup<object>();
        }

        internal float ModifyDamage(float baseDamage, object source = null) {
            return DamageModifiers.Out.Modifiy(source, baseDamage);
        }

        public void Damage(float damage) { Damage(null, damage); }

        public void Damage(object source, float damage) {
            damage = DamageModifiers.In.Modifiy(source, Mathf.Abs(damage));
            CurrentDamage = Type.Damage(CurrentDamage, damage);
        }

        public void Heal(float healing) { Heal(null, healing); }

        public void Heal(object source, float healing) {
            healing = HealingModifiers.In.Modifiy(source, Mathf.Abs(healing));
            CurrentDamage = Type.Heal(CurrentDamage, healing);
        }
    }
}