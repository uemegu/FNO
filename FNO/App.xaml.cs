using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FNO.Models;
using FNO.Pages;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FNO
{
    public partial class App : Application
    {
        private static Application _app;

        public static bool IsNeedMargin { get; set; } = false;
        public static event EventHandler ApplicationSleeped;
        public static event EventHandler ApplicationResumed;
        public static event UserProfileChangedHander CloudDataChanged;

        public App()
        {
            InitializeComponent();
            _app = this;


            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            };

            MainPage = new Splash();
        }

        public static void Transition(Page page)
        {
            _app.MainPage = page;
        }

        public static ResourceDictionary GetStyle()
        {
            return _app.Resources;
        }

        public static void NotifyCloudDataChanged(UserProfile profile)
        {
            CloudDataChanged?.Invoke(_app, new UserProfileChangedEvent(profile));
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            ApplicationSleeped?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnResume()
        {
            ApplicationResumed?.Invoke(this, EventArgs.Empty);
        }

        public static async Task ShowMessage(string e)
        {
            await Application.Current.MainPage.DisplayAlert(null, e, "OK");
        }
    }
}
