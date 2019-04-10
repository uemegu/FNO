using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FNO.Models;

namespace FNO.Controls
{
    public partial class Attribute : ContentView
    {
        public Attribute()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var data = BindingContext as Name;
            if (data != null)
            {
                if (data.Attribute == "18")
                {
                    foreach (View element in Container.Children)
                    {
                        if (element is Label)
                        {
                            ((Label)element).TextColor = Color.FromHex("#C3C3C3");
                        }
                    }
                }
            }
        }
    }
}
