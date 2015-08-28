using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using Android.Util;
using Android.Telephony.Gsm;
using System.Text;

namespace DroidMapping
{
	[Activity (Label = "Droid Mapping", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity , IOnMapReadyCallback,ILocationListener,GoogleMap.IOnMarkerClickListener
	{
		GoogleMap map;
		MapFragment mf;
		//Button _shareButton;
		Location _currentLocation;
		static readonly LatLng Location_NewYork = new LatLng(40.77, -73.98);
		Criteria criteria = new Criteria {
			Accuracy = Accuracy.Fine,
			PowerRequirement = Power.Medium,
		};
		LatLng coord;
		Marker currentLocationMarker;
		bool _markerClick = true;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			mf = FragmentManager.FindFragmentById (Resource.Id.map) as MapFragment;
		    mf.GetMapAsync(this);
			//_shareButton = FindViewById<Button> (Resource.Id.sharebutton);
			//_shareButton.Click += delegate {};

		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			map.MyLocationEnabled = true;
			map.MapType = GoogleMap.MapTypeNormal;
			map.UiSettings.ZoomControlsEnabled = true;

			//map.UiSettings.CompassEnabled = true;
			map.UiSettings.SetAllGesturesEnabled(true);
			//map.UiSettings.RotateGesturesEnabled = true;
			map.AddMarker(new MarkerOptions().SetPosition(Location_NewYork));
			map.SetOnMarkerClickListener (this);


			/*LocationManager locManager = GetSystemService (Context.LocationService) as LocationManager;
			if (locManager.IsProviderEnabled (LocationManager.GpsProvider)) {
				locManager.RequestLocationUpdates (1000, 10, criteria, this, null);
			}

			else 
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetTitle("GPS Disabled");
				builder.SetMessage("Enable GPS to use the app");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { StartActivityForResult (new Intent(Android.Provider.Settings.ActionLocationSourceSettings), 0);
				});
				builder.Show();
			}*/
			FindViewById<Button> (Resource.Id.sharebutton).Click += delegate {
				{
					var geoUri = Android.Net.Uri.Parse ("geo:"+ _currentLocation.Latitude+","+_currentLocation.Longitude);
					var mapIntent = new Intent (Intent.ActionView, geoUri);
					//mapIntent.SetType(
					//StartActivity (mapIntent);

					var smsUri = Android.Net.Uri.Parse("smsto:1234567890");
					var smsIntent = new Intent (Intent.ActionSendto, smsUri);
					StringBuilder smsstring = new StringBuilder();
					smsstring.Append("Click the link to track ");
					smsstring.Append("http://maps.google.com?q=");
					smsstring.Append(_currentLocation.Latitude);
					smsstring.Append(",");
					smsstring.Append(_currentLocation.Longitude);
					smsIntent.PutExtra ("sms_body",smsstring.ToString() );  
					StartActivity (smsIntent);
				}
			};

		} 



		public void OnLocationChanged(Location location)
		{
			
			_currentLocation = location;
			coord = new LatLng(location.Latitude, location.Longitude);
			CameraUpdate update = CameraUpdateFactory.NewLatLngZoom(coord, 17);

			map.AnimateCamera(update);

			if (currentLocationMarker != null) {
				currentLocationMarker.Position = coord;
			}
			else {
				var markerOptions = new MarkerOptions()
					.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue))
					.SetPosition(coord)
					.SetTitle("Current Position")
					.SetSnippet("You are here");
				currentLocationMarker = map.AddMarker(markerOptions);
			}
		}

		public void OnProviderDisabled(string provider)
		{
			
		}

		public void OnProviderEnabled(string provider)
		{
		}

		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{
		}

		protected override void OnResume()
		{
			base.OnResume ();

			LocationManager locManager = GetSystemService (Context.LocationService) as LocationManager;
			if (locManager.IsProviderEnabled (LocationManager.GpsProvider)) {
				locManager.RequestLocationUpdates (1000, 10, criteria, this, null);
			}

			else 
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetTitle("GPS Disabled");
				builder.SetMessage("Enable GPS to use the app");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { StartActivityForResult (new Intent(Android.Provider.Settings.ActionLocationSourceSettings), 0);
				});
				builder.Show();
			}
		}

		public bool OnMarkerClick (Marker marker)
		{
			if(marker.Equals(currentLocationMarker))
				{
				   


				if (_markerClick) {

					CameraPosition pos = new CameraPosition.Builder ().Target (coord).Zoom (17).Tilt (90).Build ();


					map.AnimateCamera (CameraUpdateFactory.NewCameraPosition (pos));
				} 
				else 
				{
					CameraPosition pos = new CameraPosition.Builder ().Target (coord).Zoom (17).Build ();


					map.AnimateCamera (CameraUpdateFactory.NewCameraPosition (pos));
					//CameraUpdate update = CameraUpdateFactory.NewLatLngZoom(coord, 17);

					//map.AnimateCamera(update);
				}

				}
			_markerClick = !_markerClick;
			return true;
		}

	}
}


