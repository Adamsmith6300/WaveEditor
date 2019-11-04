namespace WaveVisualizer
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.sRec = new System.Windows.Forms.Button();
            this.stRec = new System.Windows.Forms.Button();
            this.sPlay = new System.Windows.Forms.Button();
            this.stPlay = new System.Windows.Forms.Button();
            this.dftButton = new System.Windows.Forms.Button();
            this.filterButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1207, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.openToolStripMenuItem.Text = "File";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.openFileToolStripMenuItem.Text = "Open";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.saveToolStripMenuItem.Text = "Save As";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // chart1
            // 
            this.chart1.BorderlineColor = System.Drawing.Color.DarkRed;
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(0, 30);
            this.chart1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chart1.Name = "chart1";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(795, 259);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.Chart1_Click);
            // 
            // chart2
            // 
            chartArea4.AxisX.IntervalOffset = 1D;
            chartArea4.InnerPlotPosition.Auto = false;
            chartArea4.InnerPlotPosition.Height = 85F;
            chartArea4.InnerPlotPosition.Width = 87F;
            chartArea4.InnerPlotPosition.X = 7F;
            chartArea4.InnerPlotPosition.Y = 7F;
            chartArea4.Name = "ChartArea1";
            chartArea4.Position.Auto = false;
            chartArea4.Position.Height = 94F;
            chartArea4.Position.Width = 94F;
            chartArea4.Position.X = 3F;
            this.chart2.ChartAreas.Add(chartArea4);
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.chart2.Legends.Add(legend4);
            this.chart2.Location = new System.Drawing.Point(0, 325);
            this.chart2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chart2.Name = "chart2";
            series4.ChartArea = "ChartArea1";
            series4.IsVisibleInLegend = false;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chart2.Series.Add(series4);
            this.chart2.Size = new System.Drawing.Size(795, 349);
            this.chart2.TabIndex = 3;
            this.chart2.Text = "chart2";
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(0, 291);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(795, 21);
            this.hScrollBar1.TabIndex = 4;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // sRec
            // 
            this.sRec.Location = new System.Drawing.Point(801, 31);
            this.sRec.Name = "sRec";
            this.sRec.Size = new System.Drawing.Size(134, 38);
            this.sRec.TabIndex = 5;
            this.sRec.Text = "Start Rec";
            this.sRec.UseVisualStyleBackColor = true;
            this.sRec.Click += new System.EventHandler(this.sRec_Click_1);
            // 
            // stRec
            // 
            this.stRec.Location = new System.Drawing.Point(941, 31);
            this.stRec.Name = "stRec";
            this.stRec.Size = new System.Drawing.Size(134, 38);
            this.stRec.TabIndex = 6;
            this.stRec.Text = "Stop Rec";
            this.stRec.UseVisualStyleBackColor = true;
            this.stRec.Click += new System.EventHandler(this.stRec_Click);
            // 
            // sPlay
            // 
            this.sPlay.Location = new System.Drawing.Point(801, 75);
            this.sPlay.Name = "sPlay";
            this.sPlay.Size = new System.Drawing.Size(134, 38);
            this.sPlay.TabIndex = 7;
            this.sPlay.Text = "Start Play";
            this.sPlay.UseVisualStyleBackColor = true;
            this.sPlay.Click += new System.EventHandler(this.sPlay_Click_1);
            // 
            // stPlay
            // 
            this.stPlay.Location = new System.Drawing.Point(941, 75);
            this.stPlay.Name = "stPlay";
            this.stPlay.Size = new System.Drawing.Size(134, 38);
            this.stPlay.TabIndex = 8;
            this.stPlay.Text = "Stop Play";
            this.stPlay.UseVisualStyleBackColor = true;
            this.stPlay.Click += new System.EventHandler(this.stPlay_Click_1);
            // 
            // dftButton
            // 
            this.dftButton.Location = new System.Drawing.Point(801, 325);
            this.dftButton.Name = "dftButton";
            this.dftButton.Size = new System.Drawing.Size(108, 38);
            this.dftButton.TabIndex = 9;
            this.dftButton.Text = "DFT";
            this.dftButton.UseVisualStyleBackColor = true;
            this.dftButton.Click += new System.EventHandler(this.dftButton_Click);
            // 
            // filterButton
            // 
            this.filterButton.Location = new System.Drawing.Point(801, 369);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(108, 44);
            this.filterButton.TabIndex = 10;
            this.filterButton.Text = "Filter";
            this.filterButton.UseVisualStyleBackColor = true;
            this.filterButton.Click += new System.EventHandler(this.filterButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1207, 685);
            this.Controls.Add(this.filterButton);
            this.Controls.Add(this.dftButton);
            this.Controls.Add(this.stPlay);
            this.Controls.Add(this.sPlay);
            this.Controls.Add(this.stRec);
            this.Controls.Add(this.sRec);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Button sRec;
        private System.Windows.Forms.Button stRec;
        private System.Windows.Forms.Button sPlay;
        private System.Windows.Forms.Button stPlay;
        private System.Windows.Forms.Button dftButton;
        private System.Windows.Forms.Button filterButton;
    }
}

