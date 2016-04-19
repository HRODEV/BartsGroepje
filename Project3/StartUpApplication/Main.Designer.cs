﻿namespace StartUpApplication
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.startSimulationButton = new System.Windows.Forms.Button();
            this.RideHourDayButton = new System.Windows.Forms.Button();
            this.Total_Ride_Lines_Chart_GenButton = new System.Windows.Forms.Button();
            this.StopsPerStationButton = new System.Windows.Forms.Button();
            this.StationsPerLineButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::StartUpApplication.Properties.Resources.RETlogo;
            this.pictureBox1.Location = new System.Drawing.Point(109, 49);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(416, 200);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // startSimulationButton
            // 
            this.startSimulationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startSimulationButton.Location = new System.Drawing.Point(185, 275);
            this.startSimulationButton.Name = "startSimulationButton";
            this.startSimulationButton.Size = new System.Drawing.Size(266, 71);
            this.startSimulationButton.TabIndex = 1;
            this.startSimulationButton.Text = "start simulatie";
            this.startSimulationButton.UseVisualStyleBackColor = true;
            this.startSimulationButton.Click += new System.EventHandler(this.startSimulationButton_Click);
            // 
            // RideHourDayButton
            // 
            this.RideHourDayButton.Location = new System.Drawing.Point(36, 398);
            this.RideHourDayButton.Name = "RideHourDayButton";
            this.RideHourDayButton.Size = new System.Drawing.Size(116, 23);
            this.RideHourDayButton.TabIndex = 2;
            this.RideHourDayButton.Text = "ritten per uur en dag";
            this.RideHourDayButton.UseVisualStyleBackColor = true;
            this.RideHourDayButton.Click += new System.EventHandler(this.RideHourDayButton_Click);
            // 
            // Total_Ride_Lines_Chart_GenButton
            // 
            this.Total_Ride_Lines_Chart_GenButton.Location = new System.Drawing.Point(185, 398);
            this.Total_Ride_Lines_Chart_GenButton.Name = "Total_Ride_Lines_Chart_GenButton";
            this.Total_Ride_Lines_Chart_GenButton.Size = new System.Drawing.Size(111, 23);
            this.Total_Ride_Lines_Chart_GenButton.TabIndex = 3;
            this.Total_Ride_Lines_Chart_GenButton.Text = "aantal ritten per lijn";
            this.Total_Ride_Lines_Chart_GenButton.UseVisualStyleBackColor = true;
            this.Total_Ride_Lines_Chart_GenButton.Click += new System.EventHandler(this.Total_Ride_Lines_Chart_Gen_Click);
            // 
            // StopsPerStationButton
            // 
            this.StopsPerStationButton.Location = new System.Drawing.Point(323, 398);
            this.StopsPerStationButton.Name = "StopsPerStationButton";
            this.StopsPerStationButton.Size = new System.Drawing.Size(96, 23);
            this.StopsPerStationButton.TabIndex = 4;
            this.StopsPerStationButton.Text = "Stops per station";
            this.StopsPerStationButton.UseVisualStyleBackColor = true;
            this.StopsPerStationButton.Click += new System.EventHandler(this.StopsPerStationButton_Click);
            // 
            // StationsPerLineButton
            // 
            this.StationsPerLineButton.Location = new System.Drawing.Point(450, 398);
            this.StationsPerLineButton.Name = "StationsPerLineButton";
            this.StationsPerLineButton.Size = new System.Drawing.Size(123, 23);
            this.StationsPerLineButton.TabIndex = 5;
            this.StationsPerLineButton.Text = "Stations per lijn";
            this.StationsPerLineButton.UseVisualStyleBackColor = true;
            this.StationsPerLineButton.Click += new System.EventHandler(this.StationsPerLineButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 497);
            this.Controls.Add(this.StationsPerLineButton);
            this.Controls.Add(this.StopsPerStationButton);
            this.Controls.Add(this.Total_Ride_Lines_Chart_GenButton);
            this.Controls.Add(this.RideHourDayButton);
            this.Controls.Add(this.startSimulationButton);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Main";
            this.Text = "Main";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button startSimulationButton;
        private System.Windows.Forms.Button RideHourDayButton;
        private System.Windows.Forms.Button Total_Ride_Lines_Chart_GenButton;
        private System.Windows.Forms.Button StopsPerStationButton;
        private System.Windows.Forms.Button StationsPerLineButton;
    }
}