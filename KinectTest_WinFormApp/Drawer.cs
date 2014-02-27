using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Kinect;

namespace KinectTest_WinFormApp
{
    class Drawer
    {
        Player[] player;

        PictureBox pbox;
        float width;
        float height;

        Point pointKinect;
        Point pointLeft;
        Point pointRight;



        int numOfPlayer = 10;
        Point[] playerDrawPoint;



        public Drawer(PictureBox pbox) {
            this.pbox = pbox;
            width = pbox.Width;
            height = pbox.Height;

            pointKinect = new Point((int)(width / 2), 0);
            pointLeft = new Point(0,(int)height);
            pointRight = new Point((int)width, (int)height);




            playerDrawPoint = new Point[numOfPlayer];
            for (int i = 0; i < numOfPlayer; i++) { playerDrawPoint[i] = new Point(); }




            player = new Player[numOfPlayer];
            for (int i = 0; i < 10; i++) {
                player[i] = new Player();
            }

            
        }

        Pen LinePen = new Pen(Color.Black, 3);
        Pen playerPen = new Pen(Color.Black, 2);

        public void DrawLocation(PaintEventArgs e)
        {



            //foreach (Player p in player)
            for(int i=0;i<TotalPlayerNum; i++)
            {
                Player p = player[i];
                try
                {

                    e.Graphics.DrawLine(LinePen, pointKinect, pointLeft);
                    e.Graphics.DrawLine(LinePen, pointKinect, pointRight);
                    playerPen.Color = p.GetPlayerColor();
                    Point point = GetDrawLocationPoint(p);
                    e.Graphics.DrawEllipse(playerPen, point.X, point.Y, 10, 10);
            
                }
                catch (Exception ee)
                {

                }
            }

        }



        float xGain = 200;
        float yGain;
        float zGain = 70;


        public int TotalPlayerNum;

        public void SetPlayer(int idx, Player player) {
            this.player[idx] = player;

            float x, y, z;
            x = player.GetMeanX();
            y = player.GetMeanY();
            z = player.GetMeanZ();

            float dx = x * xGain;
            dx += (width / 2);


            float dy = z * zGain;


            playerDrawPoint[idx].X = (int)dx;
            playerDrawPoint[idx].Y = (int)dy;
            
        }


        private Point GetDrawLocationPoint(Player player) {

            float x, y, z;
            x = player.GetMeanX();
            y = player.GetMeanY();
            z = player.GetMeanZ();

            float dx = x * xGain;
            dx += (width / 2);


            float dy = z * zGain;

            Point point = new Point();
            point.X = (int)dx;
            point.Y = (int)dy;

            return point;
            
        }






        ///
        Pen jointPen = new Pen(Color.Black, 2);
        int EllipseSize = 10;
        public void DrawJointPoint(PaintEventArgs e)
        {
            
            //foreach (Player p in player) 
            for(int i=0;i<TotalPlayerNum; i++)
            {
                Player p = player[i];
                try
                {
                    if (p == null) continue;
                    int idx = p.GetPlayerIndex();
                    jointPen.Color = p.GetPlayerColor();

                    JointPointGroup jpGroup = p.GetJointPointGroup();

                    e.Graphics.DrawEllipse(jointPen, jpGroup.Head.X, jpGroup.Head.Y, EllipseSize, EllipseSize);
                    e.Graphics.DrawEllipse(jointPen, jpGroup.ShoulderCenter.X, jpGroup.ShoulderCenter.Y, EllipseSize, EllipseSize);
                    e.Graphics.DrawEllipse(jointPen, jpGroup.ShoulderLeft.X, jpGroup.ShoulderLeft.Y, EllipseSize, EllipseSize);
                    e.Graphics.DrawEllipse(jointPen, jpGroup.ShoulderRight.X, jpGroup.ShoulderRight.Y, EllipseSize, EllipseSize);
                    e.Graphics.DrawEllipse(jointPen, jpGroup.Spine.X, jpGroup.Spine.Y, EllipseSize, EllipseSize);
                    e.Graphics.DrawEllipse(jointPen, jpGroup.ElbowLeft.X, jpGroup.ElbowLeft.Y, EllipseSize, EllipseSize);
                    e.Graphics.DrawEllipse(jointPen, jpGroup.ElbowRight.X, jpGroup.ElbowRight.Y, EllipseSize, EllipseSize);
                    e.Graphics.DrawEllipse(jointPen, jpGroup.WristLeft.X, jpGroup.WristLeft.Y, EllipseSize, EllipseSize);
                    e.Graphics.DrawEllipse(jointPen, jpGroup.WristRight.X, jpGroup.WristRight.Y, EllipseSize, EllipseSize);
                }
                catch (Exception ee) { ee.ToString(); }
                

            }

            

        
        }



    }
}
