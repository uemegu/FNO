using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Essentials;
using FNO.Droid;
using Android.Gms.Ads;
using System.Threading.Tasks;
using FNO.Models;

[assembly: Dependency(typeof(DeviceService))]
namespace FNO.Droid
{
    public class DeviceService : IDeviceService
    {
        public void CopyToClipBoard(string str)
        {
            var clipboardManager = (ClipboardManager)Android.App.Application.Context.GetSystemService(Context.ClipboardService);
            ClipData clip = ClipData.NewPlainText("", str);
            clipboardManager.PrimaryClip = clip;
        }

        public void LoadAd()
        {
            throw new NotImplementedException();
        }

        public Task<UserProfile> LoadUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfile> LoadUserRandomAsync()
        {
            throw new NotImplementedException();
        }

        public void LogEvent(string eventId, string paramName, string value)
        {
            var p = new Bundle();
            p.PutString(paramName, value);
            MainActivity.firebaseAnalytics.LogEvent(eventId, p);
        }

        public void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
            var p = new Bundle();
            foreach (var item in parameters)
            {
                p.PutString(item.Key, item.Value);
            }
            MainActivity.firebaseAnalytics.LogEvent(eventId, p);
        }

        public void PlayBeep()
        {
            throw new NotImplementedException();
        }

        public void PlayMusicAsync(string title)
        {
            throw new NotImplementedException();
        }

        public void PlayNG()
        {
            throw new NotImplementedException();
        }

        public void PlayVibrate()
        {
            try
            {
                var duration = TimeSpan.FromMilliseconds(100);
                Vibration.Vibrate(duration);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public void SaveData(UserProfile user)
        {
            throw new NotImplementedException();
        }

        public void SaveOtherData(UserProfile user)
        {
            throw new NotImplementedException();
        }

        public void ShowAd(Action<int> action)
        {
            throw new NotImplementedException();
        }

        public void ShowToast(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }

        public void StartPlayEffect(string title)
        {
            throw new NotImplementedException();
        }

        public void StopMusic()
        {
            throw new NotImplementedException();
        }

        string IDeviceService.SaveNewUser(UserProfile user)
        {
            throw new NotImplementedException();
        }
    }
}
