using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using App1.Droid.BaiduSDKManager.Face;
using Java.IO;
using static Android.Hardware.Camera;

namespace App1.Droid.Activitys
{
    public class Preview : ViewGroup, ISurfaceHolderCallback
    {
        private string TAG = "Preview";

        SurfaceView mSurfaceView;
        ISurfaceHolder mHolder;
        Android.Hardware.Camera.Size mPreviewSize;
        List<Android.Hardware.Camera.Size> mSupportedPreviewSizes;
        Android.Hardware.Camera mCamera;
        bool mPreviewed = false;
        bool mSurfaceCreated = false;

        private int previewWidth;
        private int previewHeight;
        private bool mirrored = true;

        public Preview(Context context, SurfaceView sv) : base(context)
        {
            mSurfaceView = sv;
            //        addView(mSurfaceView);

            mHolder = mSurfaceView.Holder;
            mHolder.AddCallback(this);
            mHolder.SetType(SurfaceType.PushBuffers);
            //mSurfaceView.setZOrderOnTop(true);
            mSurfaceView.SetZOrderMediaOverlay(true);
            SetBackgroundColor(Color.Transparent);
        }

        public void setCamera(Android.Hardware.Camera camera, int width, int height)
        {
            mCamera = camera;
            if (mCamera != null)
            {
                mSupportedPreviewSizes = mCamera.GetParameters().SupportedPreviewSizes.ToList();
                RequestLayout();

                // get Camera parameters
                Parameters paramss = mCamera.GetParameters();
                paramss.SetPreviewSize(width, height);

                List<string> focusModes = paramss.SupportedFocusModes.ToList();
                if (focusModes.Contains(Parameters.FocusModeAuto))
                {
                    // set the focus mode
                    paramss.FocusMode = Parameters.FocusModeAuto;
                    // set Camera parameters

                }
                mCamera.SetParameters(paramss);
                if (!mPreviewed && mSurfaceCreated)
                {
                    try
                    {
                        mCamera.SetPreviewDisplay(mHolder);
                        mCamera.StartPreview();
                        mPreviewed = true;
                    }
                    catch (IOException e)
                    {
                        // TODO Auto-generated catch block
                        e.PrintStackTrace();
                    }
                }
            }

        }

        public void release()
        {
            mPreviewed = false;
            mCamera = null;
        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // We purposely disregard child measurements because act as a
            // wrapper to a SurfaceView that centers the camera preview instead
            // of stretching it.
            int width = ResolveSize(SuggestedMinimumWidth, widthMeasureSpec);
            int height = ResolveSize(SuggestedMinimumHeight, heightMeasureSpec);
            SetMeasuredDimension(width, height);

            if (mSupportedPreviewSizes != null)
            {
                mPreviewSize = getOptimalPreviewSize(mSupportedPreviewSizes, width, height);
            }
        }


        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            if (changed && ChildCount > 0)
            {
                View child = GetChildAt(0);

                int width = r - l;
                int height = b - t;

                previewWidth = width;
                previewHeight = height;
                if (mPreviewSize != null)
                {
                    previewWidth = mPreviewSize.Width;
                    previewHeight = mPreviewSize.Height;
                }

                // Center the child SurfaceView within the parent.
                if (width * previewHeight > height * previewWidth)
                {
                    int scaledChildWidth = previewWidth * height / previewHeight;
                    child.Layout((width - scaledChildWidth) / 2, 0,
                            (width + scaledChildWidth) / 2, height);
                }
                else
                {
                    int scaledChildHeight = previewHeight * width / previewWidth;
                    child.Layout(0, (height - scaledChildHeight) / 2,
                            width, (height + scaledChildHeight) / 2);
                }
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            Log.Info(TAG, "********surfaceCreated************");
            // The Surface has been created, acquire the camera and tell it where
            // to draw.
            try
            {
                if (mCamera != null && !mPreviewed)
                {
                    mHolder = holder;
                    mCamera.SetPreviewDisplay(holder);
                    mCamera.StartPreview();
                    mPreviewed = true;
                    mSurfaceCreated = true;
                }
            }
            catch (IOException exception)
            {
                Log.Error(TAG, "IOException caused by setPreviewDisplay()", exception);
            }
        }

        public ISurfaceHolder getSurfaceHolder()
        {
            return mHolder;
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            // Surface will be destroyed when we return, so stop the preview.
            if (mCamera != null)
            {
                // mCamera.stopPreview();
                mPreviewed = false;
            }
            mSurfaceCreated = false;
        }

        public bool isPreviewing()
        {
            return mPreviewed;
        }


        private Android.Hardware.Camera.Size getOptimalPreviewSize(List<Android.Hardware.Camera.Size> sizes, int w, int h)
        {
             double ASPECT_TOLERANCE = 0.1;
            double targetRatio = (double)w / h;
            if (sizes == null) return null;

            Android.Hardware.Camera.Size optimalSize = null;
            double minDiff = double.MaxValue;

            int targetHeight = h;

            // Try to find an size match aspect ratio and size
            foreach (Android.Hardware.Camera.Size size in sizes)
            {
                double ratio = (double)size.Width / size.Height;
                if (Math.Abs(ratio - targetRatio) > ASPECT_TOLERANCE)
                {
                    continue;
                }
                if (Math.Abs(size.Height - targetHeight) < minDiff)
                {
                    optimalSize = size;
                    minDiff = Math.Abs(size.Height - targetHeight);
                }
            }


            // Cannot find the one match the aspect ratio, ignore the requirement
            if (optimalSize == null)
            {
                minDiff = double.MaxValue;
                foreach (Android.Hardware.Camera.Size size in sizes)
                {
                    if (Math.Abs(size.Height - targetHeight) < minDiff)
                    {
                        optimalSize = size;
                        minDiff = Math.Abs(size.Height - targetHeight);
                    }
                }

            }
            return optimalSize;
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int w, int h)
        {
            Log.Debug(TAG, "==========surfaceChanged=================");
            //    	if(mCamera != null) {
            //    		Camera.Parameters parameters = mCamera.getParameters();
            //    		parameters.setPreviewSize(mPreviewSize.width, mPreviewSize.height);
            //    		requestLayout();
            //
            //    		mCamera.setParameters(parameters);
            //    		mCamera.startPreview();
            //    	}
        }

        public int getPreviewWidth()
        {
            return previewWidth;
        }
        public int getPreviewHeight()
        {
            return previewHeight;
        }


        public void mapFromOriginalRect(RectF rectF)
        {
            int selfWidth = Width;
            int selfHeight = Height;
            if (previewWidth == 0 || previewHeight == 0 || selfWidth == 0 || selfHeight == 0)
            {
                return;
                // TODO
            }

            Matrix matrix = new Matrix();

            ScaleType scaleType = resolveScaleType();
            if (scaleType == ScaleType.FIT_HEIGHT)
            {
                int targetWith = previewWidth * selfHeight / previewHeight;
                int delta = (targetWith - selfWidth) / 2;

                float ratio = 1.0f * selfHeight / previewHeight;

                matrix.PostScale(ratio, ratio);
                matrix.PostTranslate(-delta, 0);
            }
            else
            {
                int targetHeight = previewHeight * selfWidth / previewWidth;
                int delta = (targetHeight - selfHeight) / 2;

                float ratio = 1.0f * selfWidth / previewWidth;

                matrix.PostScale(ratio, ratio);
                matrix.PostTranslate(0, -delta);
            }
            matrix.MapRect(rectF);

            if (mirrored)
            {
                float left = selfWidth - rectF.Right;
                float right = left + rectF.Width();
                rectF.Left = left;
                rectF.Right = right;
            }
        }

        private ScaleType resolveScaleType()
        {
            float selfRatio = 1.0f * Width / Height;
            float targetRatio = 1.0f * previewWidth / previewHeight;

            ScaleType scaleType = this.scaleType;
            if (this.scaleType == ScaleType.CROP_INSIDE)
            {
                scaleType = selfRatio > targetRatio ? ScaleType.FIT_WIDTH : ScaleType.FIT_HEIGHT;
            }
            return scaleType;
        }

        private ScaleType scaleType = ScaleType.CROP_INSIDE;

    }

}