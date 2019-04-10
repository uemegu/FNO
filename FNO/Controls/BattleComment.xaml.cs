using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FNO.Utils;

namespace FNO.Controls
{
    public partial class BattleComment : ContentView
    {
        public static readonly BindableProperty TextProperty =
       BindableProperty.Create("Text", typeof(String), typeof(BattleComment));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty FontSizeProperty =
       BindableProperty.Create("FontSize", typeof(int), typeof(BattleComment));

        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public BattleComment()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public static void Show(Layout<View> layout, string comment, int delay = 2000, Action callback = null)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var container = new AbsoluteLayout();
                var thisObj = new BattleComment();
                thisObj.Text = comment;
                AbsoluteLayout.SetLayoutFlags(thisObj, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(thisObj,
                new Rectangle(0.5, 0.1 + (double)MyRandom.GetRandom(80) / 100f,
                    AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                thisObj.FontSize = MyRandom.GetRandom(20) + 12;
                container.Children.Add(thisObj);
                layout.Children.Add(container);
                Device.BeginInvokeOnMainThread(() =>
                {
                    thisObj.Area.FadeTo(0, (uint)delay);
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
