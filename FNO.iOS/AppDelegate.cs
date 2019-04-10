using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Google.MobileAds;
using Firebase.CloudMessaging;
using Lottie.Forms.iOS.Renderers;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using FNO.Pages;
using Firebase.RemoteConfig;

namespace FNO.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IMessagingDelegate
    {
        public static bool GetNeedBezelPadding()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if ((int)(UIScreen.MainScreen.NativeBounds.Height) >= 2436)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            App.IsNeedMargin = GetNeedBezelPadding();
            LoadApplication(new App());


            if (Const.Offline)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    App.Transition(new StartPage());
                });
            }
            else
            {
                Firebase.Core.App.Configure();
                Messaging.SharedInstance.Delegate = this;
                RemoteConfig.SharedInstance.ConfigSettings = new RemoteConfigSettings(false);
                FetchRemoteConfigFromServer();
                MobileAds.Configure(Const.AdMobID);
                DependencyService.Get<IDeviceService>().LoadAd();

#if !DEBUG
                var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
                app.RegisterUserNotificationSettings(settings);
                app.RegisterForRemoteNotifications();
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
#endif

                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    var canUseStore = await StorageService.Initialize();
                    if (!canUseStore)
                    {
                        await App.ShowMessage("現在ご利用できません");
                        throw new Exception("Firebase setup failed");
                    }
                    App.Transition(new StartPage());
                });
            }

            AnimationViewRenderer.Init();
            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
#if !DEBUG
            Messaging.SharedInstance.ApnsToken = deviceToken;
#endif
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            StorageService.DeviceToken = fcmToken;
        }

        private void FetchRemoteConfigFromServer()
        {
            RemoteConfig.SharedInstance.Fetch((status, error) =>
            {
                switch (status)
                {
                    case RemoteConfigFetchStatus.Success:
                        RemoteConfig.SharedInstance.ActivateFetched();
                        ReadRemoteConfig();
                        break;

                    case RemoteConfigFetchStatus.Throttled:
                    case RemoteConfigFetchStatus.NoFetchYet:
                    case RemoteConfigFetchStatus.Failure:
                        break;
                }
            });
        }

        private static void ReadRemoteConfig()
        {
            var enabled = RemoteConfig.SharedInstance["Applicatoin_Enable"].BoolValue;
            if (!enabled)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.ShowMessage("現在ご利用できません");
                    throw new Exception("Not Enable");
                });
            }

            var version = RemoteConfig.SharedInstance["Application_Version"].StringValue;
            var current = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
            current = current.Replace(".", "");
            var currentNumber = int.Parse(current);
            var min = version.Replace(".", "");
            var minNumber = int.Parse(min);
            if (currentNumber < minNumber)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.ShowMessage("App Storeより最新版をインストールしてください");
                    throw new Exception("Version Wrong");
                });
            }

            Const.AttackPower_NORMAL = RemoteConfig.SharedInstance["AttackPower_NORMAL"].NumberValue.Int32Value;
            Const.AttackPower_RARE = RemoteConfig.SharedInstance["AttackPower_RARE"].NumberValue.Int32Value;
            Const.AttackPower_ACHIEVEMENT = RemoteConfig.SharedInstance["AttackPower_ACHIEVEMENT"].NumberValue.Int32Value;
            Const.BattleAndPray_MaxCount = RemoteConfig.SharedInstance["BattleAndPray_MaxCount"].NumberValue.Int32Value;
            Const.GetRareName_Rate = RemoteConfig.SharedInstance["GetRareName_Rate"].NumberValue.Int32Value;
            Const.AttackBounus_NotSameDistance = RemoteConfig.SharedInstance["AttackBounus_NotSameDistance"].NumberValue.DoubleValue;
            Const.AttackBounus_SameDistance = RemoteConfig.SharedInstance["AttackBounus_SameDistance"].NumberValue.DoubleValue;
            Const.StrongAttack_NearCenter = RemoteConfig.SharedInstance["StrongAttack_NearCenter"].NumberValue.DoubleValue;
            Const.StrongAttack_NotNearCenter = RemoteConfig.SharedInstance["StrongAttack_NotNearCenter"].NumberValue.DoubleValue;
            Const.HistoryMaxCount = RemoteConfig.SharedInstance["HistoryMaxCount"].NumberValue.Int32Value;
        }
    }

#if !DEBUG
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            //ignore
        }
    }
#endif
}
