using System;
using System.Collections.Generic;

using Xamarin.Forms;
using FNO.ViewModels;

namespace FNO.Pages.SubPages
{
    public partial class CommentEdit : BaseSubPage
    {
        public CommentEdit(TRANSITON_FROM from) : base(from)
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var viewModel = BindingContext as MainViewModel;
            WinEntry.Text = viewModel.User.WinComment;
            LoseEntry.Text = viewModel.User.LoseComment;
            BattleEntry1.Text = viewModel.User.BattleComment[0];
            BattleEntry2.Text = viewModel.User.BattleComment[1];
            BattleEntry3.Text = viewModel.User.BattleComment[2];
        }

        private void CheckEntry()
        {
            if (!string.IsNullOrEmpty(WinEntry.Text) && !string.IsNullOrEmpty(LoseEntry.Text)
                && !string.IsNullOrEmpty(BattleEntry1.Text) && !string.IsNullOrEmpty(BattleEntry2.Text) && !string.IsNullOrEmpty(BattleEntry3.Text))
            {
                BackButton.IsVisible = true;
            }
            else
            {
                BackButton.IsVisible = false;
            }
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            CheckEntry();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var viewModel = BindingContext as MainViewModel;
            viewModel.User.WinComment = WinEntry.Text;
            viewModel.User.LoseComment = LoseEntry.Text;
            viewModel.User.BattleComment.Clear();
            viewModel.User.BattleComment.Add(BattleEntry1.Text);
            viewModel.User.BattleComment.Add(BattleEntry2.Text);
            viewModel.User.BattleComment.Add(BattleEntry3.Text);
            Back();
        }
    }
}
