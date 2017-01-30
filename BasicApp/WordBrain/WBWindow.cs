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

namespace BasicApp.WordBrain
{
    public class WBWindow   : BasicWindow
    {
        int gridSize = 3;
        List<WBButton> buttonList = new List<WBButton>();
        string firstWord = "s p i c e";
        string secondWord = "f o n t";
        Random random = new Random();
        List<string[]> puzzles = new List<string[]>();
        List<WBButton> inputs = new List<WBButton>();
        int currentPuzzle = 0;
        bool mouseIsHeld;
        Timer timer = null;

        public WBWindow()
        {
            // First create the timer
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = (50) * (1);
            timer.Enabled = true;
            timer.Start();

            //Creates the initial button
            WBButton currentButton = new WBButton();
            currentButton.Initializer(null, gridSize);

            buttonList.Add(currentButton);

            //Creates the remaining buttons
            for (int i = 0; i < gridSize * gridSize - 1; i++)
            {
                currentButton = new WBButton();
                currentButton.Initializer(buttonList[i], gridSize);
                buttonList.Add(currentButton);
            }

            //Finds adjacent buttons for each button
            foreach (WBButton theButton in buttonList)
            {
                theButton.FindAdjacent(buttonList);
            }

            //Puzzle generation, and quality check
            string[] puzzleA = firstWord.Split(' ');
            string[] puzzleB = secondWord.Split(' ');
            puzzles.Add(puzzleA);
            puzzles.Add(puzzleB);

            bool ok = false;
            bool checka = false;
            bool checkb = false;
            //int safety = 0;

            while (!ok)
            {
                for (int i = 0; i < puzzleA.Length; i++)
                {
                    ok = false;
                    while (!ok)
                    {
                        ok = buttonList[random.Next(9)].ConstructPuzzle(i, puzzleA);
                    }
                }

                for (int i = 0; i < puzzleB.Length; i++)
                {
                    ok = false;
                    while (!ok)
                    {
                        ok = buttonList[random.Next(9)].ConstructPuzzle(i, puzzleB);
                    }
                }

                ok = false;

                foreach (WBButton theButton in buttonList)
                {
                    if (theButton.content == puzzleA[0])
                        if (theButton.PuzzleCheck(0, puzzleA))
                            checka = true;
                    if (theButton.content == puzzleB[0])
                        if (theButton.PuzzleCheck(0, puzzleB))
                            checkb = true;
                }
                //safety++;
                if (checka && checkb)
                    ok = true;
                else
                {
                    foreach (WBButton theButton in buttonList)
                        theButton.Reset();
                    checka = false;
                    checkb = false;
                }

            }


        }

        public override void DrawContent(EventArgs eventargs, Graphics theGraphics)
        {
#if false
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            RectangleF theClipRectangle = theGraphics.VisibleClipBounds;
//            theGraphics.FillRectangle(whiteBrush, theClipRectangle);
            whiteBrush.Dispose();
#endif

            //Draws each of the buttons
            foreach(WBButton theButton in buttonList)
            {
                if(!theButton.visible)
                {
                    theButton.currentBackgroundColor = Color.White;
                    theButton.DrawSelf(theGraphics);
                }
            }

            //Draws each of the buttons
            foreach (WBButton theButton in buttonList)
            {
                if (theButton.visible)
                    theButton.DrawSelf(theGraphics);
            }

        }

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {

            var mouseX = e.X;
            var mouseY = e.Y;
            mouseIsHeld = true;

            // Checks which button was clicked, and if it's a valid move
            foreach (WBButton theButton in buttonList)
            {
                if (theButton.CheckMouse(mouseX, mouseY))
                {
                    if (theButton.visible && !theButton.selected && inputs.Count == 0)
                    {
                        theButton.currentBackgroundColor = Color.DarkMagenta;
                        theButton.selected = true;
                        inputs.Add(theButton);
                        record.Invalidate();
                    }
                    else if (theButton.visible && !theButton.selected && inputs[inputs.Count - 1].adjacents.Contains(theButton))
                    {
                        theButton.currentBackgroundColor = Color.DarkMagenta;
                        theButton.selected = true;
                        inputs.Add(theButton);
                        record.Invalidate();
                    }
                    //else
                    //    SystemSounds.Beep.Play();
                }
            }

            record.Invalidate();
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            var mouseX = e.X;
            var mouseY = e.Y;

            // Checks which button was clicked, and if it's a valid move
            if (mouseIsHeld)
            {
                foreach (WBButton theButton in buttonList)
                {
                    if (theButton.CheckMouse(mouseX, mouseY))
                    {
                        if (theButton.visible && !theButton.selected && inputs.Count == 0)
                        {
                            theButton.currentBackgroundColor = Color.DarkMagenta;
                            theButton.selected = true;
                            inputs.Add(theButton);  
                            record.Invalidate();
                        }
                        else if (theButton.visible && !theButton.selected && inputs[inputs.Count - 1].adjacents.Contains(theButton))
                        {
                            theButton.currentBackgroundColor = Color.DarkMagenta;
                            theButton.selected = true;
                            inputs.Add(theButton); 
                            record.Invalidate();
                        }
                        //else
                        //    SystemSounds.Beep.Play();
                    }
                }
            }
        }

        public override void OnMouseUp(object sender, MouseEventArgs e)
        {
            mouseIsHeld = false;
            //Checks if the given answer was correct
            if (inputs.Count == puzzles[currentPuzzle].Length)
            {
                bool ok = false;
                for (int i = 0; i < inputs.Count; i++)
                {
                    if (inputs[i].content == puzzles[currentPuzzle][i])
                        ok = true;
                    else
                        ok = false;

                    if (!ok)
                        break;
                }

                if (ok)
                {
                    foreach (WBButton theButton in buttonList)
                    {
                        if (theButton.selected && theButton.visible)
                        {                                                 
                            theButton.visible = false;
                            theButton.selected = false;
                        }
                    }
                    currentPuzzle++;

                }
                else
                {
                    foreach (WBButton theButton in buttonList)
                    {
                        if (theButton.selected)
                        {
                            theButton.currentBackgroundColor = Color.Brown;
                            theButton.selected = false;
                        }
                    }
                }
            }
            else
            {
                foreach (WBButton theButton in buttonList)
                {
                    if (theButton.selected)
                    {
                        theButton.currentBackgroundColor = Color.Brown;
                        theButton.selected = false;
                    }

                }
            }

            inputs.Clear();

            record.Invalidate();
        }


        void timer_Tick(object Sender, EventArgs e)
        {
#if false
            foreach (WBButton theButton in buttonList)
            {
                foreach (WBButton checkedButton in theButton.adjacents)
                {
                    if (!checkedButton.visible && checkedButton.row == theButton.row - 1 && checkedButton.column == theButton.column)
                    {
                        if (theButton.coordY != checkedButton.coordY)
                        {
                            theButton.coordY += 1;
                            record.Invalidate();
                        }
                    }
                }
            }



            int maxY = 0;

            foreach (WBButton theButton in buttonList)
            {
                if(theButton.visible && theButton.isFalling)
                {
                    foreach (WBButton checkedButton in theButton.adjacents)
                    {
                        if (!checkedButton.visible && checkedButton.row == theButton.row + 1 && checkedButton.column == theButton.column)
                        {
                            maxY = checkedButton.coordY;
                            if (theButton.coordY < maxY)
                            {
                                theButton.isFalling = true;
                                record.Invalidate();
                            }
                        }
                    }
                }
            }
#endif

            bool shouldInvalidate = false;
            foreach (WBButton theButton in buttonList)
            {
                if (buttonList.IndexOf(theButton) < 6)
                    if (!buttonList[buttonList.IndexOf(theButton) + 3].visible)
                    {
                        theButton.row = buttonList[buttonList.IndexOf(theButton) + 3].row;
                        theButton.isFalling = true;
                    }
                    else if (buttonList[buttonList.IndexOf(theButton) + 3].coordY > theButton.row * 50 + 40)
                    {
                        theButton.row = buttonList[buttonList.IndexOf(theButton) + 3].row - 1;
                        theButton.isFalling = true;
                    }

                theButton.FindAdjacent(buttonList);

                int threshold = (theButton.row - 1) * 50 + 40;

                if (theButton.visible && theButton.isFalling)
                {
                    int offset = 2;
                    theButton.newcoordY = theButton.coordY + offset;
                    if(theButton.newcoordY > threshold + 1)
                    {
                        theButton.isFalling = false;
                        theButton.newcoordY = theButton.coordY;
                    }
                    else
                    {
                        theButton.newcontentY = theButton.newcoordY + 10;
                        shouldInvalidate = true;
                    }

                }
            }

            if (shouldInvalidate)
            {
                record.Invalidate();
            }                        

        }
    }

    public class WBButton
    {
        public int coordX;
        public int coordY; 
        public int targetY;
        public int height = 40;
        public int width = 40;
        public int column;
        public int row;
        public int rowLower;          
        public Rectangle final;
        public int contentX;
        public int contentY;
        public string content = null;
        public Font font = new Font(FontFamily.GenericSansSerif, 12);
        public Color currentBackgroundColor = Color.Brown;
        public List<WBButton> adjacents;
        public bool visible = true;
        public bool selected = false;
        public bool isFalling = false;

        public int newcoordX;
        public int newcoordY;
        public int newcontentX;
        public int newcontentY;

        public enum RectangleCorners
        {
            None = 0, TopLeft = 1, TopRight = 2, BottomLeft = 4, BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        public RectangleCorners corners;

        //Updates the values for this WBButton by comparing against the last button created
        public void Initializer (WBButton previousButton, int gridSize)
        {
            if(previousButton != null)
            {
                if (previousButton.column != gridSize)
                {
                    coordX = previousButton.coordX + 50;
                    row = previousButton.row;
                    column = previousButton.column + 1;
                }
                else
                {
                    coordX = 40;
                    row = previousButton.row + 1;
                    column = 1;
                }

                coordY = (row - 1) * 50 + 40;
            }
            else
            {
                coordX = 40;
                coordY = 40;
                row = 1;
                column = 1;
            }

            contentX = coordX + 13;
            contentY = coordY + 10;

            newcontentX = contentX;
            newcontentY = contentY;
            newcoordX = coordX;
            newcoordY = coordY;
            rowLower = row + 1;
            return;

        }

        public void DrawSelf(Graphics theGraphics)
        {

            if (isFalling)
            {
                final = new Rectangle(coordX, coordY, width, height);
                SolidBrush tempBrush = new SolidBrush(Color.White);
                theGraphics.FillRectangle(tempBrush, final);

                theGraphics.DrawString(content, font, tempBrush, contentX, contentY);
                tempBrush.Dispose();


                final = new Rectangle(newcoordX, newcoordY, width, height);
                tempBrush = new SolidBrush(currentBackgroundColor);
                theGraphics.FillRectangle(tempBrush, final);
                tempBrush.Dispose();

                if (visible)
                    tempBrush = new SolidBrush(Color.Aquamarine);
                else
                    tempBrush = new SolidBrush(Color.White);
                theGraphics.DrawString(content, font, tempBrush, newcontentX, newcontentY);
                tempBrush.Dispose();

                contentX = newcontentX;
                contentY = newcontentY;
                coordX = newcoordX;
                coordY = newcoordY;

            }
            else
            {
                final = new Rectangle(coordX, coordY, width, height);
                GraphicsPath finalVersion = WBButton.Create(final);
                SolidBrush tempBrush = new SolidBrush(currentBackgroundColor);
                //            theGraphics.FillPath(tempBrush, finalVersion);
                finalVersion.Dispose();
                theGraphics.FillRectangle(tempBrush, final);
                tempBrush.Dispose();

                if (visible)
                    tempBrush = new SolidBrush(Color.Aquamarine);
                else
                    tempBrush = new SolidBrush(Color.White);
                theGraphics.DrawString(content, font, tempBrush, contentX, contentY);
                tempBrush.Dispose();
            }

        }                  

        public bool CheckMouse (int mouseX, int mouseY)
        {
            if (coordX <= mouseX && mouseX <= coordX + width
                && coordY <= mouseY && mouseY <= coordY + height)
                return true;
            else
                return false;
        }

        public void FindAdjacent (List<WBButton> buttonList)
        {
            adjacents = new List<WBButton>();
            foreach(WBButton currentButton in buttonList)
            {
                if ((currentButton.column == column - 1 && currentButton.row == row - 1)
                    || (currentButton.column == column && currentButton.row == row - 1)
                    || (currentButton.column == column + 1 && currentButton.row == row - 1)
                    || (currentButton.column == column - 1 && currentButton.row == row)
                    || (currentButton.column == column + 1 && currentButton.row == row)
                    || (currentButton.column == column - 1 && currentButton.row == row + 1)
                    || (currentButton.column == column && currentButton.row == row + 1)
                    || (currentButton.column == column + 1 && currentButton.row == row + 1))
                    adjacents.Add(currentButton);
            }

        }

        public bool ConstructPuzzle (int currentChar, string[] puzzleWord)
        {
            if (content == null)
            {
                content = puzzleWord[currentChar];
                return true;
            }
            else
                return false;   
        }

        public bool PuzzleCheck (int currentChar, string[] puzzleWord)
        {
            bool ok = true;
            if (content != puzzleWord[currentChar])
                return false;
            else if (currentChar < puzzleWord.Length - 1)
            {
                foreach (WBButton nextButton in adjacents)
                {
                    ok = nextButton.PuzzleCheck(currentChar + 1, puzzleWord);
                    if (ok)
                        break;
                }
            }
            return ok;
        }

        public void Reset ()
        {
            content = null;
        }

        public static GraphicsPath Create(int x, int y, int width, int height,
                                          int radius, RectangleCorners corners)
        {
            int xw = x + width;
            int yh = y + height;
            int xwr = xw - radius;
            int yhr = yh - radius;
            int xr = x + radius;
            int yr = y + radius;
            int r2 = radius * 2;
            int xwr2 = xw - r2;
            int yhr2 = yh - r2;

            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            //Top Left Corner
            if ((RectangleCorners.TopLeft & corners) == RectangleCorners.TopLeft)
            {
                p.AddArc(x, y, r2, r2, 180, 90);
            }
            else
            {
                p.AddLine(x, yr, x, y);
                p.AddLine(x, y, xr, y);
            }

            //Top Edge
            p.AddLine(xr, y, xwr, y);

            //Top Right Corner
            if ((RectangleCorners.TopRight & corners) == RectangleCorners.TopRight)
            {
                p.AddArc(xwr2, y, r2, r2, 270, 90);
            }
            else
            {
                p.AddLine(xwr, y, xw, y);
                p.AddLine(xw, y, xw, yr);
            }

            //Right Edge
            p.AddLine(xw, yr, xw, yhr);

            //Bottom Right Corner
            if ((RectangleCorners.BottomRight & corners) == RectangleCorners.BottomRight)
            {
                p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            }
            else
            {
                p.AddLine(xw, yhr, xw, yh);
                p.AddLine(xw, yh, xwr, yh);
            }

            //Bottom Edge
            p.AddLine(xwr, yh, xr, yh);

            //Bottom Left Corner
            if ((RectangleCorners.BottomLeft & corners) == RectangleCorners.BottomLeft)
            {
                p.AddArc(x, yhr2, r2, r2, 90, 90);
            }
            else
            {
                p.AddLine(xr, yh, x, yh);
                p.AddLine(x, yh, x, yhr);
            }

            //Left Edge
            p.AddLine(x, yhr, x, yr);

            p.CloseFigure();
            return p;
        }

        public static GraphicsPath Create(Rectangle rect, int radius, RectangleCorners c)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, c); }

        public static GraphicsPath Create(int x, int y, int width, int height, int radius)
        { return Create(x, y, width, height, radius, RectangleCorners.All); }

        public static GraphicsPath Create(Rectangle rect, int radius)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius); }

        public static GraphicsPath Create(int x, int y, int width, int height)
        { return Create(x, y, width, height, 5); }

        public static GraphicsPath Create(Rectangle rect)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height); }
    }
}