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
    public partial class ModalDialog : Form
    {

        public ModalDialog()
        {
            InitializeComponent();
        }

        public int GetTimerInterval()
        { 
            return (int)numericUpDown1.Value; 
        }

        public int GetGridHeight()
        {
            return (int)numericUpDownGridHeight.Value;
        }
        public int GetGridWidth() 
        {
            return (int)numericUpDownGridWidth.Value;
        }

        public void SetTimerInterval(int interval) 
        {
            numericUpDown1.Value = interval;
        }

        public void SetGridHeight(int height) 
        {
            numericUpDownGridHeight.Value = height;
        }

        public void SetGridWidth(int gridWidth)
        {
            numericUpDownGridWidth.Value = gridWidth;
        }
    }
}
