using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;



namespace KinectTest_WinFormApp
{
    class KinectProc
    {


        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }

        public KinectProc() {



            byte[] cColor_Red = { 0x00, 0x00, 0xFF };
            byte[] cColor_Green = { 0x00, 0xFF, 0x00 };
            byte[] cColor_Blue = { 0xFF, 0x00, 0x00 };
            byte[] cColor_Yellow = { 0x00, 0xFF, 0xFF };
            byte[] cColor_Violet = { 0xFF, 0x00, 0xFF };
            byte[] cColor_Sky = { 0xFF, 0xFF, 0x00 };
            byte[] cColor_YellowGreen = { 0x00, 0xFF, 0xC8 };

            playerColor[0] = Color.Red;
            playerColor[1] = Color.Green;
            playerColor[2] = Color.Blue;
            playerColor[3] = Color.Yellow;
            playerColor[4] = Color.Violet;
            playerColor[5] = Color.SkyBlue;
            playerColor[6] = Color.YellowGreen;
            
        }


        public static Bitmap ColorImageFrameToBitmap(ref ColorImageFrame frame, PixelFormat pixelFormal)
        {

            if (frame == null) return null;

            byte[] ImageBits = new byte[frame.PixelDataLength];
            frame.CopyPixelDataTo(ImageBits);
            
            Bitmap bmp = new Bitmap(frame.Width, frame.Height, pixelFormal);

            //using (bmp)
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                Marshal.Copy(ImageBits, 0, bmpData.Scan0, ImageBits.Length); bmp.UnlockBits(bmpData);
            }
            return bmp;
        }


        public static int GetDistance(int left, int top, ref DepthImageFrame frame) {
            short[] ImageBits = new short[frame.PixelDataLength];
            frame.CopyPixelDataTo(ImageBits);

            int dist = ImageBits[frame.Width * top + left] >> DepthImageFrame.PlayerIndexBitmaskWidth;
            return dist;
        }


        public static Bitmap DepthToGrayBitmap(ref DepthImageFrame frame)
        {
            short[] ImageBits = new short[frame.PixelDataLength];
            frame.CopyPixelDataTo(ImageBits);

            byte[] pixels = new byte[frame.Width * frame.Height * 4];
            
            int i32, i16;

            for (i16=0, i32 = 0; i16 < frame.PixelDataLength && i32 < pixels.Length  ; i16++, i32+=4) {

                int dist = ImageBits[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                byte grayscale = Get8bitGrayScale(dist);
                pixels[i32 + 0] = grayscale;
                pixels[i32 + 1] = grayscale;
                pixels[i32 + 2] = grayscale;
                //pixels[i32 + 3] = Get8bitGrayScale(dist);

            }


            Bitmap bmp = new Bitmap(frame.Width, frame.Height, PixelFormat.Format32bppRgb);

            //using (bmp)
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length); bmp.UnlockBits(bmpData);
            }

            return bmp;
        }

        public static byte[] DepthToGrayByte(ref DepthImageFrame frame) {
            short[] ImageBits = new short[frame.PixelDataLength];
            frame.CopyPixelDataTo(ImageBits);

            byte[] pixels = new byte[frame.Width * frame.Height * 4];

            int i32, i16;

            for (i16 = 0, i32 = 0; i16 < frame.PixelDataLength && i32 < pixels.Length; i16++, i32 += 4)
            {

                int dist = ImageBits[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                byte grayscale = Get8bitGrayScale(dist);
                pixels[i32 + 0] = grayscale;
                pixels[i32 + 1] = grayscale;
                pixels[i32 + 2] = grayscale;
                //pixels[i32 + 3] = Get8bitGrayScale(dist);

            }

            return pixels;

        }


        public static Bitmap ByteToBitmap(int width, int height, byte [] pixels) {

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            //using (bmp)
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length); bmp.UnlockBits(bmpData);
            }

            return bmp;
        }


        private static byte Get8bitGrayScale(int distance)
        {
            byte gray = (byte)0;

            if (distance > 4500) gray = (byte)0;
            else if (distance <= 0) gray = (byte)0;
            else {
                distance /= 18;
                distance = 255 - distance;
                gray = (byte)(distance);
            } 
            
            return gray;

        }




        Color[] playerColor = new Color[7];
        

        

        private  Color SetPlayerColor(byte[] nPlayers, int nPos, int player)
        {
            
            if (player == 0 || player > 5) return Color.Black;

            player--;

            nPlayers[nPos + 2] = playerColor[player].R;
            nPlayers[nPos + 1] = playerColor[player].G;
            nPlayers[nPos + 0] = playerColor[player].B;


            return playerColor[player];

        }

        public Color GetPlayerColor(int player) {
            if (player <= 0) return Color.Black;
            return playerColor[player - 1];
        }
        

        public  byte[] MarkPlayer(DepthImageFrame PImage)
        {
            byte[] playerCoded = new byte[PImage.Width * PImage.Height * 4];
            long[] lCount = { 0, 0, 0 };

            short[] depthFrame = new short[PImage.PixelDataLength];
            PImage.CopyPixelDataTo(depthFrame);

            for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < playerCoded.Length; i16++, i32 += 4)
            {
                int player = depthFrame[i16] & DepthImageFrame.PlayerIndexBitmask;
                
                SetRGB(playerCoded, i32, 0x00, 0x00, 0x00);
                SetPlayerColor(playerCoded, i32, player);

            }

            return playerCoded;
        }


        public void MarkPlayer(ref DepthImageFrame PImage, short[] depthFrame, byte[] playerCoded)
        {
            
            
            for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < playerCoded.Length; i16++, i32 += 4)
            {
                int player = depthFrame[i16] & DepthImageFrame.PlayerIndexBitmask;

                SetPlayerColor(playerCoded, i32, player);

            }
            return;
        }



        public int GetPlayerIdx(ref DepthImageFrame PImage, short[] depthFrame, int x, int y)
        {


            int point;
            int player;

            if (x > FrameWidth) return 0;
            if (y > FrameHeight) return 0;


            try
            {
                point = (y * PImage.Width) + (x);
                player = depthFrame[point] & DepthImageFrame.PlayerIndexBitmask;

            }
            catch {
                return 0;
            }
            


            return player;
        
        }




        void SetRGB(byte[] nPlayers, int nPos, byte r, byte g, byte b)
        {
            nPlayers[nPos + 2] = r;
            nPlayers[nPos + 1] = g;
            nPlayers[nPos + 0] = b;
        }















        ///Joint Process Methods
        ///Depth Points...
        ///Skeleton Points...
        ///
        SkeletonFrame sf;
        DepthImageFrame depthImageFrame;

        Skeleton[] skeletonData;
        
        public void SetAllFrame(SkeletonFrame sf, DepthImageFrame depthImageFrame)
        {
            this.sf = sf;
            this.depthImageFrame = depthImageFrame;

            if (sf == null) return;

            skeletonData = new Skeleton[sf.SkeletonArrayLength];

            sf.CopySkeletonDataTo(skeletonData);
            return;
            using (depthImageFrame)
            {
                if (depthImageFrame != null)
                {
                    foreach (Skeleton sd in skeletonData)
                    {
                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            

                        }
                    }
                }
            }

        }

        Point initPoint = new Point(0, 0);




        public float GetJointDistance(Skeleton sd, JointType type) {
            return sd.Joints[type].Position.Z;
        }


        /*
        public void GetJointPoint(Point point, int skeletonNum, JointType type)
        {
            
            if (skeletonNum > sf.SkeletonArrayLength) return;

            Skeleton sd = skeletonData[skeletonNum];
            
            if (sd.TrackingState == SkeletonTrackingState.Tracked)
            {
                Joint joint;
                joint = sd.Joints[JointType.Head];

                DepthImagePoint depthPoint;
                depthPoint = depthImageFrame.MapFromSkeletonPoint(joint.Position);


                point = new Point((int)(FrameWidth * depthPoint.X / depthImageFrame.Width),
                                        (int)(FrameHeight * depthPoint.Y / depthImageFrame.Height));

            

            }
            
            return;
        }

        */


        public Point GetJointPoint(Skeleton sd, JointType type)
        {
            if (sd.TrackingState == SkeletonTrackingState.Tracked)
            {
                Joint joint;
                joint = sd.Joints[type];

                DepthImagePoint depthPoint;
                depthPoint = depthImageFrame.MapFromSkeletonPoint(joint.Position);


                return new Point((int)(FrameWidth * depthPoint.X / depthImageFrame.Width),
                                        (int)(FrameHeight * depthPoint.Y / depthImageFrame.Height));


            }

            return initPoint;
        }
        
    }
}
