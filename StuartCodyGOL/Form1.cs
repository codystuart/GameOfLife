using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace StuartCodyGOL
{
    public partial class Form1 : Form
    {
        
        // The universe array
        bool[,] universe = new bool[10, 10];
        bool[,] scratchPad = new bool[10, 10];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int living = 0;

        //store universe height/width
        int gridHeight = 0;
        int gridWidth = 0;
        int seed = 0;

        //provide bools here for checkboxes
        bool neighborsCheckBox = true;
        bool gridCheckBox = true;
        bool neighborCntShow = true;
        bool gridShow = true;
        bool toroidalCount = true;
        bool finiteCount = false;
        bool hudShow = true;

        //StreamWriter strW = new StreamWriter("seed.txt");
        

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            //loop through 2d array left->right, and then top->bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //initialize count outside of if so it is within scope
                    int count = 0;

                    //get the neighbor count and assign it to a variable
                    if (toroidalCount)
                    {
                        count = CountNeighborsToroidal(x, y);
                    }
                    else if (finiteCount)
                    {
                        count = CountNeighborsFinite(x, y);
                    }

                    //apply rules                
                    //check if cell is alive if it is make sure it has enough neighbors to live
                    //this if check if the current cell (array index) is set to true or alive
                    if (universe[x,y])
                    {
                        //check if there are less than two living neighbors, if so cell will be dead in next generation
                        if(count < 2)
                        {
                            scratchPad[x, y] = false;
                        }
                        //check if there are more than 3 living neighbors, if so cell will be dead in the next generation
                        else if (count > 3)
                        {
                            scratchPad[x, y] = false;
                        }
                        //check if the numbers of living numbers is equal to 2 or 3 if so the cell will live in the next generation
                        else if(count == 2 || count == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                    //using else here since the only other option to check after the rules for living cells is dead cells
                    else
                    {
                        //if a dead cell is exactly 3 living neighbors it will then be alive in the next generation
                        if(count == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                        //dead cells with anything other than 3 neighbors will not be alive in the next generation
                        else if (count != 3)
                        {
                            scratchPad[x, y] = false;
                        }
                    }
                }
            }

            //copy what is in the scathcpad to the universe
            //SwapBools(universe, scratchPad);
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;


            // Increment generation count
            generations++;

            // Get count of living cells
            living = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x,y] == true)
                    {
                        living++;
                    }
                }
            }

            

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            // Update status strip Living Cells
            toolStripStatusLabelLiving.Text = "Living = " + living.ToString();

            //force the form to be repainted
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    //Make sure bool is true for showing grid before drawing
                    if (gridShow)
                    {
                        // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                    //Make sure bool is true for showing neighbor counts before drawing
                    if (neighborCntShow)
                    {
                        //The following writes the neighbor count to the center of the cell
                        Font font = new Font("Arial", 20f);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        //initialize neighbors outside of if so it isnt out of scope
                        int neighbors = 0;

                        if (toroidalCount) // = true
                        {
                            neighbors = CountNeighborsToroidal(x, y);
                        }
                        else if (finiteCount) // = true
                        {
                            neighbors = CountNeighborsFinite(x, y);
                        }

                        //only draw neighbor count if there are greater than zero neighbors
                        if (neighbors != 0)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Black, cellRect, stringFormat);
                        }
                        //End of writing neighbor count to the center of cells
                    if (hudShow)
                        {
                            font = new Font("Arial", 12f);
                            //The following draws the HUD
                            //stringFormat.Alignment = StringAlignment.Far;
                            //stringFormat.LineAlignment = StringAlignment.Near;

                            //determine how we are counting neighbors
                            string countType = string.Empty;
                            if (toroidalCount)
                            {
                                countType = "Toroidal";
                            }
                            else
                            {
                                countType = "Finite";
                            }

                            //get height and width
                            gridWidth = universe.GetLength(0);
                            gridHeight = universe.GetLength(1);

                            PointF hudPf = new PointF();
                            e.Graphics.DrawString("Generation: " + generations + "\nLiving: " + living + "\nBoundary: " + countType + "\nUniverse Size: " + gridHeight + ", " + gridWidth, font, Brushes.DarkRed, hudPf);
                        }


                    }
                }
            }

            // Update status strip timer interval
            toolStripStatusLabelTimer.Text = "Timer = " + timer.Interval.ToString();

            // Update Height/Width status strips
            toolStripStatusLabelHeight.Text = "Grid Height = " + universe.GetLength(1).ToString();
            toolStripStatusLabelWidth.Text = "Grid Width = " + universe.GetLength(0).ToString();

            // Update seed status strip
            toolStripStatusLabelSeed.Text = "Seed = " + seed.ToString();

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    { continue; }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    { continue; }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    { continue; }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    { continue; }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if(xOffset == 0 && yOffset == 0)
                    { continue; }
                    // if xCheck is less than 0 then set to xLen - 1
                    if(xCheck < 0)
                    { 
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    { 
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    { 
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //enable the timer so that the generations run continuosly
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //disable the timer so that the user must advance the generations manually, this is how we pause the game
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //call the NextGeneration method so we advance only once when the next button is clicked
            NextGeneration();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            //set status tool strip items back to 0
            generations = 0;
            living = 0;
            timer.Interval = 100;

            //Update status strips to defaults
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabelLiving.Text = "Living = " + living.ToString();
            toolStripStatusLabelTimer.Text = "Timer = " + timer.Interval.ToString();

            //stop the timer so generations stop counting
            timer.Enabled = false;

            //clear the arrays so we dont have false information in our new game
            Array.Clear(universe, 0, universe.Length);
            Array.Clear(scratchPad, 0, scratchPad.Length);

            //Reset Colors
            graphicsPanel1.BackColor = Color.White;
            cellColor = Color.Gray;
            gridColor = Color.Black;

            //force the form to be repainted
            graphicsPanel1.Invalidate();

        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new a color dialog object so we can utilize it
            ColorDialog clrDlg = new ColorDialog();

            //get current background color
            Color bgColor = graphicsPanel1.BackColor;
            clrDlg.Color = bgColor;

            //use the following to display the color picker dialog
            if (DialogResult.OK == clrDlg.ShowDialog())
            {
                graphicsPanel1.BackColor = clrDlg.Color;

                //Force windows to repaint
                graphicsPanel1.Invalidate();
            }

            
        }

        private void cellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new a color dialog object so we can utilize it
            ColorDialog clrDlg = new ColorDialog();

            //get current cell color
            clrDlg.Color = cellColor;

            //use the following to actually display the color picker dialog
            if (DialogResult.OK == clrDlg.ShowDialog())
            {
                //set cellColor to user's choice
                cellColor = clrDlg.Color;

                //Force windows to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new a color dialog object so we can utilize it
            ColorDialog clrDlg = new ColorDialog();

            clrDlg.Color = gridColor;

            //use the following to actually display the color picker dialog
            if (DialogResult.OK == clrDlg.ShowDialog())
            {
                gridColor = clrDlg.Color;
                
                //Force windows to repaint
                graphicsPanel1.Invalidate();
            }


        }

        private void backgroundToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //call the following method which does exactly what we need here so we arent writing code twice
            backgroundToolStripMenuItem_Click(sender, e);
        }

        private void cellToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //call the following method which does exactly what we need here so we arent writing code twice
            cellToolStripMenuItem_Click(sender, e);
        }

        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //call the following method which does exactly what we need here so we arent writing code twice
            gridToolStripMenuItem_Click(sender, e);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //instantiate an object to utilize modal dialog class
            ModalDialog dlg = new ModalDialog();

            //get timer setting for Settings dialog
            dlg.SetTimerInterval(timer.Interval);

            //Set height and width using array length
            gridHeight = universe.GetLength(1);
            gridWidth = universe.GetLength(0);

            //Set Dialog box default values to current Height and Width
            dlg.GridHeight = gridHeight;
            dlg.GridWidth = gridWidth;

            if(DialogResult.OK == dlg.ShowDialog())
            {
                //Update Status strip for Timer
                timer.Interval = dlg.GetTimerInterval();
                toolStripStatusLabelTimer.Text = "Timer = " + timer.Interval.ToString();

                //update grid height/width
                gridHeight = dlg.GridHeight;
                gridWidth = dlg.GridWidth;

                //create new temp arrays to overwrite and resize 
                bool[,] universeTemp = new bool[gridWidth, gridHeight];
                bool[,] scratchPadTemp = new bool[gridWidth, gridHeight];

                //set temps = actual arrays so they are resized
                universe = universeTemp;
                scratchPad = scratchPadTemp;

                //update status strip with new height/width
                toolStripStatusLabelHeight.Text = "Grid Height = " + universe.GetLength(1).ToString();
                toolStripStatusLabelWidth.Text = "Grid Width = " + universe.GetLength(0).ToString();

                //Force Windows to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //instantiate Seed dialog object so we can use the class
            SeedDialog sdlg = new SeedDialog();

            //get current seed
            sdlg.SetSeed(seed);


            if(DialogResult.OK == sdlg.ShowDialog())
            {
                //get current set seed
                seed = sdlg.GetSeed();
                //set current set speed to counter in Dialog
                sdlg.SetSeed(seed);

                //Create our random number object and tie it to our seed
                Random rand = new Random(seed);

                //iterate through the array and generate a number from 0-2 (inclusive), if it is 0 make the cell alive, otherwise it stays dead
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        int randFromSeed = rand.Next(0, 3);

                        //if randomly generated number is equal to zero cell is alive
                        if (randFromSeed == 0)
                        {
                            universe[x, y] = true;
                        }
                        else
                        {
                            universe[x, y] = false;
                        }
                    }
                }

                //get living cells count
                living = 0;
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                        {
                            living++;
                        }
                    }
                }

                //update living cells count status strip
                toolStripStatusLabelLiving.Text = "Living = " + living.ToString();

                //Force windows to repaint form
                graphicsPanel1.Invalidate();
            }
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get random numbers using time
            Random randTime = new Random((int)DateTime.Now.Ticks);


            //iterate through the array and generate a number from 0-2 (inclusive), if it is 0 make the cell alive, otherwise it stays dead
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int randFromTime = randTime.Next(0, 3);

                    if (randFromTime == 0)
                    {
                        universe[x, y] = true;
                    }
                    else
                    {
                        universe[x, y] = false;
                    }
                }
            }

            //get living cells count
            living = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        living++;
                    }
                }
            }

            //update living cells count status strip
            toolStripStatusLabelLiving.Text = "Living = " + living.ToString();

            //Force windows to repaint form
            graphicsPanel1.Invalidate();
        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hudShow = !hudShow;
            if(hudShow)
            {
                hUDToolStripMenuItem1.Checked = true;
                hUDToolStripMenuItem1.CheckState = CheckState.Checked;
            }
            else
            {
                hUDToolStripMenuItem1.Checked = false;
                hUDToolStripMenuItem1.CheckState = CheckState.Unchecked;
            }
            graphicsPanel1.Invalidate();
        }

        private void hUDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            hudShow = !hudShow;
            if (hudShow)
            {
                hUDToolStripMenuItem.Checked = true;
                hUDToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                hUDToolStripMenuItem.Checked = false;
                hUDToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //flip checkbox setting
            neighborsCheckBox = !neighborsCheckBox;

            //we want to make sure context menu matches the tool strip menu
            if (neighborsCheckBox == true)
            {
                //we want to make sure context menu matches the tool strip menu
                neighborCountToolStripMenuItem1.Checked = true;
                neighborCountToolStripMenuItem1.CheckState = CheckState.Checked;
            }
            else
            {
                neighborCountToolStripMenuItem1.Checked = false;
                neighborCountToolStripMenuItem1.CheckState = CheckState.Unchecked;
            }

            //toggle if neighbor count is showing
            neighborCntShow = !neighborCntShow;

            //force windows to repaint
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //flip checkbox setting
            gridCheckBox = !gridCheckBox;

            //we want to make sure context menu matches the tool strip menu
            if (gridCheckBox == true)
            {
                gridToolStripMenuItem3.Checked = true;
                gridToolStripMenuItem3.CheckState = CheckState.Checked;
            }
            else
            {
                gridToolStripMenuItem3.Checked = false;
                gridToolStripMenuItem3.CheckState = CheckState.Unchecked;
            }

            //toggle if grid is showing 
            gridShow = !gridShow;

            //force windows to repaint
            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //flip checkbox setting
            neighborsCheckBox = !neighborsCheckBox;

            //we want to make sure context menu matches the tool strip menu
            if (neighborsCheckBox == true)
            {
                neighborCountToolStripMenuItem.Checked = true;
                neighborCountToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked = false;
                neighborCountToolStripMenuItem.CheckState = CheckState.Unchecked;
            }

            //toggle if showing neighbor count or not
            neighborCntShow = !neighborCntShow;

            //force windows to repaint
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //flip checkbox setting
            gridCheckBox = !gridCheckBox;

            //we want to make sure context menu matches the tool strip menu
            if (gridCheckBox == true)
            {
                gridToolStripMenuItem2.Checked = true;
                gridToolStripMenuItem2.CheckState = CheckState.Checked;
            }
            else
            {
                gridToolStripMenuItem2.Checked = false;
                gridToolStripMenuItem2.CheckState = CheckState.Unchecked;
            }

            //toggle if showing grid or not
            gridShow = !gridShow;

            //force windows to repaint
            graphicsPanel1.Invalidate();
        }

        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //flip checkbox setting
            toroidalCount = !toroidalCount;

            //Check if toroidal is true or false and set finite to the opposite
            if (toroidalCount == true)
            {
                finiteToolStripMenuItem.Checked = false;
                finiteToolStripMenuItem.CheckState = CheckState.Unchecked;
                finiteCount = false;
            }
            else
            {
                finiteToolStripMenuItem.Checked = true;
                finiteToolStripMenuItem.CheckState = CheckState.Checked;
                finiteCount = true;
            }

            //force windows to repaint
            graphicsPanel1.Invalidate();
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //flip checkbox setting
            finiteCount = !finiteCount;

            //check if finite is true or false and set toroidal to the opposite
            if (finiteCount == true)
            {
                toroidalToolStripMenuItem.Checked = false;
                toroidalToolStripMenuItem.CheckState = CheckState.Unchecked;
                toroidalCount = false;
            }
            else
            {
                toroidalToolStripMenuItem.Checked = true;
                toroidalToolStripMenuItem.CheckState = CheckState.Checked;
                finiteCount = true;
            }

            //force windows to repaint
            graphicsPanel1.Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All Files|*.*|Cells|*.cells";
            sfd.FilterIndex = 2; sfd.DefaultExt = "cells";

            if (DialogResult.OK == sfd.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(sfd.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!Name: Cells");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    //Create a string to represent the current row.
                    String currentRow = string.Empty;

                    //Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x,y])
                        {
                            writer.Write(currentRow + 'O');

                        }
                        else if (!universe[x,y])
                        {
                            writer.Write(currentRow + '.');
                        }
                    }
                    writer.WriteLine();
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            //Call this method because we dont want to write the same code twice
            saveToolStripMenuItem_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*|Cells|*.cells";
            ofd.FilterIndex = 2;

            if (DialogResult.OK == ofd.ShowDialog())
            {
                StreamReader reader = new StreamReader(ofd.FileName);

                //create variables to calculate width and height of the data in the file
                int MaxWidth = 0;
                int MaxHeight = 0;

                //Iterate through the file once to get its size
                while (!reader.EndOfStream)
                {
                    //read one row at a time
                    string row = reader.ReadLine();

                    //if the row begins with a ! then it should be ignored
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    MaxHeight++;
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    MaxWidth = row.Length;
                }

                //resize universe, and scratchPad arrays
                universe = new bool[MaxWidth, MaxHeight];
                scratchPad = new bool[MaxWidth, MaxHeight];

                //reset the file pointer back to the beginning of the file
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                int yPos = 0;
                //iterate through the file again, this time reading in the cells
                while (!reader.EndOfStream)
                {
                    //read one row at a time
                    String row = reader.ReadLine();

                    //if the row begins with a !then it should be ignored
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        if (row[xPos] == 'O')
                        {
                            //cell is alive
                            universe[xPos,yPos] = true;
                        }
                        else
                        {
                            //cell is dead
                            universe[xPos,yPos] = false;

                        }
                        
                    }
                    yPos++;

                }
                reader.Close();
                
            }

            //force windows to repaint
            graphicsPanel1.Invalidate();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            //call the following method because we don't want to write duplicate code
            openToolStripMenuItem_Click(sender, e);
        }


    }
}
