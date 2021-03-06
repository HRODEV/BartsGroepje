﻿using System;
using System.Windows.Forms;

namespace StartUpApplication
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void startSimulationButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.Environment.CurrentDirectory + "/RealtimeDataFSharp.exe");
        }

        private void RideHourDayButton_Click(object sender, EventArgs e)
        {
            Ride_Hour_Day_Chart_Gen.GetForm().TopMost = false;
        }

        private void Total_Ride_Lines_Chart_Gen_Click(object sender, EventArgs e)
        {
            Total_Ride_Lines_Chart_Gen.GetForm().TopMost = false;
        }

        private void StopsPerStationButton_Click(object sender, EventArgs e)
        {
            Total_Stops_Per_Station_Chart_Gen.GetForm().TopMost = false;
            Total_Stops_Per_Station_Last5_Chart_Gen.GetForm().TopMost = false;
        }

        private void StationsPerLineButton_Click(object sender, EventArgs e)
        {
            Line_Has_Stations_Chart_Gen.GetForm().TopMost = false;
        }

        private void StopsPerDayButton_Click(object sender, EventArgs e)
        {
            Rides_All_Days_Chart_Gen.GetForm().TopMost = false;
        }

        private void GeneratePdf_Click(object sender, EventArgs e)
        {
            var file = Generate_Pdf.GeneratePdf();
            System.Diagnostics.Process.Start(file);

        }
    }
}