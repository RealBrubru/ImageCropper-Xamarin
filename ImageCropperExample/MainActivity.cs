using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Media;
using Com.Ticktick.Imagecropper;
using System.IO;
using Android.Provider;

namespace ImageCropperExample
{
	[Activity (Label = "ImageCropperExample", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		const int REQUEST_CODE_PICK_IMAGE = 10;
		const int REQUEST_CODE_IMAGE_CROPPER  = 11;

		MediaPicker _mediaPicker;
		ImageView _croppedImageView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Set the Xamarin.Mobile media pricker
			_mediaPicker = new MediaPicker(this);

			// Get the image view to set the cropped image
			_croppedImageView = FindViewById<ImageView> (Resource.Id.croppedImageView);

			// Get our button from the layout resource, and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += delegate {
				var intent = _mediaPicker.GetPickPhotoUI();

				StartActivityForResult (intent, REQUEST_CODE_PICK_IMAGE);
			};
		}

		protected override async void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			// User canceled
			if (resultCode == Result.Canceled) {
				return;
			}

			if (requestCode == REQUEST_CODE_PICK_IMAGE) {
				
				MediaFile file = await data.GetMediaFileExtraAsync (this);

				Console.WriteLine ("========== image to crop: {0}", file.Path);

				Intent intent = new Intent(this, typeof(CropImageActivity));
				intent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(file.Path)));
				intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(new Java.IO.File(getCroppedFileName (file.Path))));
				StartActivityForResult (intent, REQUEST_CODE_IMAGE_CROPPER);

			} else if (requestCode == REQUEST_CODE_IMAGE_CROPPER) {
				
				var croppedUri = (Android.Net.Uri)data.GetParcelableExtra (MediaStore.ExtraOutput);

				Console.WriteLine ("========== image cropped: {0}", croppedUri.Path);

				var croppedBitmap = croppedUri.Path.LoadAndResizeBitmap (_croppedImageView.Width, _croppedImageView.Height);

				_croppedImageView.SetImageBitmap (croppedBitmap);

			}

			base.OnActivityResult (requestCode, resultCode, data);
		}

		private string getCroppedFileName(string path)
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			string newFileName = path.Replace(fileName, fileName + "-cropped");

			return newFileName;
		}
	}
}