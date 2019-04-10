using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class SelectItem : ContentView
    {
        public class Item
        {
            public string Label { get; set; }
            public Action Selected { get; set; }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(String), typeof(SelectItem));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create("Items", typeof(IList<Item>), typeof(SelectItem), propertyChanged: ItemsChanged);

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisObj = bindable as SelectItem;
            thisObj.Area.Children.Clear();
            foreach (var i in thisObj.Items)
            {
                var element = new TransitionButton();
                element.Text = i.Label;
                element.Clicked += (object sender, EventArgs e) => i?.Selected();
                element.Margin = new Thickness(10);
                thisObj.Area.Children.Add(element);
            }
        }

        public IList<Item> Items
        {
            get { return (IList<Item>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public SelectItem()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public static void Show(ContentView page, string text, IList<Item> items)
        {
            var layout = page.Content as Layout<View>;
            var container = new AbsoluteLayout();
            var thisObj = new SelectItem();
            thisObj.Text = text;
            thisObj.Items = items;
            AbsoluteLayout.SetLayoutFlags(thisObj, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(thisObj, new Rectangle(0, 0, 1, 1));
            container.Children.Add(thisObj);
            layout.Children.Add(container);
            Action destroy = () =>
            {
                layout.Children.Remove(container);
            };
            foreach (var i in items)
            {
                var tmp = i.Selected;
                i.Selected = () =>
                {
                    Sound.OK();
                    tmp?.Invoke();
                    destroy();
                };
            }
        }
    }
}
