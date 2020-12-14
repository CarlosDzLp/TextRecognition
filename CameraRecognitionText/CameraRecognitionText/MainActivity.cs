using System;
using System.Text;
using Android;
using Android.App;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;

namespace CameraRecognitionText
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISurfaceHolderCallback, Detector.IProcessor,Java.Lang.IRunnable
    {
        SurfaceView mCameraView;
        TextView mTextView;
        TextView mTextViewLanguage;
        CameraSource mCameraSource;
        Button btn;
        private static int requestPermissionID = 101;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            mCameraView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            mTextView = FindViewById<TextView>(Resource.Id.text_view);
            mTextViewLanguage = FindViewById<TextView>(Resource.Id.text_viewlanguaje);
            btn = FindViewById<Button>(Resource.Id.btn);
            btn.Click += Btn_Click;           
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn.Text == "Iniciar")
                {
                    btn.Text = "Detener";
                    startCameraSource();
                }
                else
                {
                    btn.Text = "Iniciar";
                    mCameraSource.Stop();
                    textRecognizer = null;
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                {

                    ActivityCompat.RequestPermissions(this,
                            new String[] { Manifest.Permission.Camera },
                            requestPermissionID);
                    return;
                }                
                mCameraSource.Start(mCameraView.Holder);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {

        }

        SparseArray items;
        public void ReceiveDetections(Detector.Detections detections)
        {
            try
            {
                items = detections.DetectedItems;
                if (items.Size() != 0)
                {
                    mTextView.Post(this);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void Run()
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                string language = "";
                for (int i = 0; i < items.Size(); i++)
                {
                    var item = items.ValueAt(i) as TextBlock;
                    language = item.Language;
                    stringBuilder.Append(item.Value);
                    stringBuilder.Append("\n");
                }
                mTextView.Text = stringBuilder.ToString();
                mTextViewLanguage.Text = language;
            }
            catch(Exception ex)
            {

            }
        }

        public void Release()
        {
            //NO SE  HACE  NADA
        }

        TextRecognizer textRecognizer;
        private void startCameraSource()
        {
            try
            {
                textRecognizer = new TextRecognizer.Builder(this).Build();
                if (!textRecognizer.IsOperational)
                {
                    System.Diagnostics.Debug.WriteLine("Detector dependencies not loaded yet");
                }
                else
                {
                    mCameraSource = new CameraSource.Builder(this, textRecognizer)
                        .SetFacing(CameraFacing.Back)
                        .SetRequestedPreviewSize(1280, 1024)
                        .SetAutoFocusEnabled(true)
                        .SetRequestedFps(2.0f)
                        .Build();
                    mCameraView.Holder.AddCallback(this);
                    textRecognizer.SetProcessor(this);
                    if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Android.Content.PM.Permission.Granted)
                    {                      
                        mCameraSource.Start(mCameraView.Holder);
                    }
                    else
                    {
                        ActivityCompat.RequestPermissions(this,
                                new String[] { Manifest.Permission.Camera },
                                requestPermissionID);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);


            if(grantResults[0] == Android.Content.PM.Permission.Granted)
            {
                try
                {
                    if(ActivityCompat.CheckSelfPermission(this,Manifest.Permission.Camera)== Android.Content.PM.Permission.Granted)
                    {
                        mCameraSource.Start(mCameraView.Holder);
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}