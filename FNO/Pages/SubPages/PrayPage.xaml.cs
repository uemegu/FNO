using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using FNO.Controls;
using FNO.Models;
using FNO.Utils;
using FNO.ViewModels;

namespace FNO.Pages.SubPages
{
    public partial class PrayPage : BaseSubPage
    {
        private string _achivementCode;

        private UserProfile _current;

        public PrayPage(TRANSITON_FROM from) : base(from)
        {
            InitializeComponent();
        }

        public PrayPage(TRANSITON_FROM from, string achivementCode) : base(from)
        {
            InitializeComponent();
            _achivementCode = achivementCode;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _current = _vm?.User;

            if (!string.IsNullOrEmpty(_achivementCode))
            {
                if (_achivementCode == "14")
                {
                    AchiveText.Text = "連続敗退実績を達成";
                }
                else if (_achivementCode == "15")
                {
                    AchiveText.Text = "連続勝利実績を達成";
                }
                else if (_achivementCode == "16")
                {
                    AchiveText.Text = "久々ログイン実績を達成";
                }
                else if (_achivementCode == "17")
                {
                    AchiveText.Text = "連続ログイン実績を達成";
                }

                First.IsVisible = false;
                ShowSecond();
                return;
            }

            if (_current != null)
            {
                var rest = _current.TodaysPrayCount();
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
        }

        void Handle_Next(object sender, System.EventArgs e)
        {
            if (_current.TodaysPrayCount() >= Const.BattleAndPray_MaxCount)
            {
                // TODO show movie
                DependencyService.Get<IDeviceService>().ShowAd((res) =>
                {
                    _vm.User.ClearPrayCount();
                    DependencyService.Get<IDeviceService>().LogEvent("ViewAd", "Pray", "Pray");
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
            if (!string.IsNullOrEmpty(_achivementCode))
            {
                StartPrayText.Text = "実績解禁!!";
            }
            StartPray.IsVisible = true;
            Sound.Pray();
            Sound.PrayVoice();
            MagicCircle.Show(this, 3000, () =>
            {
                StartPray.IsVisible = false;
                Flash.Show(this, 1000, () =>
                {
                    Second.IsVisible = true;
                    var masters = CodeMasterServce.GetMasters<JobMaster>();
                    var target1 = string.IsNullOrEmpty(_achivementCode) ? CodeMasterServce.GetNameRandom() : CodeMasterServce.GetNameRandomByAttribute(_achivementCode);
                    var target2 = masters[MyRandom.GetRandom(masters.Count)];
                    var gotName = new Name(target1, target2);

                    var name = new Chu2Name();
                    name.BindingContext = gotName;
                    name.ShowAttribute = true;
                    name.ShowAttributeType = true;
                    name.ShowParameter = true;
                    GetResult.Children.Add(name);

                    var image = new Charactor();
                    image.BindingContext = gotName;
                    image.HorizontalOptions = LayoutOptions.End;
                    Second.Children.Insert(0, image);

                    _current.CountUpPray();

                    var hasName = _current.Names.FirstOrDefault(n => n.Attribute == gotName.Attribute);

                    if (hasName != null)
                    {
                        BackButton.IsVisible = false;
                        Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                        {
                            SelectItem.Show(this, "同じ属性の二つ名を既に持っています。二つ名を入れ替えますか？",
                                new List<SelectItem.Item>()
                                {
                                    new SelectItem.Item()
                                    {
                                        Label = "二つ名を入れ替える",
                                        Selected = () => {
                                            _current.Names.Remove(hasName);
                                            AddNewName(gotName);
                                            Device.StartTimer(TimeSpan.FromSeconds(2), () => { Back(); return false; });
                                        }
                                    },
                                    new SelectItem.Item()
                                    {
                                        Label = "キャンセル",
                                        Selected = () => {
                                            CutIn.Show(this, "二つ名の取得をキャンセルしました", 3, () => { Back(); });
                                        }
                                    }
                                });
                            return false;
                        });
                    }
                    else
                    {
                        AddNewName(gotName);
                    }
                });
            });
        }

        private void AddNewName(Name gotName)
        {
            _current.AddNewName(gotName);
        }

        void Handle_Back(object sender, System.EventArgs e)
        {
            Back(true);
        }
    }
}
