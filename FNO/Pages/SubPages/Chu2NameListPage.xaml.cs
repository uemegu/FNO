using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FNO.Controls;
using FNO.Models;
using FNO.ViewModels;

namespace FNO.Pages.SubPages
{
    public partial class Chu2NameListPage : BaseSubPage
    {
        private bool _hasChange = false;

        public Chu2NameListPage(TRANSITON_FROM from) : base(from)
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var viewModel = BindingContext as MainViewModel;
            if (viewModel == null) return;

            List.BindingContext = viewModel.User;
            List.NameSelected += (object sender, EventArgs e) =>
            {
                Sound.OK();
                SelectItem.Show(this, List.SelectedName.Chu2Name + List.SelectedName.Job + "に入れ替えますか？",
               new List<SelectItem.Item>()
               {
                    new SelectItem.Item()
                    {
                        Label = "メイン二つ名にする",
                        Selected = () => {
                            _hasChange = true;
                            if(List.SelectedName == viewModel.User.SubName) {
                                SwapName();
                            } else {
                                viewModel.User.MainName = List.SelectedName;
                            }
                        }
                    },
                    new SelectItem.Item()
                    {
                        Label = "サブ二つ名にする" +
                            "",
                        Selected = () => {
                            _hasChange = true;
                            if(List.SelectedName == viewModel.User.MainName) {
                                SwapName();
                            } else {
                                viewModel.User.SubName = List.SelectedName;
                            }
                        }
                    },
                    new SelectItem.Item()
                    {
                        Label = "キャンセル"
                    }
               });
            };
        }

        private void SwapName()
        {
            var viewModel = BindingContext as MainViewModel;
            if (viewModel == null) return;

            SelectItem.Show(this, "メイン二つ名とサブ二つ名が同じになります。メインとサブを入れ替えますか？",
            new List<SelectItem.Item>()
            {
                    new SelectItem.Item()
                    {
                        Label = "入れ替える",
                        Selected = () => {
                            var tmp = viewModel.User.SubName;
                            viewModel.User.SubName = viewModel.User.MainName;
                            viewModel.User.MainName = tmp;
                        }
                    },
                    new SelectItem.Item()
                    {
                        Label = "キャンセル"
                    }
            });
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Back(!_hasChange);
        }
    }
}
