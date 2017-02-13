using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

using BasicApp.Basic;

namespace BasicApp.SpriteEngine
{
    class SpriteWindow : BasicWindow
    {
        public static int spriteDimensions = 40;
        List<CaveWall> wallList = new List<CaveWall>();
        List<CaveWall> wallDump = new List<CaveWall>();
        Player player;
        Random random = new Random();
        int counter = 0;
        Timer timer = null;
        long lastTimeStamp = 0;
        float dimensionalScaleFactor = 5.0F;       // Pixels per meter.
        float gravity = 9.8F;                       // Gravity is 9.8 meters per second per second.
        float thrust = 0F;
        int upperWall = 0;
        int lowerWall = 350;
        int upOrDown = 0;
        bool start = false;
        bool gameStop = false;
        int score = 0;
        string sScore;
        Font scoreFont = new Font(FontFamily.GenericSansSerif, 15);


        public SpriteWindow()
        {
            // First create the timer
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = (50) * (1);
            timer.Enabled = true;
            timer.Start();

            DateTime theDateTime = DateTime.Now;
            long theCurrentTimeStamp = theDateTime.Ticks;
            lastTimeStamp = theCurrentTimeStamp;
        }

        public override void DrawContent(EventArgs eventargs, Graphics theGraphics)
        {
            //Draws each of the buttons
            foreach (CaveWall theWall in wallList)
            {
                if (!theWall.visible)
                {
                    theWall.currentBackgroundColor = Color.White;
                    theWall.DrawSelf(theGraphics);
                }
            }

            //Draws each of the buttons
            foreach (CaveWall theWall in wallList)
            {
                if (theWall.visible)
                    theWall.DrawSelf(theGraphics);
            }

            if (player != null)
                player.DrawSelf(theGraphics);

            //Draws the current score
//            drawScore(theGraphics);

        }

        public void drawScore(Graphics theGraphics)
        {
            sScore = "Score : " + score.ToString();
            SolidBrush tempBrush = new SolidBrush(Color.Black);
            theGraphics.DrawString(sScore, scoreFont, tempBrush, 10, 10);
            tempBrush.Dispose();
        }

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            var mouseX = e.X;
            var mouseY = e.Y;
            if (!start)
            {
                Player newPlayer = new Player();
                newPlayer.Initializer(mouseX, mouseY);
                player = newPlayer;
                record.Invalidate();
                thrust = 9.8F;
            }
        }

        public override void OnMouseUp(object sender, MouseEventArgs e)
        {
            start = true;
        }

        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!start)
            {
                Player newPlayer = new Player();
                newPlayer.Initializer(150, 120);
                player = newPlayer;
                record.Invalidate();
                start = true;
            }
            else if (e.KeyCode == Keys.Up)
                thrust = 19.6F;
        }

        public override void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                thrust = 0F;
        }


        void timer_Tick(object Sender, EventArgs e)
        {

            DateTime theDateTime = DateTime.Now;
            long theCurrentTimeStamp = theDateTime.Ticks;
            long timeInterval = (theCurrentTimeStamp - lastTimeStamp);
            float theRealTimeInterval = ((float)Convert.ToDecimal(timeInterval)) / (10000000.0F);
            lastTimeStamp = theCurrentTimeStamp;

            bool shouldInvalidate = false;

            int threshold = 400;

            if (gameStop) return;

            if (player != null)
                if (player.visible && player.isFalling)
                {
                    //  int offset = 2;
                    float deltaVelocity = (gravity - thrust) * theRealTimeInterval;
                    player.velocity = player.velocity + deltaVelocity;
                    float offset = dimensionalScaleFactor * player.velocity * theRealTimeInterval;
                    player.newcoordY = player.coordY + Convert.ToInt32(offset);
                    if (player.newcoordY > threshold)
                    {
                        player.isFalling = false;
                        player.newcoordY = player.coordY;
                    }
                    else
                    {
                        shouldInvalidate = true;
                    }
                }

            if (start)
            {
                foreach (CaveWall theWall in wallList)
                {
                    if (theWall.visible)
                    {
                        float offset = -5F;
                        theWall.newcoordX = theWall.coordX + Convert.ToInt32(offset);
                    }

                    if (theWall.newcoordX < -40)
                        wallDump.Add(theWall);
                }

                if (counter == 3)
                {
                    upOrDown = random.Next(2);
                    if (upperWall <= spriteDimensions)
                        upOrDown = 0;
                    else if (upperWall >= lowerWall - 100)
                        upOrDown = 1;
                    if (upOrDown == 0)
                        upperWall += random.Next(spriteDimensions);
                    else
                        upperWall -= random.Next(spriteDimensions);

                    CaveWall ceiling = new CaveWall(false);
                    ceiling.Initializer(400, upperWall);
                    wallList.Add(ceiling);

                    upOrDown = random.Next(2);
                    if (lowerWall <= upperWall + 100)
                        upOrDown = 1;
                    else if (lowerWall >= 260)
                        upOrDown = 0;
                    if (upOrDown == 0)
                        lowerWall -= random.Next(spriteDimensions);
                    else
                        lowerWall += random.Next(spriteDimensions);

                    CaveWall floor = new CaveWall(true);
                    floor.Initializer(400, lowerWall);
                    wallList.Add(floor);

                    record.Invalidate();
                    counter = 0;
                }
                else if (start)
                {
                    counter++;
                    score++;
                }

                foreach (CaveWall checkWall in wallList)
                {
                    if (checkWall.CheckCollision(player))
                    {
                        player.currentBackgroundColor = Color.Yellow;
                        record.Invalidate();
                        gameStop = true;
                    }
                }

                foreach (CaveWall targetWall in wallDump)
                {
                    wallList.Remove(targetWall);
                }

                wallDump.Clear();

                if (shouldInvalidate)
                    record.Invalidate();
            }

       }
}



    public class Player
    {
        public int coordX;
        public int coordY;
        public int targetY;
        public int targetX;
        public int height = SpriteWindow.spriteDimensions;
        public int width = SpriteWindow.spriteDimensions;
        public Rectangle final;
        public Color currentBackgroundColor = Color.Brown;
        public bool visible = true;
        public bool isFalling = true;
        public float velocity = 0.0F;      // Units are meters per second.
        public bool player = false;

        public int newcoordX;
        public int newcoordY;

        public enum RectangleCorners
        {
            None = 0, TopLeft = 1, TopRight = 2, BottomLeft = 4, BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        public RectangleCorners corners;

        //Updates the values for this SpriteButton by comparing against the last button created
        public void Initializer(int targetX, int targetY)
        {
            coordX = targetX;
            coordY = targetY;
            newcoordX = coordX;
            newcoordY = coordY;
            return;
        }

        public void DrawSelf(Graphics theGraphics)
        {

            if (isFalling)
            {
                final = new Rectangle(coordX, coordY, width, height);
                SolidBrush tempBrush = new SolidBrush(Color.White);
                theGraphics.FillRectangle(tempBrush, final);
                tempBrush.Dispose();

                final = new Rectangle(newcoordX, newcoordY, width, height);
                tempBrush = new SolidBrush(currentBackgroundColor);
                theGraphics.FillRectangle(tempBrush, final);
                tempBrush.Dispose();
                coordX = newcoordX;
                coordY = newcoordY;

            }
            else
            {
                final = new Rectangle(coordX, coordY, width, height);
                SolidBrush tempBrush = new SolidBrush(currentBackgroundColor);
                theGraphics.FillRectangle(tempBrush, final);
                tempBrush.Dispose();
            }

        }

        public bool CheckCollision(int wallX, int wallY)
        {
            if ((wallX < coordX + 40 && wallX > coordX && wallY < coordY + 40 && wallY > coordY)
                || (wallX + 40 < coordX + 40 && wallX + 40 > coordX && wallY < coordY + 40 && wallY > coordY))
            {
                return false;
            }
            else
                return true;
        }

        
    }

    public class CaveWall
    {
        public int coordX;
        public int coordY;
        public int targetY;
        public int targetX;
        public int height = SpriteWindow.spriteDimensions;
        public int width = SpriteWindow.spriteDimensions;
        public Rectangle final;
        public Color currentBackgroundColor = Color.Brown;
        public bool visible = true;
        public bool isFalling = true;
        public float velocity = 0.0F;      // Units are meters per second.
        public bool player = false;

        public int newcoordX;
        public int newcoordY;

        public bool isFloor = false;

        public CaveWall(bool isItAFloor)
        {
            isFloor = isItAFloor;
        }

        public enum RectangleCorners
        {
            None = 0, TopLeft = 1, TopRight = 2, BottomLeft = 4, BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        public RectangleCorners corners;

        //Updates the values for this SpriteButton by comparing against the last button created
        public void Initializer(int targetX, int targetY)
        {
            coordX = targetX;
            if (!isFloor)
            {
                coordY = 0;
                height = targetY;
            }
            else
            {
                coordY = targetY;
                height = 400 - targetY;
            }

            newcoordX = coordX;
            newcoordY = coordY;
        }

        public void DrawSelf(Graphics theGraphics)
        {

            if (isFalling)
            {
                final = new Rectangle(coordX, coordY, width, height);
                SolidBrush tempBrush = new SolidBrush(Color.White);
                theGraphics.FillRectangle(tempBrush, final);
                tempBrush.Dispose();

                final = new Rectangle(newcoordX, newcoordY, width, height);
                tempBrush = new SolidBrush(currentBackgroundColor);
                theGraphics.FillRectangle(tempBrush, final);
                tempBrush.Dispose();
                coordX = newcoordX;
                coordY = newcoordY;

            }
            else
            {
                final = new Rectangle(coordX, coordY, width, height); 
                SolidBrush tempBrush = new SolidBrush(currentBackgroundColor);
                theGraphics.FillRectangle(tempBrush, final);
                tempBrush.Dispose();
            }

        }

        public bool CheckCollision(Player thePlayer)
        {
            int wallLeft = coordX;
            int wallTop = coordY;
            int wallRight = coordX + width;
            int wallBottom = coordY + height;

            int playerLeft = thePlayer.coordX;
            int playerTop = thePlayer.coordY;
            int playerRight = playerLeft + thePlayer.width;
            int playerBottom = playerTop + thePlayer.height;

            int maxLeft = Math.Max(wallLeft, playerLeft);
            int minRight = Math.Min(wallRight, playerRight);
            int maxTop = Math.Max(wallTop, playerTop);
            int minBottom = Math.Min(wallBottom, playerBottom);

            if (minRight < maxLeft)
            {
                return false;
            }

            if (minBottom < maxTop)
            {
                return false;
            }

            return true;
        }




    }
}
