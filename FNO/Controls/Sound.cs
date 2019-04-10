using System;
using Xamarin.Forms;

namespace FNO.Controls
{
    public class Sound
    {
        public static void OK()
        {
            DependencyService.Get<IDeviceService>().PlayVibrate();
            DependencyService.Get<IDeviceService>().PlayBeep();
        }
        public static void NG()
        {
            DependencyService.Get<IDeviceService>().PlayVibrate();
            DependencyService.Get<IDeviceService>().PlayNG();
        }
        public static void Normal()
        {
            DependencyService.Get<IDeviceService>().PlayMusicAsync("bgm1");
        }
        public static void WIN()
        {
            DependencyService.Get<IDeviceService>().PlayMusicAsync("bgm4");
            DependencyService.Get<IDeviceService>().StartPlayEffect("win");
        }
        public static void LOSE()
        {
            DependencyService.Get<IDeviceService>().StartPlayEffect("lose");
        }
        public static void Battle()
        {
            DependencyService.Get<IDeviceService>().PlayMusicAsync("bgm3");
        }
        public static void Pray()
        {
            DependencyService.Get<IDeviceService>().PlayMusicAsync("bgm2");
        }
        public static void PrayVoice()
        {
            DependencyService.Get<IDeviceService>().StartPlayEffect("GivingName");
        }
        public static void SelectStart()
        {
            DependencyService.Get<IDeviceService>().StartPlayEffect("start");
        }
        public static void SelectPrayMenu()
        {
            DependencyService.Get<IDeviceService>().StartPlayEffect("pray");
        }
        public static void SelectBattleMenu()
        {
            DependencyService.Get<IDeviceService>().StartPlayEffect("battle");
        }
        public static void SelectStatusMenu()
        {
            DependencyService.Get<IDeviceService>().StartPlayEffect("status");
        }
    }
}
