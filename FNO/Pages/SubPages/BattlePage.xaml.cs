using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using FNO.Controls;
using FNO.Models;
using FNO.Utils;
using FNO.ViewModels;
using static FNO.Controls.Battle;

namespace FNO.Pages.SubPages
{
    public partial class BattlePage : BaseSubPage
    {
        public UserProfile Current { get; private set; }
        public UserProfile Enemy { get; private set; }
        private Battle _battle;

        public BattlePage(TRANSITON_FROM from) : base(from)
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (_vm != null)
            {
                Current = _vm.User;
                if (Current != null)
                {
                    var rest = Current.TodaysBattleCount();
                    if (rest >= Const.BattleAndPray_MaxCount)
                    {
                        NoRestCount.IsVisible = true;
                    }
                    else
                    {
                        HasRestCount.IsVisible = true;
                        RestCountString.Text = "残り" + (Const.BattleAndPray_MaxCount - rest) + "回";
                    }
                }
                BindingContext = this;
            }
        }

        void Handle_Next(object sender, System.EventArgs e)
        {
            if (Current.TodaysBattleCount() >= Const.BattleAndPray_MaxCount)
            {
                DependencyService.Get<IDeviceService>().ShowAd((res) =>
                {
                    _vm.User.ClearBattleCount();
                    DependencyService.Get<IDeviceService>().LogEvent("ViewAd", "Battle", "Battle");
                    ShowSecond();
                });
            }
            else
            {
                ShowSecond();
            }
        }

        private void ShowSecond()
        {
            First.IsVisible = false;
            MagicCircle.Show(this, 1000, async () =>
            {
                Flash.Show(this);
                Current.CountUpBattle();
                Enemy = await _vm.GetOtherRandomAsync();
                Second.IsVisible = true;
                SecondButton.Refresh();
            });
        }


        void Handle_OK(object sender, System.EventArgs e)
        {
            ShowThird();
        }

        private void ShowThird()
        {
            Sound.Battle();
            _battle = new Battle(Current, Enemy);
            _battle.BattleFinshed += _battle_BattleFinshed;
            _battle.DamagedEvent += _battle_DamagedEvent;

            Second.IsVisible = false;
            Third.IsVisible = true;

            CurrentAttribute1.BindingContext = Current.MainName;
            CurrentAttribute2.BindingContext = Current.SubName;
            EnemyAttribute1.BindingContext = Enemy.MainName;
            EnemyAttribute2.BindingContext = Enemy.SubName;

            _battle.Start();
        }

        void _battle_DamagedEvent(object sender, DamageEventArgs args)
        {
            if (args.Target == Current)
            {
                if (args.Damage == 0)
                {
                    Damege.Show(CurrentDamage, "Miss");
                    return;
                }
                Damege.Show(CurrentDamage, args.Damage.ToString());
                CurrentImage.Damaged();
                if (args.IsCritical)
                {
                    Flash.Show(this);
                    MagicCircle.Show(this, 500);
                    CutInFace.Show(this, new NameModelViewModel().Hissatsu, Enemy.MainName, false, 2);
                    DependencyService.Get<IDeviceService>().PlayVibrate();
                }
                else if (MyRandom.GetRandom(10) > 3)
                {
                    BattleComment.Show(BattleCommentAreaForCurrent, Current.GetBattleComment());
                }
            }
            else
            {
                if (args.Damage == 0)
                {
                    Damege.Show(EnemyDamage, "Miss");
                    return;
                }
                Damege.Show(EnemyDamage, args.Damage.ToString());
                EnemyImage.Damaged();
                if (args.IsCritical)
                {
                    Flash.Show(this);
                    MagicCircle.Show(this, 500);
                    CutInFace.Show(this, new NameModelViewModel().Hissatsu, Current.MainName, true, 2);
                    DependencyService.Get<IDeviceService>().PlayVibrate();
                }
                else if (MyRandom.GetRandom(10) > 3)
                {
                    BattleComment.Show(BattleCommentAreaForEnemy, Enemy.GetBattleComment());
                }
            }
        }

        void _battle_BattleFinshed(object sender, BattleFinishedEventArgs e)
        {
            Flash.Show(this);
            LogoBack.IsVisible = true;
            if (e.Winner == Current)
            {
                Sound.WIN();
                Enemy.AddBattleHistory(Current, UserProfile.HISTORY_TYPE.LOSE_OFFLINE);
                Current.AddBattleHistory(Enemy, UserProfile.HISTORY_TYPE.WIN);
                var i = 0;
                Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
                {
                    BattleComment.Show(BattleCommentAreaForEnemy, Enemy.LoseComment);
                    BattleComment.Show(BattleCommentAreaForCurrent, Current.WinComment);
                    i++;
                    return i != 10;
                });
                Device.BeginInvokeOnMainThread(() =>
                {
                    AnimationView.Animation = "animation_win.json";
                    AnimationView.Play();
                });
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    ShowFourth();
                    return false;
                });
            }
            else
            {
                Sound.LOSE();
                Enemy.AddBattleHistory(Current, UserProfile.HISTORY_TYPE.WIN_OFFLINE);
                Current.AddBattleHistory(Enemy, UserProfile.HISTORY_TYPE.LOSE);

                var i = 0;
                Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
                {
                    BattleComment.Show(BattleCommentAreaForCurrent, Current.LoseComment);
                    BattleComment.Show(BattleCommentAreaForEnemy, Enemy.WinComment);
                    i++;
                    return i != 10;
                });
                DependencyService.Get<IDeviceService>().PlayVibrate();
                Device.BeginInvokeOnMainThread(() =>
                {
                    AnimationView.Animation = "animation_lose.json";
                    AnimationView.Play();
                });
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    Back();
                    return false;
                });
            }
        }

        private void ShowFourth()
        {
            Third.IsVisible = false;
            Fourth.IsVisible = true;

            NameList.BindingContext = Enemy;
            NameList.NameSelected += (sender, e) =>
            {
                Sound.OK();
                var selectedName = NameList.SelectedName;
                var hasName = Current.Names.FirstOrDefault(name => name.Attribute == selectedName.Attribute);

                if (hasName != null)
                {
                    SelectItem.Show(this, "同じ属性の二つ名を既に持っています。二つ名を入れ替えますか？",
                   new List<SelectItem.Item>()
                   {
                    new SelectItem.Item()
                    {
                        Label = "二つ名を入れ替える",
                        Selected = () => {
                            Current.Names.Remove(hasName);
                            FinishBattleWin(selectedName);
                        }
                    },
                    new SelectItem.Item()
                    {
                        Label = "キャンセル"
                    }
                   });
                }
                else
                {
                    FinishBattleWin(selectedName);
                }
            };
        }

        private void FinishBattleWin(Name selectedName)
        {
            Enemy.RobName(selectedName);
            Current.AddNewName(selectedName);
            CutIn.Show(this, NameList.SelectedName.Chu2Name + NameList.SelectedName.Job + "を手に入れました", 3, () => { Back(); });
        }

        void Handle_Cancel(object sender, System.EventArgs e)
        {
            SelectItem.Show(this, "二つ名の取得をやめますか？",
           new List<SelectItem.Item>()
           {
                new SelectItem.Item()
                {
                    Label = "二つ名を取得しない",
                    Selected = () => {
                        Back();
                    }
                },
                new SelectItem.Item()
                {
                    Label = "キャンセル"
                }
           });
        }

        void Handle_Back(object sender, System.EventArgs e)
        {
            Back(true);
        }

        protected override void Back(bool dontSave = false)
        {
            if (!dontSave)
            {
                _vm.SaveOther(Enemy);
            }
            if (Current.ContinuousWin >= 5)
            {
                MainPage.AddSubPage(new PrayPage(TRANSITON_FROM.RIGHT, "15"), () => { base.Back(false); });
                Current.ClearContinusBattleRecord();
            }
            else if (Current.ContinuousLose >= 5)
            {
                MainPage.AddSubPage(new PrayPage(TRANSITON_FROM.RIGHT, "14"), () => { base.Back(false); });
                Current.ClearContinusBattleRecord();
            }
            else
            {
                base.Back(dontSave);
            }
        }
    }
}
