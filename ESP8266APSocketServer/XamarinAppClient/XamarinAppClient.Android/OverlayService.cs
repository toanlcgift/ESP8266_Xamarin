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
using Box2D.Dynamics;
using Box2D.Dynamics.Contacts;
using Box2D.Collision;
using Box2D.Common;
using Box2D.Collision.Shapes;
using System.Threading;

namespace XamarinAppClient.Droid
{
    [Service(Label = "OverlayService", Exported = true)]
    public class OverlayService : Service, IOnTouchListener, IOnClickListener, IOnLongClickListener
    {

        public class Myb2Listener : b2ContactListener
        {

            public override void PreSolve(b2Contact contact, b2Manifold oldManifold)
            {
            }

            public override void PostSolve(b2Contact contact, ref b2ContactImpulse impulse)
            {
            }

        }

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
        private RelativeLayout mOverlay;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private Box2D.Dynamics.b2World world;
        b2BodyDef bodydef;
        b2BodyDef def;

        public override void OnCreate()
        {
            base.OnCreate();
            InitB2World();
            hc.BaseAddress = new Uri("http://192.168.1.69");
            wm = this.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            overlayedButton = new ImageButton(this);
            overlayedButton.SetImageResource(Resource.Drawable.Snow);
            overlayedButton.SetBackgroundColor(Color.Transparent);
            overlayedButton.SetColorFilter(Color.White);

            overlayedButton.ScaleX = 0.7f;
            overlayedButton.ScaleY = 0.7f;
            overlayedButton.SetOnTouchListener(this);
            overlayedButton.SetOnClickListener(this);
            overlayedButton.SetOnLongClickListener(this);
            var param = new WindowManagerLayoutParams(
                    WindowManagerLayoutParams.WrapContent,
                    WindowManagerLayoutParams.WrapContent,
                    //WindowManagerTypes.SystemOverlay,
                    WindowManagerTypes.SystemAlert,
                        WindowManagerFlags.Fullscreen |
                        WindowManagerFlags.WatchOutsideTouch |
                        WindowManagerFlags.AllowLockWhileScreenOn |
                        WindowManagerFlags.NotTouchModal |
                        WindowManagerFlags.LayoutInScreen |
                        WindowManagerFlags.NotFocusable |
                        WindowManagerFlags.AltFocusableIm,
                    Android.Graphics.Format.Transparent);
            param.Gravity = GravityFlags.Left | GravityFlags.Top;
            param.X = 100;
            param.Y = 150;
            wm.AddView(overlayedButton, param);
            System.Timers.Timer timer = new System.Timers.Timer(5);

            timer.Elapsed += (o, e) =>
            {
                Rotate();
            };
            timer.Start();
            //Task.Run(() =>
            //{
            //    while (true)
            //        Update();
            //});
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

        private void Rotate()
        {
            if (!LightFlag)
                Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.RunOnUiThread(() =>
                {
                    if (++overlayedButton.Rotation == 360)
                    {
                        overlayedButton.Rotation = 0;
                    }
                });
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
                        //overlayedButton.SetColorFilter(Color.Yellow);
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
                        //overlayedButton.SetColorFilter(Color.Black);
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

        public bool OnLongClick(View v)
        {
            this.OnDestroy();
            return true;
        }

        private void InitB2World()
        {
            world = new Box2D.Dynamics.b2World(new Box2D.Common.b2Vec2(0, -100));
            world.SetContactListener(new Myb2Listener());
            world.SetAllowSleeping(true);
            world.SetContinuousPhysics(true);
            // Call the body factory which allocates memory for the ground body
            // from a pool and creates the ground box shape (also from a pool).
            // The body is also added to the world.
            def = new b2BodyDef();
            def.allowSleep = true;
            def.position = b2Vec2.Zero;
            def.type = b2BodyType.b2_staticBody;
            b2Body groundBody = world.CreateBody(def);
            groundBody.SetActive(true);

            // Define the ground box shape.

            // bottom
            b2EdgeShape groundBox = new b2EdgeShape();
            groundBox.Set(b2Vec2.Zero, new b2Vec2(Resources.DisplayMetrics.WidthPixels, 0));
            b2FixtureDef fd = new b2FixtureDef();
            fd.shape = groundBox;
            groundBody.CreateFixture(fd);

            // top
            groundBox = new b2EdgeShape();
            groundBox.Set(new b2Vec2(0, Resources.DisplayMetrics.HeightPixels), new b2Vec2(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels));
            fd.shape = groundBox;
            groundBody.CreateFixture(fd);

            // left
            groundBox = new b2EdgeShape();
            groundBox.Set(new b2Vec2(0, Resources.DisplayMetrics.HeightPixels), b2Vec2.Zero);
            fd.shape = groundBox;
            groundBody.CreateFixture(fd);

            // right
            groundBox = new b2EdgeShape();
            groundBox.Set(new b2Vec2(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels), new b2Vec2(Resources.DisplayMetrics.WidthPixels, 0));
            fd.shape = groundBox;
            groundBody.CreateFixture(fd);

            //
            bodydef = new b2BodyDef();
            bodydef.active = true;
            bodydef.position = new b2Vec2(100, 100);
            bodydef.type = b2BodyType.b2_dynamicBody;
            b2Body buttonBody = world.CreateBody(bodydef);
            buttonBody.SetActive(true);

            var shape = new b2EdgeShape();
            shape.Set(new b2Vec2(100, 100), new b2Vec2(150, 100));
            buttonBody.CreateFixture(new b2FixtureDef()
            {
                density = 10,
                shape = shape,

            });
        }

        private void Update()
        {
            world.Step((float)1 / 60, 8, 1);
            var param = (WindowManagerLayoutParams)overlayedButton.LayoutParameters;
            param.X = (int)bodydef.position.x;
            param.Y = (int)bodydef.position.y;
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.RunOnUiThread(() =>
            {
                //System.Diagnostics.Debug.WriteLine("paramX = " + param.X + "  parramY = " + param.Y);
                wm.UpdateViewLayout(overlayedButton, param);
            });
            world.Dump();
        }


    }
}