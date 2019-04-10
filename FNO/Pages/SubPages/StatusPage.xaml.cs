using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using FNO.Controls;
using FNO.Models;
using FNO.ViewModels;
using Attribute = FNO.Controls.Attribute;

namespace FNO.Pages.SubPages
{
    public partial class StatusPage : BaseSubPage
    {
        public StatusPage(TRANSITON_FROM from) : base(from)
        {
            InitializeComponent();

            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) =>
            {
                Sound.OK();
                MainPage.AddSubPage(new Chu2NameListPage(TRANSITON_FROM.BOTTOM));
            };
            MainChu2Name.GestureRecognizers.Add(tgr);
            SubChu2Name.GestureRecognizers.Add(tgr);

        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var vm = BindingContext as MainViewModel;
            if (vm == null)
                return;

            var num = 0;
            var masters = CodeMasterServce.GetMasters<AttributeMaster>();
            foreach (var master in masters)
            {
                var attribute = new Attribute();

                var hasAttribute = vm.User.Names.Any((name) => name.Attribute == master.Code);
                attribute.BindingContext = new
                {
                    CharactorColor = hasAttribute ? Color.FromHex(CodeMasterServce.GetValue<ColorMaster>(master.Code)) : Color.Transparent,
                    AttributeName = master.Value
                };
                if (!hasAttribute)
                {
                    attribute.Opacity = 0.5;
                }
                if (num < masters.Count / 2)
                    AttributeDisplay.Children.Add(attribute);
                else
                    AttributeDisplay2.Children.Add(attribute);
                num++;
            }
        }

        void Handle_Chu2Name(object sender, System.EventArgs e)
        {
            MainPage.AddSubPage(new Chu2NameListPage(TRANSITON_FROM.BOTTOM));
        }

        void Handle_Revival(object sender, System.EventArgs e)
        {
            MainPage.AddSubPage(new ShowPassword(TRANSITON_FROM.BOTTOM));
        }

        void Handle_Comment(object sender, System.EventArgs e)
        {
            MainPage.AddSubPage(new CommentEdit(TRANSITON_FROM.BOTTOM));
        }

        void Handle_History(object sender, System.EventArgs e)
        {
            var vm = BindingContext as MainViewModel;
            var str = string.Empty;
            foreach (var s in vm.User.History) str += s + Environment.NewLine;
            Explanation2.Show(this, str);
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Back(true);
        }
    }
}
