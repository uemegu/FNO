
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class MenuButton : ContentView
    {
        public event EventHandler Clicked;

        public static readonly BindableProperty NameProperty =
            BindableProperty.Create("Name", typeof(String), typeof(MenuButton), string.Empty, propertyChanged: HandleBindingPropertyChangedDelegate);

        public String Name
        {
            get { return (String)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly BindableProperty SubNameProperty =
            BindableProperty.Create("SubName", typeof(String), typeof(MenuButton), string.Empty, propertyChanged: HandleBindingPropertyChangedDelegate);

        public String SubName
        {
            get { return (String)GetValue(SubNameProperty); }
            set { SetValue(SubNameProperty, value); }
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command", typeof(Command), typeof(MenuButton));

        public Command Command
        {
            get { return (Command)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public MenuButton()
        {
            InitializeComponent();

            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) =>
            {
                Sound.OK();
                Clicked?.Invoke(this, e);
            };
            GestureRecognizers.Add(tgr);
        }

        private static void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            var button = bindable as MenuButton;
            button.ChangeName();
        }

        private void ChangeName()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                FirstCharLabel.Text = Name[0].ToString();
                if (Name.Length > 1)
                {
                    NameLabel.Text = Name.Substring(1);
                }
            }
            else
            {
                FirstCharLabel.Text = string.Empty;
                NameLabel.Text = string.Empty;
            }
            SubNameLabel.Text = SubName;
        }
    }
}
