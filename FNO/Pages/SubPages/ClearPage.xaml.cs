using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Pages.SubPages
{
    public partial class ClearPage : BaseSubPage
    {
        public ClearPage(TRANSITON_FROM from) : base(from)
        {
            InitializeComponent();
            BackImage.Source = ImageSource.FromResource("FNO.Resouces.Back1.png");
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (_vm == null)
            {
                DependencyService.Get<IDeviceService>().LogEvent("Clear", "Player", _vm.User.DisplayName);
            }
        }

        void Handle_Back(object sender, System.EventArgs e)
        {
            Back();
        }
    }
}
