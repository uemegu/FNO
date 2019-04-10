using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using FNO.Models;
using FNO.Utils;

namespace FNO.Controls
{
    public class Battle
    {
        public event BattleFinishedEventHandler BattleFinshed;
        public event DamageEventHandler DamagedEvent;
        private UserProfile _current;
        private UserProfile _enemy;
        private IList<AttackUnit> _sequence;
        int _count = 0;

        public Battle(UserProfile current, UserProfile enemy)
        {
            _current = current;
            _enemy = enemy;

            _current.CurrentHP = _current.HP;
            _enemy.CurrentHP = _enemy.HP;

            var tmp = new List<AttackUnit>();
            tmp.Add(new AttackUnit() { Name = current.MainName, Target = enemy, AttackRatio = CalcAttackBounus(current.MainName, current.SubName) });
            tmp.Add(new AttackUnit() { Name = current.SubName, Target = enemy, AttackRatio = CalcAttackBounus(current.MainName, current.SubName) });
            tmp.Add(new AttackUnit() { Name = enemy.MainName, Target = current, AttackRatio = CalcAttackBounus(enemy.MainName, enemy.SubName) });
            tmp.Add(new AttackUnit() { Name = enemy.SubName, Target = current, AttackRatio = CalcAttackBounus(enemy.MainName, enemy.SubName) });
            _sequence = tmp.OrderBy((arg) => arg.Name.DEX).ToList();
            for (var i = 0; i < _sequence.Count; i += 1)
            {
                var unit = _sequence[i];
                int count = i + 1;
                while (true)
                {
                    if (count >= _sequence.Count)
                    {
                        count = 0;
                    }
                    var target = _sequence[count];
                    if (target.Target != unit.Target && !_sequence.Any((arg) => arg.AttackTargetUnit == target))
                    {
                        unit.AttackTargetUnit = target;
                        break;
                    }
                    count++;
                }
            }
        }

        public void Start()
        {
            StartCore();
        }

        private void StartCore()
        {
            Task.Run(async () =>
            {
                if (Attack())
                {
                    return;
                }
                await Task.Delay(250);
                StartCore();
            });
        }

        private bool Attack()
        {
            var attackUnit = _sequence[_count];
            var target = attackUnit.Target;
            _count++;
            if (_count > 3) _count = 0;

            if (target.DEX > MyRandom.GetRandom(100))
            {
                Device.BeginInvokeOnMainThread(() =>
                    DamagedEvent?.Invoke(this, new DamageEventArgs() { Target = target, Damage = 0 })
                );
                return false;
            }

            var tmp = AttackPower(attackUnit.Name) * attackUnit.AttackRatio * CalcStrongAttak(attackUnit.Name, attackUnit.AttackTargetUnit.Name);
            var creticalRatio = CalcCritical(target == _current ? _enemy : _current);
            var damage = (int)(tmp * creticalRatio);

            target.CurrentHP -= damage;
            if (target.CurrentHP < 0) target.CurrentHP = 0;
            Device.BeginInvokeOnMainThread(() =>
                DamagedEvent?.Invoke(this, new DamageEventArgs() { Target = target, Damage = damage, IsCritical = creticalRatio != 1 })
            );
            if (target.CurrentHP <= 0)
            {
                var winner = target == _current ? _enemy : _current;
                Device.BeginInvokeOnMainThread(() =>
                    BattleFinshed?.Invoke(this, new BattleFinishedEventArgs() { Winner = winner })
                );
                return true;
            }


            return false;
        }

        private double AttackPower(Name name)
        {
            if (name.AttributeType == ATTRIBUTE_TYPE.NORMAL) return Const.AttackPower_NORMAL;
            if (name.AttributeType == ATTRIBUTE_TYPE.RARE) return Const.AttackPower_RARE;
            if (name.AttributeType == ATTRIBUTE_TYPE.ACHIEVEMENT) return Const.AttackPower_ACHIEVEMENT;
            throw new Exception("Unknown type:" + name.AttributeType);
        }

        private double CalcStrongAttak(Name current, Name enemy)
        {
            if (current.StrongAttackTarget.Contains(enemy.Attribute))
            {
                if (current.DistanceFromCenter < enemy.DistanceFromCenter)
                {
                    return Const.StrongAttack_NearCenter;
                }
                else
                {
                    return Const.StrongAttack_NotNearCenter;
                }
            }
            return 1;
        }

        private double CalcAttackBounus(Name main, Name sub)
        {
            if (main.NeighborAttribute.Contains(sub.Attribute))
            {
                if (main.DistanceFromCenter == sub.DistanceFromCenter)
                {
                    return Const.AttackBounus_SameDistance;
                }
                else
                {
                    return Const.AttackBounus_NotSameDistance;
                }
            }
            return 1;
        }

        private int CalcCritical(UserProfile attaker)
        {
            return attaker.DEX / 2 > MyRandom.GetRandom(100) ? Const.AttackBounus_Critical : 1;
        }

        public delegate void DamageEventHandler(object sender, DamageEventArgs args);
        public class DamageEventArgs : EventArgs
        {
            public UserProfile Target { get; set; }
            public int Damage { get; set; }
            public bool IsCritical { get; set; } = false;
        }

        public delegate void BattleFinishedEventHandler(object sender, BattleFinishedEventArgs args);
        public class BattleFinishedEventArgs : EventArgs
        {
            public UserProfile Winner { get; set; }
        }

        public class AttackUnit
        {
            public Name Name { get; set; }
            public UserProfile Target { get; set; }
            public double AttackRatio { get; set; }
            public AttackUnit AttackTargetUnit { get; set; }
        }
    }
}
