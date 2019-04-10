

using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class TransitionButton : ContentView
    {
        public event EventHandler Clicked;

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(String), typeof(TransitionButton));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command", typeof(Command), typeof(TransitionButton));

        public Command Command
        {
            get { return (Command)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public TransitionButton()
        {
            InitializeComponent();
            BindingContext = this;
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Sound.OK();
            Clicked?.Invoke(this, e);
        }

    }
}
