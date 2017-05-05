﻿using System;
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

namespace XamarinAppClient.Droid
{
    [Service(Label = "OverlayService", Exported = true)]
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
            overlayedButton = new Button(this);
            overlayedButton.SetText("Overlay button", TextView.BufferType.Normal);
            overlayedButton.SetOnTouchListener(this);
            overlayedButton.SetTextColor(Color.Red);
            overlayedButton.Alpha = 0.0f;
            overlayedButton.SetBackgroundColor(Android.Graphics.Color.Green);
            overlayedButton.SetOnClickListener(this);
            var param = new WindowManagerLayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent,
                    WindowManagerTypes.SystemOverlay,
                    WindowManagerFlags.Fullscreen | WindowManagerFlags.WatchOutsideTouch | WindowManagerFlags.AllowLockWhileScreenOn | WindowManagerFlags.NotTouchable | WindowManagerFlags.NotFocusable,
                    Android.Graphics.Format.Translucent);
            param.Gravity = GravityFlags.Left | GravityFlags.Top;
            param.X = 0;
            param.Y = 0;
            wm.AddView(overlayedButton, param);



            var param2 = new WindowManagerLayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent,
                    WindowManagerTypes.SystemOverlay,
                    WindowManagerFlags.Fullscreen | WindowManagerFlags.WatchOutsideTouch | WindowManagerFlags.AllowLockWhileScreenOn | WindowManagerFlags.NotTouchable | WindowManagerFlags.NotFocusable,
                    Android.Graphics.Format.Translucent);

            param2.Gravity = GravityFlags.Left | GravityFlags.Top;
            param2.X = 0;
            param2.Y = 0;
            param2.Width = 0;
            param2.Height = 0;

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
                topLeftView.GetLocationOnScreen(topLeftLocationOnScreen);

                float x = e.RawX;
                float y = e.RawY;

                var param = (WindowManagerLayoutParams)overlayedButton.LayoutParameters;

                int newX = (int)(offsetX + x);
                int newY = (int)(offsetY + y);

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