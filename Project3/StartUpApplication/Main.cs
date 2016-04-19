using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartUpApplication
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private async void startSimulationButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.Environment.CurrentDirectory + "/RealtimeDataFSharp.exe");
        }

        private void RideHourDayButton_Click(object sender, EventArgs e)
        {
            Ride_Hour_Day_Chart_Gen.GetChart();
        }
    }
}
