using System;
using System.Collections.Generic;

using Xamarin.Forms;
using FNO.ViewModels;

namespace FNO.Pages.SubPages
{
    public partial class ShowPassword : BaseSubPage
    {
        public ShowPassword(TRANSITON_FROM from) : base(from)
        {
            InitializeComponent();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Back();
        }
        void Handle_Clicked2(object sender, System.EventArgs e)
        {
            var vm = BindingContext as MainViewModel;
            DependencyService.Get<IDeviceService>().CopyToClipBoard(vm.User.Id);
            DependencyService.Get<IDeviceService>().ShowToast("復活の呪文をコピーしました");
        }
    }
}
