using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StuartCodyGOL
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[5, 5];
        bool[,] scratchPad = new bool[5, 5];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;



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
                    //get the neighbor count and assign it to a variable for use later
                    int count = CountNeighborsToroidal(x, y);

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
                        else if(count ==2 || count == 3)
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


                    //Turn on or off in the scratchpad
                    //scratchPad[x, y] = !scratchPad[x,y];
                }
            }

            //copy what is in the scathcpad to the universe
            //SwapBools(universe, scratchPad);
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;


            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

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

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    //The following writes the neighbor count to the center of the cell
                    Font font = new Font("Arial", 20f);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    int neighbors = CountNeighborsToroidal(x,y);

                    if (neighbors != 0)
                    {
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Black, cellRect, stringFormat);
                    }
                    //End of writing neighbor count to the center of cells
                }
            }



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

        private void graphicsPanel1_Click(object sender, EventArgs e)
        {

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
                    if (yCheck >= 0)
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

        private void SwapBools(bool[,] bool1, bool[,] bool2)
        {
            
            bool[,] temp = bool1;
            bool1 = bool2;
            bool2 = temp;
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
            //set generations counter back to 0
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            //stop the timer so generations stop counting
            timer.Enabled=false;

            //clear the arrays so we dont have false information in our new game
            Array.Clear(universe, 0, universe.Length);
            Array.Clear(scratchPad, 0, scratchPad.Length);

            //force the form to be repainted
            graphicsPanel1.Invalidate();

        }

    }
}
