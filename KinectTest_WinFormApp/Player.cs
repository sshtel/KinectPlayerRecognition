using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using Microsoft.Kinect;

namespace KinectTest_WinFormApp
{

    public class JointPointGroup {
        public Point Head;
        public Point Spine;
        public Point ShoulderCenter;
        public Point ShoulderLeft;
        public Point ShoulderRight;
        public Point ElbowLeft;
        public Point ElbowRight;
        public Point WristLeft;
        public Point WristRight;
        public Point GetPoint(JointType type){
            switch (type) { 
                case JointType.Head:
                    return Head;
                case JointType.Spine:
                    return Spine;
            }

            return new Point(0, 0);
        }

    }


    public class JointGroup {
        public Joint Head;
        public Joint Spine;
        public Joint ShoulderCenter;
        public Joint ShoulderLeft;
        public Joint ShoulderRight;
        public Joint ElbowLeft;
        public Joint ElbowRight;
        public Joint WristLeft;
        public Joint WristRight;
    }



    public class JointInfo {
        public JointGroup joints { get; set; }

        public float meanX { get; set; }
        public float meanY { get; set; }

        public float varianceX { get; set; }
        public float varianceY { get; set; }

        public float covariance { get; set; }



    }


    class Player
    {
        
        //variance of.. head, shoulder center, shoulder left, shoulder right, spine, 

        
        Queue<JointInfo> jointInfoQ = new Queue<JointInfo>();
        
        int numOfJoints = 9;
        
        DepthImageFrame depthImageFrame;
        short[] depthFrame;

        public void SetDepthImageFrame(ref DepthImageFrame depthImageFrame, short [] depthFrame) {
            this.FrameWidth = depthImageFrame.Width;
            this.FrameHeight = depthImageFrame.Height;
            this.depthImageFrame = depthImageFrame;

            this.depthFrame = depthFrame;

            return;
            try {
                this.depthFrame = new short[depthImageFrame.PixelDataLength];
                depthImageFrame.CopyPixelDataTo(depthFrame);

            }
            catch (Exception ee) { ee.ToString(); }
            
        }




        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }


        Point initPoint = new Point(0, 0);




        Queue<JointPointGroup> jpgQ = new Queue<JointPointGroup>();
        int jpgQmax = 10;
        JointPointGroup curr_jpg;

        private void EnqJointPointGroup(JointPointGroup jpg)
        {
            
            jpgQ.Enqueue(jpg);
            if (jpgQ.Count > jpgQmax) jpgQ.Dequeue();
            this.curr_jpg = jpgQ.Last();

            JointInfo j = new JointInfo();


            float meanX = 0;
            float meanY = 0;

            float varianceX= 0;
            float varianceY = 0;

            meanX += jpg.Head.X;
            meanX += jpg.Spine.X;
            meanX += jpg.ShoulderCenter.X;
            meanX += jpg.ShoulderLeft.X;
            meanX += jpg.ShoulderRight.X;
            meanX += jpg.ElbowLeft.X;
            meanX += jpg.ElbowRight.X;
            meanX += jpg.WristLeft.X;
            meanX += jpg.WristRight.X;

            meanX /= numOfJoints;



            varianceX += (meanX - (float)jpg.Head.X) * (meanX - (float)jpg.Head.X);
            varianceX += (meanX - (float)jpg.Spine.X) * (meanX - (float)jpg.Spine.X);
            varianceX += (meanX - (float)jpg.ShoulderCenter.X) * (meanX - (float)jpg.ShoulderCenter.X);
            varianceX += (meanX - (float)jpg.ShoulderLeft.X) * (meanX - (float)jpg.ShoulderLeft.X);
            varianceX += (meanX - (float)jpg.ShoulderRight.X) * (meanX - (float)jpg.ShoulderRight.X);
            varianceX += (meanX - (float)jpg.ElbowLeft.X) * (meanX - (float)jpg.ElbowLeft.X);
            varianceX += (meanX - (float)jpg.ElbowRight.X) * (meanX - (float)jpg.ElbowRight.X);
            varianceX += (meanX - (float)jpg.WristLeft.X) * (meanX - (float)jpg.WristLeft.X);
            varianceX += (meanX - (float)jpg.WristRight.X) * (meanX - (float)jpg.WristRight.X);

            varianceX /= numOfJoints;





            meanY += jpg.Head.Y;
            meanY += jpg.Spine.Y;
            meanY += jpg.ShoulderCenter.Y;
            meanY += jpg.ShoulderLeft.Y;
            meanY += jpg.ShoulderRight.Y;
            meanY += jpg.ElbowLeft.Y;
            meanY += jpg.ElbowRight.Y;
            meanY += jpg.WristLeft.Y;
            meanY += jpg.WristRight.Y;

            meanY /= numOfJoints;


            varianceY += (meanY - (float)jpg.Head.Y)             * (meanY - (float)jpg.Head.Y);
            varianceY += (meanY - (float)jpg.Spine.Y)            * (meanY - (float)jpg.Spine.Y);
            varianceY += (meanY - (float)jpg.ShoulderCenter.Y)   * (meanY - (float)jpg.ShoulderCenter.Y);
            varianceY += (meanY - (float)jpg.ShoulderLeft.Y)     * (meanY - (float)jpg.ShoulderLeft.Y);
            varianceY += (meanY - (float)jpg.ShoulderRight.Y)    * (meanY - (float)jpg.ShoulderRight.Y);
            varianceY += (meanY - (float)jpg.ElbowLeft.Y)        * (meanY - (float)jpg.ElbowLeft.Y);
            varianceY += (meanY - (float)jpg.ElbowRight.Y)       * (meanY - (float)jpg.ElbowRight.Y);
            varianceY += (meanY - (float)jpg.WristLeft.Y)        * (meanY - (float)jpg.WristLeft.Y);
            varianceY += (meanY - (float)jpg.WristRight.Y)       * (meanY - (float)jpg.WristRight.Y);

            varianceY /= numOfJoints;




            j.meanX = meanX;
            j.meanY = meanY;
            j.varianceX = varianceX;
            j.varianceY = varianceY;



            
            //jointInfoQ.Enqueue(j);




        }

        public JointPointGroup GetCurrentJointPointGroup() {
            return curr_jpg;
        }
        
        
        
        public float GetMeanX() {
            if (skelQ.Count <= 0) return 0;
            Skeleton sd = skelQ.Last();
            if (sd == null) return 0;

            float meanX = 0;
            meanX += sd.Joints[JointType.Head].Position.X;
            meanX += sd.Joints[JointType.ShoulderCenter].Position.X;
            meanX += sd.Joints[JointType.ShoulderLeft].Position.X;
            meanX += sd.Joints[JointType.ShoulderRight].Position.X;
            meanX += sd.Joints[JointType.Spine].Position.X;
            meanX += sd.Joints[JointType.ElbowLeft].Position.X;
            meanX += sd.Joints[JointType.ElbowRight].Position.X;
            meanX += sd.Joints[JointType.WristLeft].Position.X;
            meanX += sd.Joints[JointType.WristRight].Position.X;
            meanX /= numOfJoints;

            return meanX;

        }
        public float GetMeanY()
        {
            if (skelQ.Count <= 0) return 0;
            Skeleton sd = skelQ.Last();
            if (sd == null) return 0;


            float meanY = 0;
            meanY += sd.Joints[JointType.Head].Position.Y;
            meanY += sd.Joints[JointType.ShoulderCenter].Position.Y;
            meanY += sd.Joints[JointType.ShoulderLeft].Position.Y;
            meanY += sd.Joints[JointType.ShoulderRight].Position.Y;
            meanY += sd.Joints[JointType.Spine].Position.Y;
            meanY += sd.Joints[JointType.ElbowLeft].Position.Y;
            meanY += sd.Joints[JointType.ElbowRight].Position.Y;
            meanY += sd.Joints[JointType.WristLeft].Position.Y;
            meanY += sd.Joints[JointType.WristRight].Position.Y;
            meanY /= numOfJoints;

            return meanY;
        }
        public float GetMeanZ()
        {
            if (skelQ.Count <= 0) return 0;
            Skeleton sd = skelQ.Last();
            if (sd == null) return 0;



            float meanZ = 0;
            meanZ += sd.Joints[JointType.Head].Position.Z;
            meanZ += sd.Joints[JointType.ShoulderCenter].Position.Z;
            meanZ += sd.Joints[JointType.ShoulderLeft].Position.Z;
            meanZ += sd.Joints[JointType.ShoulderRight].Position.Z;
            meanZ += sd.Joints[JointType.Spine].Position.Z;
            meanZ += sd.Joints[JointType.ElbowLeft].Position.Z;
            meanZ += sd.Joints[JointType.ElbowRight].Position.Z;
            meanZ += sd.Joints[JointType.WristLeft].Position.Z;
            meanZ += sd.Joints[JointType.WristRight].Position.Z;
            meanZ /= numOfJoints;

            return meanZ;
        }





        /// <summary>
        /// Gaussian Matrix
        /// </summary>
        /// <returns></returns>

        private double[] GaussianMatrix = { 13.534, 32.465, 60.653, 88.250, 100.000 };

        public double GetLikelihood_XLocation()
        {

            double center = (double)FrameWidth / 2.0;
            double loc = GetMeanX();
            double blockSize = FrameWidth / 9;
            double halfBlockSize = blockSize / 2;
            double diff;


            /*
            diff = Math.Abs(loc - center);

            if (diff < halfBlockSize)
            {
                return GaussianMatrix[4];
            }
            else if (diff < blockSize + halfBlockSize)
            {
                return GaussianMatrix[3];
            }
            else if (diff < (blockSize * 2) + halfBlockSize)
            {
                return GaussianMatrix[2];
            }
            else if (diff < (blockSize * 3) + halfBlockSize)
            {
                return GaussianMatrix[1];
            }
            else if (diff < (blockSize * 4) + halfBlockSize)
            {
                return GaussianMatrix[0];
            }
            else
            {
                return 0;
            }


            */


            loc = Math.Abs(loc);

            if (loc > 0.5) { return GaussianMatrix[0]; }
            else if (loc > 0.4) { return GaussianMatrix[0]; }
            else if (loc > 0.3) { return GaussianMatrix[1]; }
            else if (loc > 0.2) { return GaussianMatrix[2]; }
            else if (loc > 0.1) { return GaussianMatrix[3]; }
            else { return GaussianMatrix[4]; }
            



        }
        
















        public float GetVarianceX() {
            if (skelQ.Count <= 0) return 0;
            Skeleton sd = skelQ.Last();
            if (sd == null) return 0;



            float meanX = GetMeanX();
            
            Joint Head = sd.Joints[JointType.Head];
            Joint Spine = sd.Joints[JointType.Spine];
            Joint ShoulderCenter = sd.Joints[JointType.ShoulderCenter];
            Joint ShoulderLeft = sd.Joints[JointType.ShoulderLeft];
            Joint ShoulderRight = sd.Joints[JointType.ShoulderRight];
            Joint ElbowLeft = sd.Joints[JointType.ElbowLeft];
            Joint ElbowRight = sd.Joints[JointType.ElbowRight];
            Joint WristLeft = sd.Joints[JointType.WristLeft];
            Joint WristRight = sd.Joints[JointType.WristRight];




            float var = 0;


            var += (meanX - Head.Position.X) * (meanX - Head.Position.X);
            var += (meanX - Spine.Position.X) * (meanX - Spine.Position.X);
            var += (meanX - ShoulderCenter.Position.X) * (meanX - ShoulderCenter.Position.X);
            var += (meanX - ShoulderLeft.Position.X) * (meanX - ShoulderLeft.Position.X);
            var += (meanX - ShoulderRight.Position.X) * (meanX - ShoulderRight.Position.X);
            var += (meanX - ElbowLeft.Position.X) * (meanX - ElbowLeft.Position.X);
            var += (meanX - ElbowRight.Position.X) * (meanX - ElbowRight.Position.X);
            var += (meanX - WristLeft.Position.X) * (meanX - WristLeft.Position.X);
            var += (meanX - WristRight.Position.X) * (meanX - WristRight.Position.X);

            var /= numOfJoints;

            return var;
        }


        public float GetVarianceY()
        {
            if (skelQ.Count <= 0) return 0;
            Skeleton sd = skelQ.Last();
            if (sd == null) return 0;


            float meanY = GetMeanY();

            Joint Head = sd.Joints[JointType.Head];
            Joint Spine = sd.Joints[JointType.Spine];
            Joint ShoulderCenter = sd.Joints[JointType.ShoulderCenter];
            Joint ShoulderLeft = sd.Joints[JointType.ShoulderLeft];
            Joint ShoulderRight = sd.Joints[JointType.ShoulderRight];
            Joint ElbowLeft = sd.Joints[JointType.ElbowLeft];
            Joint ElbowRight = sd.Joints[JointType.ElbowRight];
            Joint WristLeft = sd.Joints[JointType.WristLeft];
            Joint WristRight = sd.Joints[JointType.WristRight];




            float var = 0;



            var += (meanY - Head.Position.Y) * (meanY - Head.Position.Y);
            var += (meanY - Spine.Position.Y) * (meanY - Spine.Position.Y);
            var += (meanY - ShoulderCenter.Position.Y) * (meanY - ShoulderCenter.Position.Y);
            var += (meanY - ShoulderLeft.Position.Y) * (meanY - ShoulderLeft.Position.Y);
            var += (meanY - ShoulderRight.Position.Y) * (meanY - ShoulderRight.Position.Y);
            var += (meanY - ElbowLeft.Position.Y) * (meanY - ElbowLeft.Position.Y);
            var += (meanY - ElbowRight.Position.Y) * (meanY - ElbowRight.Position.Y);
            var += (meanY - WristLeft.Position.Y) * (meanY - WristLeft.Position.Y);
            var += (meanY - WristRight.Position.Y) * (meanY - WristRight.Position.Y);




            var /= numOfJoints;

            return var;
        }



        public float GetCovariance2D(Skeleton sd)
        {

            float meanX = GetMeanX();
            float meanY = GetMeanY();
            
            Joint Head = sd.Joints[JointType.Head];
            Joint Spine = sd.Joints[JointType.Spine];
            Joint ShoulderCenter = sd.Joints[JointType.ShoulderCenter];
            Joint ShoulderLeft = sd.Joints[JointType.ShoulderLeft];
            Joint ShoulderRight = sd.Joints[JointType.ShoulderRight];
            Joint ElbowLeft = sd.Joints[JointType.ElbowLeft];
            Joint ElbowRight = sd.Joints[JointType.ElbowRight];
            Joint WristLeft = sd.Joints[JointType.WristLeft];
            Joint WristRight = sd.Joints[JointType.WristRight];




            float Covar = 0;


            Covar += (meanX - Head.Position.X) * (meanY - Head.Position.Y);
            Covar += (meanX - Spine.Position.X) * (meanY - Spine.Position.Y);
            Covar += (meanX - ShoulderCenter.Position.X) * (meanY - ShoulderCenter.Position.Y);
            Covar += (meanX - ShoulderLeft.Position.X) * (meanY - ShoulderLeft.Position.Y);
            Covar += (meanX - ShoulderRight.Position.X) * (meanY - ShoulderRight.Position.Y);
            Covar += (meanX - ElbowLeft.Position.X) * (meanY - ElbowLeft.Position.Y);
            Covar += (meanX - ElbowRight.Position.X) * (meanY - ElbowRight.Position.Y);
            Covar += (meanX - WristLeft.Position.X) * (meanY - WristLeft.Position.Y);
            Covar += (meanX - WristRight.Position.X) * (meanY - WristRight.Position.Y);

            Covar /= numOfJoints;


            return Covar;
        }



        private void GetVarianceXY() {
            if (skelQ.Count <= 0) return;
            Skeleton sd = skelQ.Last();
            if (sd == null) return;


            JointGroup joints = new JointGroup();

            joints.Head = sd.Joints[JointType.Head];
            joints.ShoulderCenter = sd.Joints[JointType.ShoulderCenter];
            joints.ShoulderLeft = sd.Joints[JointType.ShoulderLeft];
            joints.ShoulderRight = sd.Joints[JointType.ShoulderRight];
            joints.Spine = sd.Joints[JointType.Spine];
            joints.ElbowLeft = sd.Joints[JointType.ElbowLeft];
            joints.ElbowRight = sd.Joints[JointType.ElbowRight];
            joints.WristLeft = sd.Joints[JointType.WristLeft];
            joints.WristRight = sd.Joints[JointType.WristRight];

            

            



            

            float meanX = 0;
            float meanY = 0;

            float varianceX = 0;
            float varianceY = 0;

            meanX += joints.Head.Position.X;
            meanX += joints.Spine.Position.X;
            meanX += joints.ShoulderCenter.Position.X;
            meanX += joints.ShoulderLeft.Position.X;
            meanX += joints.ShoulderRight.Position.X;
            meanX += joints.ElbowLeft.Position.X;
            meanX += joints.ElbowRight.Position.X;
            meanX += joints.WristLeft.Position.X;
            meanX += joints.WristRight.Position.X;

            meanX /= numOfJoints;



            varianceX += (meanX - (float)joints.Head.Position.X) * (meanX - (float)joints.Head.Position.X);
            varianceX += (meanX - (float)joints.Spine.Position.X) * (meanX - (float)joints.Spine.Position.X);
            varianceX += (meanX - (float)joints.ShoulderCenter.Position.X) * (meanX - (float)joints.ShoulderCenter.Position.X);
            varianceX += (meanX - (float)joints.ShoulderLeft.Position.X) * (meanX - (float)joints.ShoulderLeft.Position.X);
            varianceX += (meanX - (float)joints.ShoulderRight.Position.X) * (meanX - (float)joints.ShoulderRight.Position.X);
            varianceX += (meanX - (float)joints.ElbowLeft.Position.X) * (meanX - (float)joints.ElbowLeft.Position.X);
            varianceX += (meanX - (float)joints.ElbowRight.Position.X) * (meanX - (float)joints.ElbowRight.Position.X);
            varianceX += (meanX - (float)joints.WristLeft.Position.X) * (meanX - (float)joints.WristLeft.Position.X);
            varianceX += (meanX - (float)joints.WristRight.Position.X) * (meanX - (float)joints.WristRight.Position.X);

            varianceX /= numOfJoints;


            



            meanY += joints.Head.Position.Y;
            meanY += joints.Spine.Position.Y;
            meanY += joints.ShoulderCenter.Position.Y;
            meanY += joints.ShoulderLeft.Position.Y;
            meanY += joints.ShoulderRight.Position.Y;
            meanY += joints.ElbowLeft.Position.Y;
            meanY += joints.ElbowRight.Position.Y;
            meanY += joints.WristLeft.Position.Y;
            meanY += joints.WristRight.Position.Y;

            meanY /= numOfJoints;


            varianceY += (meanY - (float)joints.Head.Position.Y) * (meanY - (float)joints.Head.Position.Y);
            varianceY += (meanY - (float)joints.Spine.Position.Y) * (meanY - (float)joints.Spine.Position.Y);
            varianceY += (meanY - (float)joints.ShoulderCenter.Position.Y) * (meanY - (float)joints.ShoulderCenter.Position.Y);
            varianceY += (meanY - (float)joints.ShoulderLeft.Position.Y) * (meanY - (float)joints.ShoulderLeft.Position.Y);
            varianceY += (meanY - (float)joints.ShoulderRight.Position.Y) * (meanY - (float)joints.ShoulderRight.Position.Y);
            varianceY += (meanY - (float)joints.ElbowLeft.Position.Y) * (meanY - (float)joints.ElbowLeft.Position.Y);
            varianceY += (meanY - (float)joints.ElbowRight.Position.Y) * (meanY - (float)joints.ElbowRight.Position.Y);
            varianceY += (meanY - (float)joints.WristLeft.Position.Y) * (meanY - (float)joints.WristLeft.Position.Y);
            varianceY += (meanY - (float)joints.WristRight.Position.Y) * (meanY - (float)joints.WristRight.Position.Y);


            varianceY /= numOfJoints;




            JointInfo j = new JointInfo();

            j.joints = joints;

            j.meanX = meanX;
            j.meanY = meanY;
            j.varianceX = varianceX;
            j.varianceY = varianceY;
            
            

        }





        KinectProc kproc;

        public void SetKinectProc(KinectProc kproc) { this.kproc = kproc; }




        Queue<Skeleton> skelQ = new Queue<Skeleton>();
        int enqMax = 10;

        JointPointGroup jpGroup = new JointPointGroup();
        int playerIndex;

        Skeleton sd;
        public void EnqueueSkeleton(Skeleton sd)
        {
            this.sd = sd;


            jpGroup.Head = kproc.GetJointPoint(sd, JointType.Head);
            jpGroup.ShoulderCenter = kproc.GetJointPoint(sd, JointType.ShoulderCenter);
            jpGroup.ShoulderLeft = kproc.GetJointPoint(sd, JointType.ShoulderLeft);
            jpGroup.ShoulderRight = kproc.GetJointPoint(sd, JointType.ShoulderRight);
            jpGroup.Spine = kproc.GetJointPoint(sd, JointType.Spine);
            jpGroup.ElbowLeft = kproc.GetJointPoint(sd, JointType.ElbowLeft);
            jpGroup.ElbowRight = kproc.GetJointPoint(sd, JointType.ElbowRight);
            jpGroup.WristLeft = kproc.GetJointPoint(sd, JointType.WristLeft);
            jpGroup.WristRight = kproc.GetJointPoint(sd, JointType.WristRight);



            playerIndex = kproc.GetPlayerIdx(ref depthImageFrame, depthFrame, jpGroup.Head.X, jpGroup.Head.Y);


            
            


            this.EnqJointPointGroup(jpGroup);

            //return;
            skelQ.Enqueue(sd);
            if (skelQ.Count > enqMax) skelQ.Dequeue();

        }


        public int GetPlayerIndex() { return playerIndex; }

        public Color GetPlayerColor() {            return kproc.GetPlayerColor(playerIndex);        }
        
        public Point GetJointPoint(JointType type){
            return jpGroup.GetPoint(type);
            return  kproc.GetJointPoint(skelQ.Last(), type);
        }
        public JointPointGroup GetJointPointGroup() { return jpGroup; }



        public void GetMeanVelocityWrist(JointType type)
        {

            double total_movement = 0;

            float prev_x = 0;
            float prev_y = 0;
            float prev_z = 0;

            foreach (Skeleton s in skelQ)
            {
                Joint j = s.Joints[type];

                if (prev_x == 0 && prev_y == 0 && prev_z == 0)
                {
                    continue;
                }

                float curr_x, curr_y, curr_z;
                curr_x = j.Position.X;
                curr_y = j.Position.Y;
                curr_z = j.Position.Z;


                double dist = 0;
                dist += Math.Pow((curr_x - prev_x), 2);
                dist += Math.Pow((curr_y - prev_y), 2);
                dist += Math.Pow((curr_z - prev_z), 2);
                dist = Math.Sqrt(dist);
                total_movement += dist;


                prev_x = s.Joints[JointType.WristLeft].Position.X;
                prev_y = s.Joints[JointType.WristLeft].Position.Y;
                prev_z = s.Joints[JointType.WristLeft].Position.Z;

            }

        }





        private double GetAcummulatedDistance(JointType type)
        {

            double total_movement = 0;

            float prev_x = 0;
            float prev_y = 0;
            float prev_z = 0;

            //foreach (Skeleton s in skelQ)


            prev_x = skelQ.ElementAt(0).Joints[type].Position.X;
            prev_y = skelQ.ElementAt(0).Joints[type].Position.Y;
            prev_z = skelQ.ElementAt(0).Joints[type].Position.Z;


            if (skelQ.Count < 2) return 0;

            for (int i = 1; i < skelQ.Count; i++)
            {
                Skeleton s = skelQ.ElementAt(i);
                Joint j = s.Joints[type];




                float curr_x, curr_y, curr_z;
                curr_x = j.Position.X;
                curr_y = j.Position.Y;
                curr_z = j.Position.Z;


                Skeleton prev_s = skelQ.ElementAt(i - 1);
                Joint prev_j = prev_s.Joints[type];

                prev_x = prev_j.Position.X;
                prev_y = prev_j.Position.Y;
                prev_z = prev_j.Position.Z;


                double dist = 0;
                dist += Math.Pow((curr_x - prev_x), 2);
                dist += Math.Pow((curr_y - prev_y), 2);
                dist += Math.Pow((curr_z - prev_z), 2);
                dist = Math.Sqrt(dist);
                total_movement += dist;

            }

            return total_movement;

        }




        Queue<double> WristLeftVelocityQ = new Queue<double>();
        int VelocityQMax = 50;


        public double GetMeanVelocityWristLeft()
        {
            double dist = GetAcummulatedDistance(JointType.WristLeft);

            WristLeftVelocityQ.Enqueue(dist);
            if (WristLeftVelocityQ.Count >= VelocityQMax) WristLeftVelocityQ.Dequeue();


            double sum = 0;
            foreach (double d in WristLeftVelocityQ)
            {
                sum += d;

            }

            return sum / WristLeftVelocityQ.Count;

        }



        Queue<double> WristRightVelocityQ = new Queue<double>();

        public double GetMeanVelocityWristRight()
        {
            double dist = GetAcummulatedDistance(JointType.WristRight);

            WristRightVelocityQ.Enqueue(dist);
            if (WristRightVelocityQ.Count >= VelocityQMax) WristRightVelocityQ.Dequeue();


            double sum = 0;
            foreach (double d in WristRightVelocityQ)
            {
                sum += d;

            }

            return sum / WristRightVelocityQ.Count;

        }








        public double GetDistPrevCurrJoint(JointType type)
        {
            int count = skelQ.Count;

            float x1, x2, y1, y2, z1, z2;

            x1 = skelQ.ElementAt(count).Joints[type].Position.X;
            y1 = skelQ.ElementAt(count).Joints[type].Position.Y;
            z1 = skelQ.ElementAt(count).Joints[type].Position.Z;

            x2 = skelQ.ElementAt(count - 1).Joints[type].Position.X;
            y2 = skelQ.ElementAt(count - 1).Joints[type].Position.Y;
            z2 = skelQ.ElementAt(count - 1).Joints[type].Position.Z;

            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2) + Math.Pow((z1 - z2), 2));

        }









        

    }
}
