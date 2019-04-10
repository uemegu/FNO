using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FNO.Models;

namespace FNO.Controls
{
    public partial class CutInFace : ContentView
    {
        private static CutInFace _prev;
        public CutInFace()
        {
            InitializeComponent();
            Area.Opacity = 0;
        }

        public static void Show(ContentView page, string text, Name name, bool isRightSide, int delay)
        {
            var layout = page.Content as Layout<View>;
            var container = new AbsoluteLayout();
            var thisObj = new CutInFace();
            if (_prev != null)
            {
                _prev.IsVisible = false;
            }
            _prev = thisObj;
            thisObj.Discription.Text = text;
            thisObj.BindingContext = name;
            if (!isRightSide)
            {
                thisObj.Img.ScaleX = -1;
            }
            AbsoluteLayout.SetLayoutFlags(thisObj, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(thisObj, new Rectangle(0, 0, 1, 1));
            container.Children.Add(thisObj);
            layout.Children.Add(container);
            Device.BeginInvokeOnMainThread(() =>
            {
                thisObj.Area.FadeTo(1, 200).ContinueWith((arg) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        thisObj.Discription.IsVisible = true;
                    });
                });
            });
            Device.StartTimer(TimeSpan.FromSeconds(delay), () =>
            {
                _prev = null;
                layout.Children.Remove(container);
                return false;
            });
        }
    }
}
