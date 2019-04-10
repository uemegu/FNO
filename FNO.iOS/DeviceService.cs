using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioToolbox;
using AVFoundation;
using Firebase.Analytics;
using Foundation;
using Google.MobileAds;
using UIKit;
using Xamarin.Forms;
using FNO.iOS;
using FNO.Models;

[assembly: Dependency(typeof(DeviceService))]
namespace FNO.iOS
{
    public class DeviceService : IDeviceService
    {
        private Toast _toast = null;

        public void CopyToClipBoard(string str)
        {
            UIPasteboard clipboard = UIPasteboard.General;
            clipboard.String = str;
        }

        public void ShowToast(string message)
        {
            var view = UIApplication.SharedApplication.KeyWindow.RootViewController.View;
            if (_toast != null)
            {
                if (_toast.IsShowing(view))
                {
                    _toast.Cancel();
                }
            }
            //新たにToastを表示する
            _toast = new Toast();
            _toast.Show(view, message);
        }

        #region Sound
        private SystemSound _vibe = null;
        private SystemSound _beep = null;
        private SystemSound _ng = null;
        private AVAudioPlayer player = null;
        private AVAudioPlayer effect = null;

        public void PlayVibrate()
        {
            if (_vibe == null)
            {
                _vibe = new SystemSound(1519);
            }
            _vibe.PlaySystemSound();
        }

        public void PlayBeep()
        {
            if (_beep == null)
            {
                _beep = new SystemSound(1103);
            }
            _beep.PlaySystemSound();
        }

        public void PlayNG()
        {
            if (_ng == null)
            {
                _ng = new SystemSound(1105);
            }
            _ng.PlaySystemSound();
        }

        private void StartPlayer(string title)
        {
            if (player == null)
            {
                var url = NSUrl.FromFilename(title + ".mp3");

                NSError _err = null;

                player = new AVAudioPlayer(
                            url,
                            AVFileType.MpegLayer3,
                            out _err
                         );

                player.Volume = 0.5f;
                player.NumberOfLoops = -1;//player.Looping = true;
                player.PrepareToPlay();
                player.Play();
            }
        }

        public void StartPlayEffect(string title)
        {
            try
            {
                if (effect != null)
                {
                    effect.Stop();
                    effect.Dispose();
                    effect = null;
                }
                var url = NSUrl.FromFilename(title + ".mp3");

                NSError _err = null;

                effect = new AVAudioPlayer(
                            url,
                            AVFileType.MpegLayer3,
                            out _err
                         );

                effect.NumberOfLoops = 0;
                effect.PrepareToPlay();
                effect.Play();
            }
            catch
            {
                //ignore
            }
        }

        private void StopPlayer()
        {
            if (player != null)
            {
                if (player.Playing)
                {
                    player.Stop();
                }
                player.Dispose();
                player = null;
            }
        }

        public void PlayMusicAsync(string title)
        {
            StopPlayer();
            StartPlayer(title);
        }

        public void StopMusic()
        {
            StopPlayer();
        }
        #endregion

        #region LOG
        public void LogEvent(string eventId, string paramName, string value)
        {
            LogEvent(eventId, new Dictionary<string, string>
            {
                { paramName, value }
            });
        }

        public void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
#if !DEBUG
            var keys = new List<NSString>();
            var values = new List<NSString>();
            foreach (var item in parameters)
            {
                keys.Add(new NSString(item.Key));
                values.Add(new NSString(item.Value));
            }

            var parametersDictionary =
                NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values.ToArray(), keys.ToArray(), keys.Count);
            Analytics.LogEvent(eventId, parametersDictionary);
#endif
        }
        #endregion

        #region Ad
        public void LoadAd()
        {
#if !DEBUG
            RewardBasedVideoAd.SharedInstance.LoadRequest(Request.GetDefaultRequest(), Const.AdMobAdID);
#endif
        }

        public void ShowAd(Action<int> action)
        {
#if !DEBUG
            if (RewardBasedVideoAd.SharedInstance.IsReady)
            {
                RewardBasedVideoAd.SharedInstance.Closed += (s, e) => LoadAd();
                RewardBasedVideoAd.SharedInstance.UserRewarded += (object sender, RewardBasedVideoAdRewardEventArgs e) =>
                {
                    action?.Invoke(e.Reward.Amount.Int32Value);
                };
                RewardBasedVideoAd.SharedInstance.PresentFromRootViewController(GetTopViewController());
            }
            else
            {
                ShowToast("表示可能な広告がありませんでした");
                action?.Invoke(5);
            }
#endif
        }

        public static UIViewController GetTopViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;

            if (vc is UINavigationController navController)
                vc = navController.ViewControllers.Last();

            return vc;
        }
        #endregion



        public void SaveData(UserProfile user)
        {
            StorageService.SaveUser(user, false);
        }

        public void SaveOtherData(UserProfile user)
        {
            StorageService.SaveUser(user, true);
        }

        public string SaveNewUser(UserProfile user)
        {
            return StorageService.SaveNewUser(user);
        }

        public async Task<UserProfile> LoadUserAsync(string id)
        {
            return await StorageService.GetUserAsync(id);
        }

        public async Task<UserProfile> LoadUserRandomAsync()
        {
            return await StorageService.GetUserRandomAsync();
        }

    }
}
