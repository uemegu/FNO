using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PropertyChanged;
using Xamarin.Forms;
using FNO.Models;
using FNO.Utils;

/***
 * まずは聞いて欲しい。
 * 最初はちゃんと画面毎にViewModelを作ろうと思ったんだ。嘘じゃない。
 * だけど、画面の数あまりないし、ViewModel作るほどの事のコード量でもないように思えたんだ。
 * コピペっぽいロジックが何箇所か生まれてしまったことは否定できないが、画面操作が多いので
 * ViewModelにする方が面倒じゃないかと思ったんだ。
 * そう、つまりはめんどくさかったんだ。
 * だから悪の巣窟のようなクラスがいるんだ。
 * でもこれだけは言わせてもらおう。
 * MainVideModelはMainPageのために作ったのでMainViewModelなんだ。
 * 決して中央集権的な権限を有したクラスを目指したわけじゃないんだ。
 * まあでも大した仕事してないのは実装を見て貰えばわかるだろう。
 * つまりはそういうことなんだ。
 */
namespace FNO.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        public UserProfile User { get; private set; }

        public async Task<UserProfile> GetOtherRandomAsync()
        {
            if (Const.Offline)
                return OtherProfiles[MyRandom.GetRandom(OtherProfiles.Count)];

            var other = await DependencyService.Get<IDeviceService>().LoadUserRandomAsync();
            if (other.Id == User.Id || !other.CanBattle)
            {
                other = await GetOtherRandomAsync();
            }
            return other;
        }

        public void CreateUser()
        {
            User = new UserProfile();
        }

        public void Save()
        {
            if (Const.Offline)
                return;

            if (string.IsNullOrEmpty(User.Id))
            {
                User.Id = DependencyService.Get<IDeviceService>().SaveNewUser(User);
                new ID() { Id = User.Id }.Save();
            }
            else
            {
                DependencyService.Get<IDeviceService>().SaveData(User);
            }
        }

        public void SaveOther(UserProfile other)
        {
            if (Const.Offline)
                return;

            DependencyService.Get<IDeviceService>().SaveOtherData(other);
        }

        public async Task LoadAsync()
        {
            if (Const.Offline)
                return;

            var id = ID.Load();
            if (id != null)
            {
                User = await DependencyService.Get<IDeviceService>().LoadUserAsync(id.Id);
                if (User != null)
                {
                    User.Id = id.Id;
                }
            }
        }

        public async Task Reload(UserProfile profile)
        {
            var needNotification = false;
            foreach (var name in profile.RobbedNames)
            {
                needNotification |= User.RobName(name);
            }
            foreach (var history in profile.AddedHistory)
            {
                if (!User.History.Contains(history))
                {
                    User.History.Insert(0, history);
                    if (User.History.Count == UserProfile.MAX_HISTOTY)
                        User.History.RemoveAt(UserProfile.MAX_HISTOTY - 1);
                }
            }
            if (needNotification)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.ShowMessage("二つ名が奪われたため設定が変更されました");
                });
            }
            User.RobbedNames.Clear();
            User.AddedHistory.Clear();
            Save();
            var id = ID.Load();
            User = await DependencyService.Get<IDeviceService>().LoadUserAsync(id.Id);
            User.Id = id.Id;

            //念の為
            User.RobbedNames.Clear();
            User.AddedHistory.Clear();
        }

        //以下、オフラインプレイデバッグ時に使うもの
        private static IList<UserProfile> _others;
        public static IList<UserProfile> OtherProfiles
        {
            get
            {
                if (_others == null)
                    _others = new List<UserProfile>();

                SetOtherUserProfile(_others);

                return _others;
            }
        }
        private static void SetOtherUserProfile(IList<UserProfile> others)
        {
            for (var j = 0; j < 10; j++)
            {
                var p = new UserProfile();
                for (var i = 0; i < 5; i++)
                {
                    p.Names.Add(new Name()
                    {
                        First = (MyRandom.GetRandom(CodeMasterServce.GetMasters<NameMaster>().Count() - 1) + 1).ToString(),
                        Second = (MyRandom.GetRandom(CodeMasterServce.GetMasters<JobMaster>().Count() - 1) + 1).ToString()
                    });
                }
                p.MainName = p.Names[0];
                p.SubName = p.Names[1];
                p.WinComment = "ククク、たわいもない";
                p.LoseComment = "今日のところは勘弁してやろう";
                p.BattleComment.Add("まだまだ");
                p.BattleComment.Add("ここからが本番だぞ");
                p.BattleComment.Add("なかなかやるようだな");
                p.PlayerName = "上田" + j;
                others.Add(p);
            }
        }
    }
}
