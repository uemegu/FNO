using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class MagicCircle : ContentView
    {
        private static MagicCircle _prev;

        public MagicCircle()
        {
            InitializeComponent();
            BackImage.Source = ImageSource.FromResource("FNO.Resouces.Back1.png");
            Circle.Source = ImageSource.FromResource("FNO.Resouces.MagicCircle.png");
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
            var thisObj = new MagicCircle();
            if (_prev != null)
            {
                _prev.IsVisible = false;
            }
            _prev = thisObj;
            AbsoluteLayout.SetLayoutFlags(thisObj, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(thisObj, new Rectangle(0, 0, 1, 1));
            container.Children.Add(thisObj);
            layout.Children.Add(container);
            thisObj.BackImage.IsVisible = true;
            thisObj.Circle.FadeTo(1, 50);
            Device.BeginInvokeOnMainThread(() =>
            {
                thisObj.Circle.RotateTo(Math.PI * 10, (uint)delay, Easing.CubicIn);
                Task.Delay(delay * 4 / 5).ContinueWith((arg) =>
                {
                    thisObj.Circle.FadeTo(0, (uint)(delay / 5));
                });
            });
            Device.StartTimer(TimeSpan.FromMilliseconds(delay), () =>
            {
                layout.Children.Remove(container);
                callback?.Invoke();
                _prev = null;
                return false;
            });
        }
    }
}
