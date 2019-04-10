using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class HPBar : ContentView
    {
        private bool _animation = false;

        public HPBar()
        {
            InitializeComponent();
            HPLabel.PropertyChanged += HPLabel_PropertyChanged;
        }

        void HPLabel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                if (_animation)
                    return;

                _animation = true;
                HPLabel.ScaleTo(2, 100).ContinueWith((arg) => { 
                    HPLabel.ScaleTo(1, 100).ContinueWith((arg2) => { _animation = false; });
                });
            }
        }

    }
}
