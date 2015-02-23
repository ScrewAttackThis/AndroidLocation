using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Hardware;

namespace AndroidLocation
{
    [Activity(Label = "AndroidLocation", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener, ISensorEventListener
    {
        LocationManager locMgr;
        SensorManager sensorMgr;
        Sensor orientationSensor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Initialize location manager
            locMgr = GetSystemService(Context.LocationService) as LocationManager;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            sensorMgr = (SensorManager)GetSystemService(Context.SensorService);

        }

        protected override void OnResume()
        {
            base.OnResume();

            TextView locationStatusTextView = FindViewById<TextView>(Resource.Id.locationStatusTextView);

            Criteria locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;


            string provider = locMgr.GetBestProvider(locationCriteria,true);

            if (locMgr.IsProviderEnabled(provider))
            {
                locMgr.RequestLocationUpdates(provider, 2000, 1, this);
                locationStatusTextView.Text = provider;
            }
            else
            {
                locationStatusTextView.Text = "No providers available.";
            }


            orientationSensor = sensorMgr.GetDefaultSensor(SensorType.Orientation);
            sensorMgr.RegisterListener(this, orientationSensor, SensorDelay.Ui);

        }

        protected override void OnPause()
        {
            base.OnPause();

            locMgr.RemoveUpdates(this);
        }

        protected override void OnStop()
        {
            sensorMgr.UnregisterListener(this);
            base.OnStop();
        }

        public void OnLocationChanged(Location location)
        {
            TextView currentLocationTextView = FindViewById<TextView>(Resource.Id.currentLocationTextView);
            TextView locationStatusTextView = FindViewById<TextView>(Resource.Id.locationStatusTextView);

            currentLocationTextView.Text = "Lat: " + location.Latitude.ToString() + " Long: " + location.Longitude.ToString();
            locationStatusTextView.Text = location.Provider;
        }

        public void OnProviderDisabled(string provider)
        {
            TextView locationStatusTextView = FindViewById<TextView>(Resource.Id.locationStatusTextView);
            locationStatusTextView.Text = "Provider is disabled.";
        }

        public void OnProviderEnabled(string provider)
        {
            TextView locationStatusTextView = FindViewById<TextView>(Resource.Id.locationStatusTextView);
            locationStatusTextView.Text = "Provider is enabled.";
            locMgr.RequestLocationUpdates(provider, 2000, 1, this);
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            TextView locationStatusTextView = FindViewById<TextView>(Resource.Id.locationStatusTextView);
            locationStatusTextView.Text = status.ToString();
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(SensorEvent e)
        {
            ImageView arrowImageView = FindViewById<ImageView>(Resource.Id.arrowImageView);
            arrowImageView.Rotation = e.Values[0];
        }
    }
}

