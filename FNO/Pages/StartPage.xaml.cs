
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using FNO.Controls;
using FNO.Models;

namespace FNO.Pages
{
    public partial class StartPage : BasePage
    {
        public StartPage()
        {
            InitializeComponent();
            BackImage.Source = ImageSource.FromResource("FNO.Resouces.Back1.png");
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Sound.SelectStart();
            App.Transition(new StartingPage());
        }

        void Handle_Clicked2(object sender, System.EventArgs e)
        {
            First.IsVisible = false;
            Second.IsVisible = true;
        }

        async void Handle_Clicked3(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PWEntry.Text))
            {
                await App.ShowMessage("復活の呪文を入力してください");
                return;
            }
            Second.IsVisible = false;

            var user = await DependencyService.Get<IDeviceService>().LoadUserAsync(PWEntry.Text);
            if (user == null)
            {
                await App.ShowMessage("復活の呪文が違います");
                Second.IsVisible = true;
                return;
            }

            new ID() { Id = user.Id }.Save();
            await App.ShowMessage("読み込みに成功しました");

            First.IsVisible = true;
        }

        void Handle_Clicked4(object sender, System.EventArgs e)
        {
            First.IsVisible = true;
            Second.IsVisible = false;
        }

        async void Handle_Clicked5Async(object sender, System.EventArgs e)
        {
            var resut = await Application.Current.MainPage.DisplayAlert("確認",
                "端末内に保存されているユーザーデータを消去しますか？" +
                "サーバーに保存されているデータは一定期間ログインがなければ自動的に消去されます",
            "消去する", "キャンセル");
            if (resut)
            {
                ID.Delete();
                await App.ShowMessage("データは消去されました");
            }
        }
    }
}
