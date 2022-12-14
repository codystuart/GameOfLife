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

        //get and set Height of Grid
        public int GridHeight
        {
            get
            {
                return (int)numericUpDownGridHeight.Value;
            }
            set 
            {
                numericUpDownGridHeight.Value = value;
            }
        }

        //Get and set Width of grid
        public int GridWidth
        {
            get
            {
                return(int)numericUpDownGridWidth.Value;
            }
            set
            {
                numericUpDownGridWidth.Value = value;
            }
        }

        //get and set Timer Value
        public int GetTimerInterval()
        {
            return (int)numericUpDown1.Value;
        }

        public void SetTimerInterval(int interval)
        {
            numericUpDown1.Value = interval;
        }
    }
}
