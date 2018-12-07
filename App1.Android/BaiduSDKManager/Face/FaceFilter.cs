using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Support.V4.Util;
using Android.Util;
using App1.Droid.BaiduSDKManager.Face;
using Com.Baidu.Idl.Facesdk;

namespace App1.Droid.BaiduSDKManager.Face
{
    /**
 * 过虑器，可用根据条件过虑帧。
 */
    public class FaceFilter
    {
        private OnTrackListener onTrackListener;

        private SparseArray<TrackedModel> trackingFaces = new SparseArray<TrackedModel>();
        private SynchronizedPool<TrackedModel> pool = new SynchronizedPool<TrackedModel>(3);

        private HashSet<TrackedModel> currentFrame = new HashSet<TrackedModel>();
        private List<int?> leftFaces = new List<int?>();

        private static int angle = 15;

        /**
         * 人脸追踪回调。
         */
        public interface OnTrackListener
        {
            /**
             * 追踪到某张人脸
             *
             * @param trackedModel 人脸信息
             */
            void onTrack(TrackedModel trackedModel);
        }

        /**
         * 人脸追踪事件。
         */
        public enum Event
        {
            /**
             * 人脸第一次进入检测区域。
             */
            OnEnter,
            /**
             * 人脸没有离开检测区域。人脸检测更新。
             */
            OnUpdate,
            /**
             * 该人脸离开了检测区域，或者丢失了跟踪。
             */
            OnLeave,
        }

        public class TrackedModel
        {
            private string trackId;
            private ImageFrame frame;
            private FaceInfo info;
            private Event eevent;

            public ImageFrame getImageFrame()
            {
                return getFrame();
            }

            /**
             * 对应的事件
             */
            public Event getEvent()
            {
                return eevent;
            }

            /**
             * 是否符合过虑标准
             *
             * @return 符合过虑标准
             */
            public bool meetCriteria()
            {
                float pitch = Math.Abs(getInfo().HeadPose[0]);
                float yaw = Math.Abs(getInfo().HeadPose[1]);
                float roll = Math.Abs(getInfo().HeadPose[2]);
                return pitch < angle && yaw < angle && roll < angle;
            }

            public Bitmap cropFace()
            {
                return cropFace(getFaceRect());
            }

            /**
             * 裁剪人脸图片。
             *
             * @param rect 裁剪区域。如果区域超出人脸，区域会被调整。
             *
             * @return 裁剪后的人脸图片。
             */

            // TODO
            public Bitmap cropFace(Rect rect)
            {
                return FaceCropper.getFace(getFrame().getArgb(), info, getImageFrame().getWidth());
                //            int[] argb = FaceCropper.crop(getFrame().getArgb(), getFrame().getWidth(), rect);
                //            return Bitmap.createBitmap(argb, rect.width(), rect.height(), Bitmap.Config.ARGB_8888);
            }

            public int[] cropFaceArgb()
            {
                return FaceCropper.getCropFace(getFrame().getArgb(), info, getImageFrame().getWidth());
            }

            public int hashCode()
            {
                return getInfo().FaceId;
            }

            int[] points = new int[8];

            /**
             * 获取人脸框区域。
             *
             * @return 人脸框区域
             */
            // TODO padding?
            public Rect getFaceRect()
            {
                Rect rect = new Rect();
                getInfo().GetRectPoints(points);

                int left = points[2];
                int top = points[3];
                int right = points[6];
                int bottom = points[7];
                //
                //            int width = (right - left) * 4 / 3;
                //            int height = (bottom - top) * 4 / 3;
                //
                //            left = getInfo().mCenter_x - width / 2;
                //            top = getInfo().mCenter_y - height / 2;
                //
                //            rect.top = top;
                //            rect.left = left;
                //            rect.right = left + width;
                //            rect.bottom = top + height;

                //            int width = (right - left) * 4 / 3;
                //            int height = (bottom - top) * 5 / 3;
                int width = (right - left);
                int height = (bottom - top);

                //            left = getInfo().mCenter_x - width / 2;
                //            top = getInfo().mCenter_y - height * 2 / 3;
                left = (int)(getInfo().MCenterX - width / 2);
                top = (int)(getInfo().MCenterY - height / 2);


                rect.Top = top < 0 ? 0 : top;
                rect.Left = left < 0 ? 0 : left;
                rect.Right = (left + width) > frame.getWidth() ? frame.getWidth() : (left + width);
                rect.Bottom = (top + height) > frame.getHeight() ? frame.getHeight() : (top + height);

                return rect;
            }

            /**
             * 标识一张人脸的追踪id。
             */
            public string getTrackId()
            {
                return trackId;
            }

            public void setTrackId(string trackId)
            {
                this.trackId = trackId;
            }

            /**
             * 对应的帧
             */
            public ImageFrame getFrame()
            {
                return frame;
            }

            public void setFrame(ImageFrame frame)
            {
                this.frame = frame;
            }

            /**
             * 人脸检测数据
             */
            public FaceInfo getInfo()
            {
                return info;
            }

            public void setInfo(FaceInfo info)
            {
                this.info = info;
            }

            public void setEvent(Event eevent) {

                this.eevent = eevent;
            }
        }

        /**
         * 设置过虑角度。参见人脸欧拉角;
         *
         * @param angle 欧拉角
         */
        public void setAngle(int angle)
        {
            FaceFilter.angle = angle;
        }

        /**
         * 设置跟踪监听器
         *
         * @param onTrackListener 跟踪监听器
         */
        public void setOnTrackListener(OnTrackListener onTrackListener)
        {
            this.onTrackListener = onTrackListener;
        }

        public void filter(FaceInfo[] infos, ImageFrame frame)
        {
            currentFrame.Clear();
            if (infos != null)
            {
                foreach (FaceInfo faceInfo in infos)
                {
                    TrackedModel face = getTrackedModel(faceInfo, frame);
                    currentFrame.Add(face);
                    face.setInfo(faceInfo);
                }

            }

            leftFaces.Clear();
            for (int i = 0; i < trackingFaces.Size(); i++)
            {
                int key = trackingFaces.KeyAt(i);
                TrackedModel face = trackingFaces.Get(key);
                if (!currentFrame.Contains(face))
                {
                    leftFaces.Add(key);
                }
                else
                {
                    if (onTrackListener != null)
                    {
                        face.setFrame(frame);
                        onTrackListener.onTrack(face);
                    }
                }

            }
            foreach (int? key in leftFaces)
            {
                TrackedModel left = trackingFaces.Get(key.Value);
                Log.Info("xx", " left:" + left);
                left.setEvent(Event.OnLeave);
                trackingFaces.Remove(key.Value);
                if (onTrackListener != null)
                {
                    onTrackListener.onTrack(left);
                }
                // TODO release argb?
            }
        }

        private TrackedModel getTrackedModel(FaceInfo faceInfo, ImageFrame frame)
        {
            TrackedModel face = trackingFaces.Get(faceInfo.FaceId);
            if (face == null)
            {
                face = pool.Take();
                if (face == null)
                {
                    face = new TrackedModel();
                }
                trackingFaces.Append(faceInfo.FaceId, face);
                face.setTrackId(Guid.NewGuid().ToString());
                face.setEvent(Event.OnEnter);
            }
            face.setInfo(faceInfo);
            face.setFrame(frame);
            return face;
        }
    }
}