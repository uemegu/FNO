
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glorious;
using Xamarin.Forms;
using FNO.Controls;
using FNO.Models;
using FNO.Pages.SubPages;
using FNO.ViewModels;

namespace FNO.Pages
{
    public partial class MainPage : BasePage
    {
        private static MainPage _main;
        private bool _clearFlg = false;
        private int _defaultNumOfContent;
        private bool _nowShowingSubPage = false;
        private UserProfile _dataChangedFromRemote;
        private int _initialContentNum;

        public MainPage(MainViewModel vm)
        {
            _main = this;
            InitializeComponent();
            BindingContext = vm;
            _defaultNumOfContent = _main.Container.Children.Count;
            vm.User.GameClear += User_GameClear;
            PredictArea.BindingContext = new PredictViewModel();
            Back.Source = ImageSource.FromResource("FNO.Resouces.Parts.Back.png");
            Animation();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (vm.User.ContinuousLogin != 0 && vm.User.ContinuousLogin == 3)
                {
                    vm.User.ClearContinusLoginRecord();
                    AddSubPage(new PrayPage(TRANSITON_FROM.RIGHT, "17"), vm.Save);
                }
                else if (vm.User.RareLogin)
                {
                    vm.User.ClearContinusLoginRecord();
                    AddSubPage(new PrayPage(TRANSITON_FROM.RIGHT, "16"), vm.Save);
                }
                return false;
            });

            App.CloudDataChanged += async (sender, e) =>
            {
                _dataChangedFromRemote = e.UserProfile;
                if (!_nowShowingSubPage)
                {
                    await CheckData();
                }
            };

            _initialContentNum = Container.Children.Count;
        }

        void User_GameClear(object sender, EventArgs e)
        {
            _clearFlg = true;
        }

        private void Animation()
        {
            MyImage.Opacity = 0;
            Device.BeginInvokeOnMainThread(() =>
            {
                MyImage.FadeTo(1, 1000);
            });
        }

        void Handle_Pray(object sender, System.EventArgs e)
        {
            Sound.SelectPrayMenu();
            AddSubPage(new PrayPage(TRANSITON_FROM.LEFT));
        }

        void Handle_Battle(object sender, System.EventArgs e)
        {
            Sound.SelectBattleMenu();
            AddSubPage(new BattlePage(TRANSITON_FROM.RIGHT));
        }

        void Handle_Status(object sender, System.EventArgs e)
        {
            Sound.SelectStatusMenu();
            AddSubPage(new StatusPage(TRANSITON_FROM.BOTTOM));
        }

        void Handle_Help(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("http://megyo.jp/software/fno/index.html"));
        }

        public static void AddSubPage(BaseSubPage page, Action disposed = null)
        {
            _main._nowShowingSubPage = true;
            var SubPageArea = new AbsoluteLayout();
            Grid.SetRowSpan(SubPageArea, 2);
            page.BindingContext = _main.BindingContext;
            page.Disappeared = async () =>
            {
                _main.Container.Children.Remove(SubPageArea);
                disposed?.Invoke();
                if (_main._defaultNumOfContent == _main.Container.Children.Count && _main._clearFlg)
                {
                    _main._clearFlg = false;
                    AddSubPage(new ClearPage(TRANSITON_FROM.RIGHT));
                }
                if (_main.Container.Children.Count == _main._initialContentNum)
                {
                    await _main.CheckData();
                    _main._dataChangedFromRemote = null;
                    _main._nowShowingSubPage = false;
                }
            };
            SubPageArea.Children.Add(page);
            _main.Container.Children.Add(SubPageArea);
        }

        private async Task CheckData()
        {
            if (_dataChangedFromRemote != null)
            {
                await ((MainViewModel)BindingContext).Reload(_dataChangedFromRemote);
                Device.BeginInvokeOnMainThread(() =>
                {
                    //await App.ShowMessage("誰かから戦いを挑まれました。ステータスを確認して下さい。");
                    DependencyService.Get<IDeviceService>().ShowToast("誰かと戦ったようだ・・・");
                    DependencyService.Get<IDeviceService>().LogEvent("Attacked", "Online", "Online");
                });
            }
        }
    }
}
