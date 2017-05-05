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
using Plugin.Permissions;
using static Android.Manifest;
using Android.Support.V4.App;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using System.Net.Http;

namespace XamarinAppClient.Droid
{
    [Service(Label = "OverlayService", Exported = true)]
    public class OverlayService : Service, IOnTouchListener, IOnClickListener
    {
        bool LightFlag = true;
        bool Doing = false;
        HttpClient hc = new HttpClient();
        private ImageButton overlayedButton;
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
            hc.BaseAddress = new Uri("http://192.168.1.69");
            wm = this.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            overlayedButton = new ImageButton(this);
            overlayedButton.SetImageResource(Resource.Drawable.Light);
            overlayedButton.SetBackgroundColor(Color.Transparent);
            overlayedButton.SetColorFilter(Color.Black);
            overlayedButton.SetOnTouchListener(this);
            overlayedButton.SetOnClickListener(this);
            var param = new WindowManagerLayoutParams(
                    WindowManagerLayoutParams.WrapContent,
                    WindowManagerLayoutParams.WrapContent,
                    //WindowManagerTypes.SystemOverlay,
                    WindowManagerTypes.SystemAlert,
                    WindowManagerFlags.Fullscreen | WindowManagerFlags.WatchOutsideTouch | WindowManagerFlags.AllowLockWhileScreenOn | WindowManagerFlags.NotTouchModal | WindowManagerFlags.LayoutInScreen,
                    Android.Graphics.Format.Transparent);
            param.Gravity = GravityFlags.Left | GravityFlags.Top;
            param.X = 150;
            param.Y = 300;
            wm.AddView(overlayedButton, param);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (overlayedButton != null)
            {
                wm.RemoveView(overlayedButton);
                overlayedButton = null;
            }
        }

        public async void OnClick(View v)
        {
            if (!Doing)
            {
                Doing = true;
                if (LightFlag)
                {
                    try
                    {
                        overlayedButton.SetColorFilter(Color.Yellow);
                        hc.GetAsync("/1");
                        await Task.Delay(200);
                        hc.CancelPendingRequests();
                    }
                    catch { }
                    
                }
                else
                {
                    try
                    {
                        overlayedButton.SetColorFilter(Color.Black);
                        hc.GetAsync("/0");
                        await Task.Delay(200);
                        hc.CancelPendingRequests();
                    }
                    catch { }
                    
                }
                LightFlag = !LightFlag;
                Doing = false;
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.PointerDown)
            {
                float x = e.RawX;
                float y = e.RawY;

                moving = false;

                int[] location = new int[2];
                overlayedButton.GetLocationOnScreen(location);

                originalXPos = location[0];
                originalYPos = location[1];

                offsetX = originalXPos - x;
                offsetY = originalYPos - y;

            }
            else if (e.Action == MotionEventActions.Move)
            {
                int[] topLeftLocationOnScreen = new int[2];

                float x = e.RawX;
                float y = e.RawY;

                var param = (WindowManagerLayoutParams)overlayedButton.LayoutParameters;

                int newX = (int)(offsetX + x - overlayedButton.Width / 2);
                int newY = (int)(offsetY + y - overlayedButton.Height / 2);

                if (Math.Abs(newX - originalXPos) < 1 && Math.Abs(newY - originalYPos) < 1 && !moving)
                {
                    return false;
                }

                param.X = newX - (topLeftLocationOnScreen[0]);
                param.Y = newY - (topLeftLocationOnScreen[1]);

                wm.UpdateViewLayout(overlayedButton, param);
                moving = true;
            }
            else if (e.Action == MotionEventActions.PointerUp)
            {
                if (moving)
                {
                    return true;
                }
            }

            return false;
        }
    }
}