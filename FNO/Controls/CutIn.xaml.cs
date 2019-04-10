using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class CutIn : ContentView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(String), typeof(Explanation));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public CutIn()
        {
            InitializeComponent();
            BindingContext = this;
            Area.Opacity = 0;
        }

        public static void Show(ContentView page, string text, int delay, Action callback)
        {
            var layout = page.Content as Layout<View>;
            var container = new AbsoluteLayout();
            var thisObj = new CutIn();
            thisObj.Text = text;
            AbsoluteLayout.SetLayoutFlags(thisObj, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(thisObj, new Rectangle(0, 0, 1, 1));
            container.Children.Add(thisObj);
            layout.Children.Add(container);
            Device.BeginInvokeOnMainThread(() =>
            {
                thisObj.Area.FadeTo(1, 350).ContinueWith((arg) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        thisObj.Discription.IsVisible = true;
                    });
                });
            });
            Device.StartTimer(TimeSpan.FromSeconds(delay), () =>
             {
                 layout.Children.Remove(container);
                 callback?.Invoke();
                 return false;
             });
        }
    }
}
