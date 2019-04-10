using System;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using FNO.Controls;

namespace FNO.Pages
{
    public class BasePage : ContentPage
    {
        public BasePage()
        {
            BackgroundColor = (Color)App.GetStyle()["Background"];
            if (App.IsNeedMargin)
            {
                this.Padding = new Thickness(0, 30, 0, 30);
            }
            On<iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True)
            .SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.Fade);
        }
    }
}

