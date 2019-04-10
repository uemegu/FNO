using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Controls
{
    public partial class Explanation2 : ContentView
    {
        public Explanation2()
        {
            InitializeComponent();
        }

        public static void Show(ContentView page, string text)
        {
            var layout = page.Content as Layout<View>;
            var container = new AbsoluteLayout();
            var thisObj = new Explanation2();
            thisObj.Explain.Text = text;
            thisObj.BackButton.Clicked += (sender, e) => layout.Children.Remove(container);
            AbsoluteLayout.SetLayoutFlags(thisObj, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(thisObj, new Rectangle(0, 0, 1, 1));
            container.Children.Add(thisObj);
            layout.Children.Add(container);
        }
    }
}
