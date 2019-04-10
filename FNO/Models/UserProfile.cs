using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using FNO.Utils;
using System.Threading.Tasks;

namespace FNO.Models
{
    public class UserProfile : BaseModel
    {
        public const int MAX_HISTOTY = 30;

        public event EventHandler GameClear;

        [StorageTarget]
        public string PlayerName { get; set; }
        [StorageTarget]
        public IList<Name> Names { get; set; } = new List<Name>();
        [StorageTarget]
        public IList<Name> RobbedNames { get; set; } = new List<Name>();
        [StorageTarget]
        public Name MainName { get; set; }
        [StorageTarget]
        public Name SubName { get; set; }
        [StorageTarget]
        public string WinComment { get; set; }
        [StorageTarget]
        public string LoseComment { get; set; }
        [StorageTarget]
        public IList<string> BattleComment { get; set; } = new List<string>();
        [StorageTarget]
        public int ContinuousWin { get; private set; }
        [StorageTarget]
        public int ContinuousLose { get; private set; }
        [StorageTarget]
        public IList<String> History { get; set; } = new List<string>();
        [StorageTarget]
        public IList<String> AddedHistory { get; set; } = new List<string>();
        [StorageTarget]
        public int ContinuousLogin { get; private set; }
        [StorageTarget]
        public bool RareLogin { get; private set; } = false;
        [StorageTarget]
        private String _lastLogin { get; set; }
        [StorageTarget]
        private IList<String> _prayDate { get; set; } = new List<string>();
        [StorageTarget]
        private IList<String> _battleDate { get; set; } = new List<string>();

        public String Id { get; set; }
        public bool CanBattle => Names.Count >= 2;
        public string DisplayName => MainName.Chu2Name + PlayerName;
        public int AmountOfNames => Names.Count();
        public string HoldNumber => Names.GroupBy(n => n.Attribute).Count() + "/" + CodeMasterServce.GetCount<AttributeMaster>();
        public int CurrentHP { get; set; }
        public int HP => MainName.HP + SubName.HP / 2;
        public int DEX => MainName.DEX + SubName.DEX / 2;
        public ImageSource CharactorImage => MainName.CharactorImage;
        public Color CharactorColor => MainName.CharactorColor;
        public ImageSource CharactorImage2 => SubName.CharactorImage;
        public Color CharactorColor2 => SubName.CharactorColor;
        private bool _isCleared => Names.GroupBy(n => n.Attribute).Count() == CodeMasterServce.GetCount<AttributeMaster>();

        public void TodaysLogin()
        {
            if (!string.IsNullOrEmpty(_lastLogin))
            {
                var d = DateTime.ParseExact(_lastLogin, "yyyyMMdd", null);
                var days = (DateTime.Now - d).Days;
                if (days == 1)
                {
                    ContinuousLogin++;
                }
                else if (days >= 3)
                {
                    RareLogin = true;
                }
                if (days >= 1)
                {
                    ClearPrayCount();
                    ClearBattleCount();
                }
            }
            _lastLogin = DateTime.Now.ToString("yyyyMMdd");
        }
        public int TodaysPrayCount()
        {
            var target = DateTime.Now.ToString("yyyyMMdd");
            return _prayDate.Count(d => d == target);
        }
        public void CountUpPray()
        {
            _prayDate.Add(DateTime.Now.ToString("yyyyMMdd"));
        }
        public void ClearPrayCount()
        {
            _prayDate.Clear();
        }
        public int TodaysBattleCount()
        {
            var target = DateTime.Now.ToString("yyyyMMdd");
            return _battleDate.Count(d => d == target);
        }
        public void CountUpBattle()
        {
            _battleDate.Add(DateTime.Now.ToString("yyyyMMdd"));
        }
        public void ClearBattleCount()
        {
            _battleDate.Clear();
        }

        public void AddBattleHistory(UserProfile user, HISTORY_TYPE type)
        {
            var str = DateTime.Now.ToString("MM/dd HH:mm") + Environment.NewLine;
            switch (type)
            {
                case HISTORY_TYPE.WIN_OFFLINE:
                    str += $"{user.DisplayName}を撃退" + Environment.NewLine;
                    str += $"「{user.LoseComment}」" + Environment.NewLine;
                    break;
                case HISTORY_TYPE.LOSE_OFFLINE:
                    str += $"{user.DisplayName}から襲撃" + Environment.NewLine;
                    str += $"「{user.WinComment}」" + Environment.NewLine;
                    break;
                case HISTORY_TYPE.WIN:
                    str += $"{user.DisplayName}を撃破" + Environment.NewLine;
                    str += $"「{user.LoseComment}」" + Environment.NewLine;
                    ContinuousWin++;
                    ContinuousLose = 0;
                    break;
                case HISTORY_TYPE.LOSE:
                    str += $"{user.DisplayName}に敗退" + Environment.NewLine;
                    str += $"「{user.WinComment}」" + Environment.NewLine;
                    ContinuousWin = 0;
                    ContinuousLose++;
                    break;
            }
            History.Insert(0, str);
            if (History.Count >= MAX_HISTOTY)
                History.RemoveAt(MAX_HISTOTY - 1);
            AddedHistory.Add(str);
            if (AddedHistory.Count >= MAX_HISTOTY)
                AddedHistory.RemoveAt(0);
        }
        public void ClearContinusLoginRecord()
        {
            ContinuousLogin = 0;
            RareLogin = false;
        }
        public void ClearContinusBattleRecord()
        {
            ContinuousWin = 0;
            ContinuousLose = 0;
        }
        public enum HISTORY_TYPE
        {
            WIN, LOSE, WIN_OFFLINE, LOSE_OFFLINE
        }

        public string GetBattleComment()
        {
            return BattleComment[MyRandom.GetRandom(BattleComment.Count)];
        }

        public bool RobName(Name name)
        {
            // クリアしてる人はなくならない
            if (_isCleared)
                return false;

            if (Names.Count > 2)
            {
                while (name.Equals(MainName))
                {
                    MainName = Names[MyRandom.GetRandom(Names.Count)];
                }
                while (name.Equals(SubName))
                {
                    SubName = Names[MyRandom.GetRandom(Names.Count)];
                }
            }
            else
            {
                if (name.Equals(MainName)) MainName = null;
                if (name.Equals(SubName)) SubName = null;
            }
            Names.Remove(name);
            RobbedNames.Add(name);
            return RecoveryNameSettingIfNeed();
        }

        public void AddNewName(Name name)
        {
            var before = _isCleared;
            Names.Add(name);
            if (MainName == null || !Names.Contains(MainName)) MainName = name;
            else if (SubName == null || !Names.Contains(SubName)) SubName = name;
            var after = _isCleared;
            if (!before && after)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    GameClear?.Invoke(this, EventArgs.Empty);
                });
            }
            DependencyService.Get<IDeviceService>().LogEvent("GetName", "Count", Names.Count.ToString());
        }

        public bool RecoveryNameSettingIfNeed()
        {
            bool needPopupNotification = false;
            while (Names.Count < 2)
            {
                var masters = CodeMasterServce.GetMasters<JobMaster>();
                var target1 = CodeMasterServce.GetNameRandom();
                while (Names.Any(item => target1.Attribute == item.Attribute))
                {
                    target1 = CodeMasterServce.GetNameRandom();
                }
                var target2 = masters[MyRandom.GetRandom(masters.Count)];
                var gotName = new Name(target1, target2);
                Names.Add(gotName);
                needPopupNotification = true;
            }
            if (Names.Count >= 1 && (MainName == null || !Names.Contains(MainName)))
            {
                while ((SubName != null && SubName.Equals(MainName)) || (MainName == null || !Names.Contains(MainName)))
                {
                    MainName = Names[MyRandom.GetRandom(Names.Count)];
                }
                needPopupNotification = true;
            }
            if (Names.Count >= 2 && (SubName == null || !Names.Contains(SubName)))
            {
                while ((SubName != null && SubName.Equals(MainName)) || (SubName == null || !Names.Contains(SubName)))
                {
                    SubName = Names[MyRandom.GetRandom(Names.Count)];
                }
                needPopupNotification = true;
            }
            return needPopupNotification;
        }

    }
}
