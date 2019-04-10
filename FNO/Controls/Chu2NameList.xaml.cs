using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FNO.Models;

namespace FNO.Controls
{
    public partial class Chu2NameList : ContentView
    {
        public event EventHandler NameSelected;
        public Name SelectedName { get; private set; }

        public Chu2NameList()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var user = BindingContext as UserProfile;
            if (user == null) return;

            Containter.Children.Clear();
            foreach (var item in user.Names)
            {
                var element = new Chu2Name();
                element.BindingContext = item;
                element.ShowParameter = true;
                element.ShowAttribute = true;
                element.Margin = new Thickness(10);
                var tgr = new TapGestureRecognizer();
                tgr.Tapped += (s, e) =>
                {
                    SelectedName = item;
                    NameSelected?.Invoke(this, EventArgs.Empty);
                };
                element.GestureRecognizers.Add(tgr);
                Containter.Children.Add(element);
            }
        }
    }
}
