
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using FNO.Controls;
using FNO.Models;
using FNO.Utils;
using FNO.ViewModels;

namespace FNO.Pages
{
    public partial class StartingPage : BasePage
    {
        private Steps _steps { get; set; }
        private MainViewModel _vm;

        public StartingPage()
        {
            InitializeComponent();
            _steps = new Controls.Steps()
            {
                NavigationType = Controls.DISPLAY_TYPE.OK
            };
            BindingContext = _steps;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await LoadAsync();
            });

            Sound.Normal();
        }

        private async Task LoadAsync()
        {
            CodeMasterServce.Load();
            _vm = new MainViewModel();
            await _vm.LoadAsync();

            if (_vm.User == null)
            {
                _steps.Add(() =>
                {
                    First.IsVisible = true;
                    Explain.Text = "このゲームをプレイしてくださりありがとうございます" + Environment.NewLine;
                    Explain.Text += "このゲームは二つ名を集めるゲームです" + Environment.NewLine;
                }).Add(() =>
                {
                    Explain.Text = "二つ名には火、闇といった属性があり、全属性の二つ名を集めることでゲームクリアとなります" + Environment.NewLine + Environment.NewLine;
                    Explain.Text += "二つ名は祈りを捧げるか、運命の戦いで勝つことで手に入れることができます";
                }).Add(() =>
                {
                    Explain.Text = "二つ名には通常二つ名、レア二つ名、実績二つ名の３種類があります" + Environment.NewLine + Environment.NewLine;
                    Explain.Text += "実績二つ名は何かを成し遂げることで手に入れることができ、これだけは通常の手段では手に入りません";
                }).Add(() =>
                {
                    Explain.Text = "まずはユーザー名とセリフを入力してください" + Environment.NewLine;
                    Explain.Text += "プレイヤー名は後で変更できませんのでご注意ください" + Environment.NewLine;
                    Explain.Text += "初期値はランダムで設定されています" + Environment.NewLine;
                }).Add(() =>
                {
                    First.IsVisible = false;
                    InputArea.IsVisible = true;
                    NameEntry.Text = CodeMasterServce.DefaultNames[MyRandom.GetRandom(CodeMasterServce.DefaultNames.Count)];
                    WinEntry.Text = CodeMasterServce.DefaultWinComment[MyRandom.GetRandom(CodeMasterServce.DefaultWinComment.Count)];
                    LoseEntry.Text = CodeMasterServce.DefaultLoseComment[MyRandom.GetRandom(CodeMasterServce.DefaultLoseComment.Count)];
                    BattleEntry1.Text = CodeMasterServce.DefaultBattleComment[MyRandom.GetRandom(CodeMasterServce.DefaultBattleComment.Count)];
                    BattleEntry2.Text = CodeMasterServce.DefaultBattleComment[MyRandom.GetRandom(CodeMasterServce.DefaultBattleComment.Count)];
                    BattleEntry3.Text = CodeMasterServce.DefaultBattleComment[MyRandom.GetRandom(CodeMasterServce.DefaultBattleComment.Count)];
                }).Add(() =>
                {
                    First.IsVisible = true;
                    InputArea.IsVisible = false;
                    _vm.CreateUser();
                    _vm.User.PlayerName = NameEntry.Text;
                    _vm.User.WinComment = WinEntry.Text;
                    _vm.User.LoseComment = LoseEntry.Text;
                    _vm.User.BattleComment.Add(BattleEntry1.Text);
                    _vm.User.BattleComment.Add(BattleEntry2.Text);
                    _vm.User.BattleComment.Add(BattleEntry3.Text);
                    Explain.Text = "一定期間ご利用がないとデータは消去されますのでご注意ください" + Environment.NewLine + Environment.NewLine;
                    Explain.Text += "それではゲームスタートです";
                }).Add(() =>
                {
                    First.IsVisible = false;
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        _steps.Next();
                        return false;
                    });
                }).Add(() =>
                {
                    ThirdLabel.Text = "汝、二つ名を欲するものよ" + Environment.NewLine + "汝に二つ名を授けよう";
                    Third.IsVisible = true;
                }).Add(() =>
                {
                    ThirdLabel.Text = "";
                    Third.IsVisible = false;
                    Sound.PrayVoice();
                    ShowNames();
                });
            }
            else if (!_vm.User.CanBattle)
            {
                _steps.Add(() =>
                {
                    Third.IsVisible = true;
                    ThirdLabel.Text = "激しい戦いの末、二つ名を奪われたか・・・" + Environment.NewLine + "二つ名が不足しているようだ" + Environment.NewLine + "汝に新たな二つ名を授けよう";
                }).Add(() =>
                {
                    Explain.Text = "";
                    Third.IsVisible = false;
                    Sound.PrayVoice();
                    ShowNames();
                });
            }

            _steps.Add(() =>
            {
                var comment = new NameModelViewModel();
                ThirdLabel.Text = "さぁ、" + _vm.User.DisplayName + "よ" + Environment.NewLine + "栄光ある未来目指していざ行かん";
                if (MyRandom.GetRandom(10) > 5)
                    ThirdLabel.Text += Environment.NewLine + Environment.NewLine + "ちなみに今日のラッキーカラーは「" + comment.Color + "」だ";
                else
                    ThirdLabel.Text += Environment.NewLine + Environment.NewLine + "ちなみに今日のラッキースポットは「" + comment.Spot + "」だ";
                Third.IsVisible = true;
            }).Add(() =>
            {
                _vm.User.TodaysLogin();
                _vm.Save();
                App.Transition(new MainPage(_vm));
            }).Next();
        }

        private void CheckEntry()
        {
            if (!string.IsNullOrEmpty(NameEntry.Text) && !string.IsNullOrEmpty(WinEntry.Text) && !string.IsNullOrEmpty(LoseEntry.Text)
                && !string.IsNullOrEmpty(BattleEntry1.Text) && !string.IsNullOrEmpty(BattleEntry2.Text) && !string.IsNullOrEmpty(BattleEntry3.Text))
            {
                NextBotton.IsVisible = true;
            }
            else
            {
                NextBotton.IsVisible = false;
            }
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            CheckEntry();
        }

        private void ShowNames()
        {
            Sound.Pray();
            MagicCircle.Show(this, 3000, () =>
            {
                Flash.Show(this, 1000, () =>
                {
                    Second.IsVisible = true;
                    var masters = CodeMasterServce.GetMasters<JobMaster>();
                    var target1 = CodeMasterServce.GetNameRandom();
                    while (_vm.User.Names.Any(item => target1.Attribute == item.Attribute))
                    {
                        target1 = CodeMasterServce.GetNameRandom();
                    }
                    var target2 = masters[MyRandom.GetRandom(masters.Count)];
                    var gotName = new Name(target1, target2);
                    _vm.User.Names.Add(gotName);

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

                    Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                     {
                         if (_vm.User.Names.Count == 2)
                         {
                             Second.IsVisible = false;
                             Third.IsVisible = true;
                             _vm.User.MainName = _vm.User.Names[0];
                             _vm.User.SubName = _vm.User.Names[1];
                             Sound.Normal();
                             _steps.Next();
                         }
                         else
                         {
                             GetResult.Children.Clear();
                             Second.Children.Remove(image);
                             ShowNames();
                         }
                         return false;
                     });

                });
            });
        }

    }
}
