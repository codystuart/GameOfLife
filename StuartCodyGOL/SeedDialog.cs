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
    public partial class SeedDialog : Form
    {
        public SeedDialog()
        {
            InitializeComponent();
        }

        public int GetSeed()
        {
            return (int)numericUpDownSeed.Value;
        }

        public void SetSeed(int seed) 
        {
            numericUpDownSeed.Value = seed;
        }
    }
}
