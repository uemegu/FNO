using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class Charactor : ContentView
    {
        private bool _nowAnimation = false;

        public Charactor()
        {
            InitializeComponent();
        }

        public void Damaged()
        {
            if (_nowAnimation) return;
            _nowAnimation = true;

            Device.BeginInvokeOnMainThread(() =>
            {
                Img.Scale = 0.9;
                Img.ScaleTo(1, 100).ContinueWith((arg) => _nowAnimation = false);
            });
        }
    }
}
