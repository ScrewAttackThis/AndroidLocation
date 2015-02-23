using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;

namespace AndroidLocation
{
    [Activity(Label = "AndroidLocation", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener
    {
        LocationManager locMgr;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Initialize location manager
            locMgr = GetSystemService(Context.LocationService) as LocationManager;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it

        }

        protected override void OnResume()
        {
            base.OnResume();

            string provider = LocationManager.GpsProvider;

            if (locMgr.IsProviderEnabled(provider))
            {
                locMgr.RequestLocationUpdates(provider, 2000, 1, this);
            }
        }

        private void getLocation()
        {

        }

        public void OnLocationChanged(Location location)
        {
            throw new NotImplementedException();
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }
    }
}

