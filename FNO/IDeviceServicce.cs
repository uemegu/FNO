using System;
using System.Threading.Tasks;
using FNO.Models;

namespace FNO
{
    public interface IDeviceService
    {
        void PlayVibrate();
        void PlayBeep();
        void PlayNG();
        void CopyToClipBoard(string str);
        void ShowToast(string message);
        void LogEvent(string eventId, string paramName, string value);
        void LogEvent(string eventId, System.Collections.Generic.IDictionary<string, string> parameters);
        void ShowAd(Action<int> action);
        void LoadAd();
        void PlayMusicAsync(string title);
        void StartPlayEffect(string title);
        void StopMusic();
        void SaveData(UserProfile user);
        void SaveOtherData(UserProfile user);
        string SaveNewUser(UserProfile user);
        Task<UserProfile> LoadUserAsync(string id);
        Task<UserProfile> LoadUserRandomAsync();
    }
}
