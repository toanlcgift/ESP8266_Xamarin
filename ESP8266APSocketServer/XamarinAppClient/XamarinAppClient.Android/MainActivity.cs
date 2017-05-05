using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Graphics;
using Plugin.Permissions;
using Android.Provider;

namespace XamarinAppClient.Droid
{
    [Activity(Label = "XamarinAppClient", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Activity// global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        int count = 1;
        Bitmap myBitmap;
        protected override async void OnCreate(Bundle bundle)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);
            Intent inc = new Intent(this, typeof(OverlayService));
            StartService(inc);
            Finish();
            //Toast.MakeText(Application.Context, "ahihi", ToastLength.Long).Show();
            //TestPermission();
            //var a = await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Overlay);
            //global::Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new App());
        }

        private const int RequestCode = 5469;

        private void TestPermission()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M) return;
            if (!Settings.CanDrawOverlays(this)) return;

            var intent = new Intent(Settings.ActionManageOverlayPermission);
            intent.SetPackage(PackageName);
            StartActivityForResult(intent, RequestCode);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == RequestCode)
            {
                if (Settings.CanDrawOverlays(this))
                {
                    
                    // we have permission
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

