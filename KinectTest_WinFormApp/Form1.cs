using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Kinect;

using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


namespace KinectTest_WinFormApp
{
    public partial class s : Form
    {
        public s()
        {
            InitializeComponent();
            InitializeNui();
        }

        KinectSensor nui = null;

        KinectProc kproc = new KinectProc();



        Point[] jointpoint = new Point[30];


        //JointPointGroup jpGroup = new JointPointGroup();




        int numOfPlayers = 20;
        Player[] players;
        

        void InitializeNui()
        {

            kproc.FrameWidth = pictureBox_Color.Width;
            kproc.FrameHeight = pictureBox_Color.Height;

            players = new Player[numOfPlayers];
            for (int i = 0; i < numOfPlayers; i++) { players[i] = new Player(); }



            nui = KinectSensor.KinectSensors[0];

            nui.ColorStream.Enable(ColorImageFormat.RawYuvResolution640x480Fps15);
            nui.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(nui_ColorFrameReady);

            nui.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            //nui.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(nui_DepthFrameReady);
            
            
            nui.SkeletonStream.Enable();

            nui.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(nui_AllFramesReady);
            
            nui.Start();
        }
        
        void nui_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame ImageParam = e.OpenColorImageFrame();

            if (ImageParam == null) return;
            
            try
            {
                this.pictureBox_Color.Image = (Image)KinectProc.ColorImageFrameToBitmap(ref ImageParam, PixelFormat.Format32bppRgb);
            }
            catch (Exception ) { 
                
            }
            
        }
        
        void nui_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            DepthImageFrame ImageParam_Depth = null;
                
            ImageParam_Depth = e.OpenDepthImageFrame();

            
            if (ImageParam_Depth == null) return;

            short[] depthFrame = new short[ImageParam_Depth.PixelDataLength];

            ImageParam_Depth.CopyPixelDataTo(depthFrame);


            
            //pictureBox_Depth.Image = (Bitmap)KinectProc.DepthToGrayBitmap(ref ImageParam_Depth);

            byte []pixels = KinectProc.DepthToGrayByte(ref ImageParam_Depth);


            //pictureBox_Depth.Image = (Bitmap)KinectProc.ByteToBitmap(ImageParam_Depth.Width, ImageParam_Depth.Height, pixels);


            kproc.MarkPlayer(ref ImageParam_Depth, depthFrame, pixels);                      

            pictureBox_Depth.Image = (Bitmap)KinectProc.ByteToBitmap(ImageParam_Depth.Width, ImageParam_Depth.Height, pixels);


            

            //textBox_dist.Text = (KinectProc.GetDistance(100, 100, ref ImageParam_Depth)).ToString();


            



            return;

            short[] ImageBits = new short[ImageParam_Depth.PixelDataLength];
            ImageParam_Depth.CopyPixelDataTo(ImageBits);


            Bitmap bmp = new Bitmap(ImageParam_Depth.Width, ImageParam_Depth.Height, PixelFormat.Format16bppGrayScale);

            //using (bmp)
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                Marshal.Copy(ImageBits, 0, bmpData.Scan0, ImageBits.Length); bmp.UnlockBits(bmpData);
            }

            pictureBox_Depth.Image = bmp;

            /*
            BitmapSource src = null;
            src = BitmapSource.Create(ImageParam_Depth.Width, ImageParam_Depth.Height,
                                    96, 96, PixelFormats.Gray16,
                                    null, ImageBits,
                                    ImageParam_Depth.Width * ImageParam_Depth.BytesPerPixel);
            image_depth.Source = src;
             * */


        }

        






        //listbox 

        int skeletonCount = 0;
        int SkeletonIndex = 0;
        Skeleton[] skeletonData;


        
        void nui_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            
            SkeletonFrame sf = e.OpenSkeletonFrame();

            if (sf == null) return;

            skeletonData = new Skeleton[sf.SkeletonArrayLength];

            sf.CopySkeletonDataTo(skeletonData);

            
            //using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            DepthImageFrame depthImageFrame = e.OpenDepthImageFrame();
            {
                if (depthImageFrame != null)
                {


                    ///Depth Draw
                    ///
                    
                    short[] depthFrame = new short[depthImageFrame.PixelDataLength];

                    depthImageFrame.CopyPixelDataTo(depthFrame);



                    //pictureBox_Depth.Image = (Bitmap)KinectProc.DepthToGrayBitmap(ref ImageParam_Depth);

                    byte[] pixels = KinectProc.DepthToGrayByte(ref depthImageFrame);


                    //pictureBox_Depth.Image = (Bitmap)KinectProc.ByteToBitmap(ImageParam_Depth.Width, ImageParam_Depth.Height, pixels);


                    kproc.MarkPlayer(ref depthImageFrame, depthFrame, pixels);
                    
                    pictureBox_Depth.Image = (Bitmap)KinectProc.ByteToBitmap(depthImageFrame.Width, depthImageFrame.Height, pixels);




                    ///
                    /// Skeleton

                    kproc.SetAllFrame(sf, depthImageFrame);


                    skeletonCount = 0;



                    int playerCount = 0;

                    foreach (Skeleton sd in skeletonData)
                    {

                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {


                            Player unknownPlayer = new Player();



                            /*
                            jpGroup.Head = kproc.GetJointPoint(sd, JointType.Head);
                            jpGroup.ShoulderCenter = kproc.GetJointPoint(sd, JointType.ShoulderCenter);
                            jpGroup.ShoulderLeft = kproc.GetJointPoint(sd, JointType.ShoulderLeft);
                            jpGroup.ShoulderRight = kproc.GetJointPoint(sd, JointType.ShoulderRight);
                            jpGroup.Spine = kproc.GetJointPoint(sd, JointType.Spine);
                            jpGroup.ElbowLeft = kproc.GetJointPoint(sd, JointType.ElbowLeft);
                            jpGroup.ElbowRight = kproc.GetJointPoint(sd, JointType.ElbowRight);
                            jpGroup.WristLeft = kproc.GetJointPoint(sd, JointType.WristLeft);
                            jpGroup.WristRight = kproc.GetJointPoint(sd, JointType.WristRight);
                            */

                            unknownPlayer.SetKinectProc(kproc);
                            unknownPlayer.SetDepthImageFrame(ref depthImageFrame, depthFrame);
                            unknownPlayer.EnqueueSkeleton(sd);
                            
                            

                            //int playerIndex = kproc.GetPlayerIdx(ref depthImageFrame, depthFrame, jpGroup.Head.X, jpGroup.Head.Y);
                            int playerIndex = unknownPlayer.GetPlayerIndex();
                            //Color player_color = kproc.GetPlayerColor(playerIndex);
                            Color player_color = unknownPlayer.GetPlayerColor();

                            //players[playerIndex].SetDepthImageFrame(depthImageFrame);
                            
                            //players[playerIndex].EnqJointPointGroup(jpGroup);
                            //players[playerIndex].EnqueueSkeleton(sd);



                            float meanX = unknownPlayer.GetMeanX();
                            float meanY = unknownPlayer.GetMeanY();
                            float meanZ = unknownPlayer.GetMeanZ();
                            

                            textBox_meanX.Text = meanX.ToString();
                            textBox_meanY.Text = meanY.ToString();
                            


                            textBox_Height.Text = sd.Joints[JointType.Head].Position.Y.ToString();

                            
                            textBox_varianceX.Text = unknownPlayer.GetVarianceX().ToString();
                            textBox_varianceY.Text = unknownPlayer.GetVarianceY().ToString();





                            



                            

                            //draw

                            float headDistance = kproc.GetJointDistance(sd, JointType.Head);
                            textBox_location.Text = string.Format("X:{0:0.00} Y:{1:0.00} Z:{2:0.00}",
                                                            unknownPlayer.GetJointPoint(JointType.Head).X,
                                                            unknownPlayer.GetJointPoint(JointType.Head).Y,
                                                            headDistance);





                            ///Drawer!!!
                            

                            //if (drawer != null) 
                            
                            {

                                players[playerCount] = unknownPlayer;
                                playerCount++;
                                
                            }

                            


                            pictureBox_location.Refresh();



                        }


                    }


                    //end of foreach

                    
                    if (playerCount > 3) playerCount = 3;
                    drawer.TotalPlayerNum = playerCount;
                    textBox_skelcount.Text = playerCount.ToString();

                    double prior = (double)(1.0 / playerCount);
                    textBox_prior.Text = prior.ToString();
                    
                    double evidence = 0;

                    for (int i = 0; i < playerCount; i++) {
                        evidence += (players[i].GetLikelihood_XLocation() * prior);
                    }




                    //players...
                    if (playerCount >= 1) {
                        groupBoxA.Text = "player : " + players[0].GetPlayerIndex();
                        groupBoxA.BackColor = players[0].GetPlayerColor();
                        textBoxA_like.Text = players[0].GetLikelihood_XLocation().ToString();
                        textBoxA_location.Text = players[0].GetMeanX().ToString();
                        //evidence += (players[0].GetLikelihood_XLocation() * prior);
                        double p_da = (prior * players[0].GetLikelihood_XLocation() / evidence) * 100;
                        textBoxA_P_AD.Text = p_da.ToString() + "%";
                    }

                    if (playerCount >= 2) {
                        groupBoxB.Text = "player : " + players[1].GetPlayerIndex();
                        groupBoxB.BackColor = players[1].GetPlayerColor();
                        textBoxB_like.Text = players[1].GetLikelihood_XLocation().ToString();
                        textBoxB_location.Text = players[1].GetMeanX().ToString();
                        //evidence += (players[1].GetLikelihood_XLocation() * 3);
                        double p_db = (prior * players[1].GetLikelihood_XLocation() / evidence) * 100;
                        textBoxB_P_BD.Text = p_db.ToString() + "%";
                    }

                    if (playerCount >= 3)
                    {
                        groupBoxC.Text = "player : " + players[2].GetPlayerIndex();
                        groupBoxA.BackColor = players[2].GetPlayerColor();
                        textBoxC_like.Text = players[2].GetLikelihood_XLocation().ToString();
                        textBoxC_location.Text = players[2].GetMeanX().ToString();
                        //evidence += (players[2].GetLikelihood_XLocation() * 3);
                        double p_dc = (prior * players[2].GetLikelihood_XLocation() / evidence) * 100;
                        textBoxC_P_CD.Text = p_dc.ToString() + "%";
                        
                    }

                    textBox_evidence.Text = evidence.ToString();


                    
                    
                    






                    for (int i = 0; i < playerCount; i++) {
                        Player p = players[i];
                        drawer.SetPlayer(i, p);
                        
                    }



                    
                }
            }
        }

        private Bitmap ColorImageFrame2Bitmap(ColorImageFrame image){
            return null;
        }
        


        private void Form1_Load(object sender, EventArgs e)
        {
            drawer = new Drawer(pictureBox_location);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }





        Pen jointPen = new Pen(Color.Red, 2);
        private void pictureBox_Color_Paint(object sender, PaintEventArgs e)
        {

            int idx;


            
            drawer.DrawJointPoint(e);


            /*
            e.Graphics.DrawEllipse(jointPen, jpGroup.Head.X, jpGroup.Head.Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jpGroup.ShoulderCenter.X, jpGroup.ShoulderCenter.Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jpGroup.ShoulderLeft.X, jpGroup.ShoulderLeft.Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jpGroup.ShoulderRight.X, jpGroup.ShoulderRight.Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jpGroup.Spine.X, jpGroup.Spine.Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jpGroup.ElbowLeft.X, jpGroup.ElbowLeft.Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jpGroup.ElbowRight.X, jpGroup.ElbowRight.Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jpGroup.WristLeft.X, jpGroup.WristLeft.Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jpGroup.WristRight.X, jpGroup.WristRight.Y, 10, 10);

            */

           

            /*
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.Head].X, jointpoint[(int)JointType.Head].Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.ShoulderCenter].X, jointpoint[(int)JointType.ShoulderCenter].Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.ShoulderLeft].X, jointpoint[(int)JointType.ShoulderLeft].Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.ShoulderRight].X, jointpoint[(int)JointType.ShoulderRight].Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.Spine].X, jointpoint[(int)JointType.Spine].Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.ElbowLeft].X, jointpoint[(int)JointType.ElbowLeft].Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.ElbowRight].X, jointpoint[(int)JointType.ElbowRight].Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.WristLeft].X, jointpoint[(int)JointType.WristLeft].Y, 10, 10);
            e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.WristRight].X, jointpoint[(int)JointType.WristRight].Y, 10, 10);
            */


        }





        private void listBox_current_player_info_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void listBox_skeleton_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        

        /// <summary>
        /// Drawer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        Drawer drawer;



        
        private void pictureBox_all_Paint(object sender, PaintEventArgs e)
        {
            drawer.DrawLocation(e);
            //e.Graphics.DrawEllipse(jointPen, jointpoint[(int)JointType.Head].X, jointpoint[(int)JointType.Head].Y, 20, 20);
            e.Graphics.Flush();
        }

    }
}
