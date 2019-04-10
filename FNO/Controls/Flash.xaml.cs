using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class Flash : ContentView
    {
        public Flash()
        {
            InitializeComponent();
        }

        public static void Show(ContentView page, int delay = 200, Action callback = null)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var layout = page.Content as Layout<View>;
                ShowCore(delay, callback, layout);
            });
        }

        public static void Show(ContentPage page, int delay = 200, Action callback = null)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var layout = page.Content as Layout<View>;
                ShowCore(delay, callback, layout);
            });
        }

        private static void ShowCore(int delay, Action callback, Layout<View> layout)
        {
            var container = new AbsoluteLayout();
            var thisObj = new Flash();
            AbsoluteLayout.SetLayoutFlags(thisObj, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(thisObj, new Rectangle(0, 0, 1, 1));
            container.Children.Add(thisObj);
            layout.Children.Add(container);
            Device.BeginInvokeOnMainThread(() =>
            {
                thisObj.Area.FadeTo(1, 50).ContinueWith((arg) =>
                {
                    thisObj.Area.FadeTo(0, (uint)(delay - 50));
                });
            });
            Device.StartTimer(TimeSpan.FromMilliseconds(delay), () =>
            {
                layout.Children.Remove(container);
                callback?.Invoke();
                return false;
            });
        }
    }
}
