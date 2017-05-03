using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Views.View;
using Android.Content;
using Android.Graphics;

namespace XamarinAppClient.Droid
{
    public class OverlayService : Service, IOnTouchListener, IOnClickListener
    {

        private View topLeftView;

        private Button overlayedButton;
        private float offsetX;
        private float offsetY;
        private int originalXPos;
        private int originalYPos;
        private bool moving;
        private IWindowManager wm;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            
            wm = this.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            ImageView overlayImage = new ImageView(this);
            overlayImage.SetImageResource(Resource.Drawable.ic_media_play);
            overlayedButton = new Button(this);
            overlayedButton.SetText("Overlay button",TextView.BufferType.Normal);
            overlayedButton.SetOnTouchListener(this);
            overlayedButton.SetBackgroundColor(Android.Graphics.Color.Green);
            overlayedButton.SetOnClickListener(this);
            var param = new WindowManagerLayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent,
                    WindowManagerTypes.SystemOverlay,
                    WindowManagerFlags.Fullscreen | WindowManagerFlags.WatchOutsideTouch | WindowManagerFlags.AllowLockWhileScreenOn | WindowManagerFlags.NotTouchable | WindowManagerFlags.NotFocusable,
                    Android.Graphics.Format.Translucent);
            overlayImage.SetScaleType(ImageView.ScaleType.FitXy);
            param.Gravity = GravityFlags.Top;

            wm.AddView(overlayedButton, param);
            wm.AddView(overlayImage, param);
            var param2 = new WindowManagerLayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent,
                    WindowManagerTypes.SystemOverlay,
                    WindowManagerFlags.Fullscreen | WindowManagerFlags.WatchOutsideTouch | WindowManagerFlags.AllowLockWhileScreenOn | WindowManagerFlags.NotTouchable | WindowManagerFlags.NotFocusable,
                    Android.Graphics.Format.Translucent);

            param2.Gravity = GravityFlags.Left;


            topLeftView = new View(this);
            wm.AddView(topLeftView, param2);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (overlayedButton != null)
            {
                wm.RemoveView(overlayedButton);
                wm.RemoveView(topLeftView);
                overlayedButton = null;
                topLeftView = null;
            }
        }

        public void OnClick(View v)
        {
            var toast = Toast.MakeText(Application.Context, "ahihi", ToastLength.Long);
            toast.Show();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}