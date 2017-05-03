using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Graphics;

namespace XamarinAppClient.Droid
{
    [Activity(Label = "XamarinAppClient", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Activity// global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        int count = 1;
        Bitmap myBitmap;
        protected override void OnCreate(Bundle bundle)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Intent inc = new Intent(this, typeof(OverlayService));
            StartService(inc);
            Finish();
            //global::Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new App());
        }
    }
}

