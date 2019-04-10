using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class NextBack : ContentView
    {
        public event EventHandler OK;
        public event EventHandler Next;
        public event EventHandler Back;

        public static readonly BindableProperty ButtonTypeProperty =
            BindableProperty.Create("ButtonType", typeof(DISPLAY_TYPE), typeof(NextBack), propertyChanged: HandleBindingPropertyChangedDelegate);

        public DISPLAY_TYPE ButtonType
        {
            get { return (DISPLAY_TYPE)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

        public static readonly BindableProperty ClickButtonProperty =
            BindableProperty.Create("ClickButton", typeof(Command<BUTTON_TYPE>), typeof(NextBack), propertyChanged: HandleBindingPropertyChangedDelegate);

        public Command<BUTTON_TYPE> ClickButton
        {
            get { return (Command<BUTTON_TYPE>)GetValue(ClickButtonProperty); }
            set { SetValue(ClickButtonProperty, value); }
        }

        public NextBack()
        {
            InitializeComponent();
            OKButton.Clicked += (object sender, EventArgs e) => { Sound.OK(); OK?.Invoke(this, e); };
            NextButton.Clicked += (object sender, EventArgs e) => { Sound.OK(); Next?.Invoke(this, e); };
            BackButton.Clicked += (object sender, EventArgs e) => { Sound.NG(); Back?.Invoke(this, e); };
            OKButton.Command = new Command(() => { Sound.OK(); ClickButton?.Execute(BUTTON_TYPE.OK); });
            NextButton.Command = new Command(() => { Sound.OK(); ClickButton?.Execute(BUTTON_TYPE.NEXT); });
            BackButton.Command = new Command(() => { Sound.OK(); ClickButton?.Execute(BUTTON_TYPE.BACK); });
        }

        private static void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            ((NextBack)bindable).OnTypeChanged();
        }

        private void OnTypeChanged()
        {
            switch (ButtonType)
            {
                case DISPLAY_TYPE.OK:
                    BackButton.IsVisible = false;
                    NextButton.IsVisible = false;
                    break;
                case DISPLAY_TYPE.BACK_NEXT:
                    OKButton.IsVisible = false;
                    break;
            }
        }


        public void Refresh()
        {
            OnTypeChanged();
        }
    }

    public enum DISPLAY_TYPE
    {
        OK, BACK_NEXT
    }

    public enum BUTTON_TYPE
    {
        OK, BACK, NEXT
    }
}
