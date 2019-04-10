
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FNO.Models;

namespace FNO.Controls
{
    public partial class Chu2Name : ContentView
    {
        public static readonly BindableProperty ShowAttributeTypeProperty =
            BindableProperty.Create("ShowAttributeType", typeof(bool), typeof(Chu2Name), false, propertyChanged: HandleBindingPropertyChangedDelegate);

        public bool ShowAttributeType
        {
            get { return (bool)GetValue(ShowAttributeTypeProperty); }
            set { SetValue(ShowAttributeTypeProperty, value); }
        }

        public static readonly BindableProperty ShowParameterProperty =
            BindableProperty.Create("ShowParameter", typeof(bool), typeof(Chu2Name), false, propertyChanged: HandleBindingPropertyChangedDelegate);

        public bool ShowParameter
        {
            get { return (bool)GetValue(ShowParameterProperty); }
            set { SetValue(ShowParameterProperty, value); }
        }

        public static readonly BindableProperty ShowAttributeProperty =
            BindableProperty.Create("ShowAttribute", typeof(bool), typeof(Chu2Name), false, propertyChanged: HandleBindingPropertyChangedDelegate);

        public bool ShowAttribute
        {
            get { return (bool)GetValue(ShowAttributeProperty); }
            set { SetValue(ShowAttributeProperty, value); }
        }

        private static void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            var name = bindable as Chu2Name;
            name.CheckStyle();
        }

        public Chu2Name()
        {
            InitializeComponent();
            BindingContextChanged += Chu2Name_BindingContextChanged;
        }

        void Chu2Name_BindingContextChanged(object sender, EventArgs e)
        {
            CheckStyle();
        }

        private void CheckStyle()
        {
            AttributeLabel.IsVisible = ShowAttribute;
            AttributeLabel2.IsVisible = ShowAttribute;
            Parameter.IsVisible = ShowParameter;
            AttributeType.IsVisible = ShowAttributeType;
            if (ShowAttribute)
            {
                NameContainer.HorizontalOptions = LayoutOptions.Start;
                NameContainer.Margin = new Thickness(50, 0, 0, 0);
                foreach (View element in NameContainer.Children)
                {
                    element.HorizontalOptions = LayoutOptions.Start;
                }
                NameContainer2.HorizontalOptions = LayoutOptions.Start;
                NameContainer2.Margin = new Thickness(50, 0, 0, 0);
                foreach (View element in NameContainer2.Children)
                {
                    element.HorizontalOptions = LayoutOptions.Start;
                }
            }
            else
            {
                NameContainer.HorizontalOptions = LayoutOptions.Center;
                NameContainer.Margin = new Thickness(0, 0, 0, 0);
                foreach (View element in NameContainer.Children)
                {
                    element.HorizontalOptions = LayoutOptions.Center;
                }
                NameContainer2.HorizontalOptions = LayoutOptions.Center;
                NameContainer2.Margin = new Thickness(0, 0, 0, 0);
                foreach (View element in NameContainer2.Children)
                {
                    element.HorizontalOptions = LayoutOptions.Center;
                }
            }

            var data = BindingContext as Name;
            if (data != null)
            {
                if (data.Attribute == "18")
                {
                    foreach (View element in NameContainer.Children)
                    {
                        if (element is Label)
                        {
                            ((Label)element).TextColor = Color.FromHex("#C3C3C3");
                        }
                    }
                }
                if (ShowAttributeType)
                {
                    if (data.AttributeType == ATTRIBUTE_TYPE.ACHIEVEMENT)
                    {
                        AttributeType.Text = "実績二つ名";
                    }
                    else if (data.AttributeType == ATTRIBUTE_TYPE.RARE)
                    {
                        AttributeType.Text = "レア二つ名";
                    }
                }
            }
        }
    }
}
