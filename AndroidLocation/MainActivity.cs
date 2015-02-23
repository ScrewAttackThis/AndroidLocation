using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Hardware;
using System.Collections.Generic;

namespace AndroidLocation
{
    [Activity(Label = "AndroidLocation", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener, ISensorEventListener
    {
        LocationManager locMgr;
        SensorManager sensorMgr;
        Sensor accelerometer;
        Sensor magnetometer;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Initialize location manager
            locMgr = GetSystemService(Context.LocationService) as LocationManager;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            sensorMgr = (SensorManager)GetSystemService(Context.SensorService);

            accelerometer = sensorMgr.GetDefaultSensor(SensorType.Accelerometer);
            magnetometer = sensorMgr.GetDefaultSensor(SensorType.MagneticField);
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


            sensorMgr.RegisterListener(this, accelerometer, SensorDelay.Ui);
            sensorMgr.RegisterListener(this, magnetometer, SensorDelay.Ui);

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

        List<float> gravity = new List<float>();
        List<float> magnet = new List<float>();
        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.MagneticField)
            {
                magnet.Clear();
                magnet.AddRange(e.Values);
            }
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                gravity.Clear();
                gravity.AddRange(e.Values);
            }

            if (magnet.Count > 0 && gravity.Count > 0)
            {
                float[] R = new float[9];
                float[] I = new float[9];
                bool worked = SensorManager.GetRotationMatrix(R, I, gravity.ToArray(), magnet.ToArray());

                if (worked)
                {
                    float[] orientation = new float[3];
                    SensorManager.GetOrientation(R, orientation);

                    ImageView arrowImageView = FindViewById<ImageView>(Resource.Id.arrowImageView);
                    float azimuth = orientation[0] * 180 / (float)Math.PI; //convert to degrees
                    arrowImageView.Rotation = azimuth;
                }
            }
        }
    }
}

