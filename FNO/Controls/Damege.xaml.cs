

using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FNO.Utils;

namespace FNO.Controls
{
    public partial class Damege : ContentView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(String), typeof(Damege));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Damege()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public static void Show(Layout<View> layout, string damage, int delay = 1000, Action callback = null)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var container = new AbsoluteLayout();
                var thisObj = new Damege();
                thisObj.Text = damage.ToString();
                AbsoluteLayout.SetLayoutFlags(thisObj, AbsoluteLayoutFlags.SizeProportional);
                AbsoluteLayout.SetLayoutBounds(thisObj, new Rectangle(0, 0, 1, 1));
                container.Children.Add(thisObj);
                layout.Children.Add(container);
                Device.BeginInvokeOnMainThread(() =>
                {
                    thisObj.Area.FadeTo(0, (uint)delay);
                    thisObj.Area.TranslateTo(0, -300, (uint)delay);
                });
                Device.StartTimer(TimeSpan.FromMilliseconds(delay), () =>
                {
                    layout.Children.Remove(container);
                    callback?.Invoke();
                    return false;
                });
            });
        }
    }
}
