
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class Explanation : ContentView
    {
        public event EventHandler Clicked;

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(String), typeof(Explanation));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Explanation()
        {
            InitializeComponent();
            BindingContext = this;

            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) =>
            {
                Clicked?.Invoke(this, e);
            };
            GestureRecognizers.Add(tgr);

            Back.Opacity = 0;
            Back.ScaleTo(0.1, 0);
            Device.BeginInvokeOnMainThread(() =>
            {
                Back.ScaleTo(10, 350);
                Back.FadeTo(0.5, 350).ContinueWith((arg) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Discription.IsVisible = true;
                    });
                });
            });
        }
    }
}
