using System;
using System.Linq;
using CoreGraphics;
using UIKit;

//copy from https://itblogdsi.blog.fc2.com/blog-entry-224.html
namespace FNO.iOS
{
    public class Toast
    {
        //トーストビュー本体
        private UIView _view;
        // 文字列を表示するためのラベル
        private UILabel _label;
        //トーストのサイズ（可変）
        private nfloat _margin = 40;
        private nfloat _height = 40;
        private nfloat _width = 0;

        //Cancelフラグ
        private bool _isCancel = false;

        public Toast()
        {
            if (_view == null)
            {
                _view = new UIView(new CGRect(0, 0, 0, 0))
                {
                    BackgroundColor = UIColor.Black,
                };
            }

            _view.Layer.CornerRadius = (nfloat)20.0;

            _label = new UILabel(new CGRect(0, 0, 0, 0))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White
            };
            _view.AddSubview(_label);

        }

        public async void Show(UIView parent, string message)
        {
            _label.Text = message;

            CGSize maxSize = new CGSize(parent.Bounds.Width, _height);
            CGSize fitSize = _label.SizeThatFits(maxSize);
            _width = fitSize.Width;
            if (parent.Bounds.Width >= _width + 30)
            {
                _width += 30;
            }

            _view.Alpha = (nfloat)0;

            _view.Frame = new CGRect(
                                (parent.Bounds.Width - _width) / 2,
                                 parent.Bounds.Height - _height - _margin,
                                _width,
                                _height);

            _label.Frame = new CGRect(0, 0, _width, _height);

            parent.AddSubview(_view);

            while (true)
            {
                _view.Alpha += (nfloat)0.05;
                await System.Threading.Tasks.Task.Delay(50);
                if (_isCancel ||
                    _view.Alpha >= 0.8)
                {
                    break;
                }
            }

            await System.Threading.Tasks.Task.Delay(2000);

            while (true)
            {
                _view.Alpha -= (nfloat)0.05;
                await System.Threading.Tasks.Task.Delay(50);
                if (_isCancel ||
                    _view.Alpha <= 0)
                {
                    break;
                }
            }

            _view.RemoveFromSuperview();
        }

        public bool IsShowing(UIView parent)
        {
            if (parent == null ||
                parent.Subviews == null)
            {
                return false;
            }

            if (parent.Subviews.Contains(_view))
            {
                return true;
            }
            return false;
        }

        public void Cancel()
        {
            if (_view != null)
            {
                _isCancel = true;
                _view.RemoveFromSuperview();
            }
        }
    }
}
