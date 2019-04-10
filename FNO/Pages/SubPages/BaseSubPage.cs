using System;
using Xamarin.Forms;
using FNO.Controls;
using FNO.ViewModels;

namespace FNO.Pages.SubPages
{
    public class BaseSubPage : ContentView
    {
        private Action _backAction;
        public Action Disappeared;
        protected MainViewModel _vm;

        public BaseSubPage(TRANSITON_FROM from)
        {
            BackgroundColor = (Color)App.GetStyle()["Background"];
            Opacity = 0;
            AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.SizeProportional);
            if (from == TRANSITON_FROM.RIGHT)
            {
                AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 0, 1, 1));
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.TranslateTo(this.Width, 0, 0).ContinueWith((arg) => ShowAnimation());
                });
                _backAction = () => this.TranslateTo(this.Width, 0, 200).ContinueWith((arg) => Dispose());
            }
            else if (from == TRANSITON_FROM.LEFT)
            {
                AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 0, 1, 1));
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.TranslateTo(-this.Width, 0, 0).ContinueWith((arg) => ShowAnimation());
                });
                _backAction = () => this.TranslateTo(-this.Width, 0, 200).ContinueWith((arg) => Dispose());
            }
            else
            {
                AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 0, 1, 1));
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.TranslateTo(0, this.Height, 0).ContinueWith((arg) => ShowAnimation());
                });
                _backAction = () => this.TranslateTo(0, this.Height, 200).ContinueWith((arg) => Dispose());
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (_vm == null)
            {
                _vm = BindingContext as MainViewModel;
            }
        }

        private void ShowAnimation()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Opacity = 1;
                this.TranslateTo(0, 0, 200);
            });
        }

        virtual protected void Back(bool dontSave = false)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!dontSave)
                {
                    _vm.Save();
                }
                Sound.Normal();
                _backAction();
            });
        }

        protected void Dispose()
        {
            Device.BeginInvokeOnMainThread(Disappeared);
        }

    }

    public enum TRANSITON_FROM
    {
        RIGHT, BOTTOM, LEFT
    }
}
