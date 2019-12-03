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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.sRec = new System.Windows.Forms.Button();
            this.stRec = new System.Windows.Forms.Button();
            this.sPlay = new System.Windows.Forms.Button();
            this.stPlay = new System.Windows.Forms.Button();
            this.dftButton = new System.Windows.Forms.Button();
            this.filterButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(1362, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.newEditorToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.openToolStripMenuItem.Text = "File";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.openFileToolStripMenuItem.Text = "Open";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.saveToolStripMenuItem.Text = "Save As";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // newEditorToolStripMenuItem
            // 
            this.newEditorToolStripMenuItem.Name = "newEditorToolStripMenuItem";
            this.newEditorToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.newEditorToolStripMenuItem.Text = "New Editor";
            this.newEditorToolStripMenuItem.Click += new System.EventHandler(this.newEditorToolStripMenuItem_Click);
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.DimGray;
            this.chart1.BorderlineColor = System.Drawing.Color.DarkRed;
            chartArea7.BackColor = System.Drawing.Color.DimGray;
            chartArea7.InnerPlotPosition.Auto = false;
            chartArea7.InnerPlotPosition.Height = 82.58639F;
            chartArea7.InnerPlotPosition.Width = 90F;
            chartArea7.InnerPlotPosition.X = 10F;
            chartArea7.InnerPlotPosition.Y = 5.0266F;
            chartArea7.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea7);
            legend7.Enabled = false;
            legend7.Name = "Legend1";
            this.chart1.Legends.Add(legend7);
            this.chart1.Location = new System.Drawing.Point(0, 30);
            this.chart1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chart1.Name = "chart1";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series7.Legend = "Legend1";
            series7.Name = "Series1";
            series7.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            this.chart1.Series.Add(series7);
            this.chart1.Size = new System.Drawing.Size(989, 368);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.Chart1_Click);
            // 
            // chart2
            // 
            this.chart2.BackColor = System.Drawing.Color.DimGray;
            chartArea8.AxisX.IntervalOffset = 1D;
            chartArea8.BackColor = System.Drawing.Color.DimGray;
            chartArea8.InnerPlotPosition.Auto = false;
            chartArea8.InnerPlotPosition.Height = 80F;
            chartArea8.InnerPlotPosition.Width = 90F;
            chartArea8.InnerPlotPosition.X = 10F;
            chartArea8.InnerPlotPosition.Y = 10F;
            chartArea8.Name = "ChartArea1";
            chartArea8.Position.Auto = false;
            chartArea8.Position.Height = 94F;
            chartArea8.Position.Width = 94F;
            chartArea8.Position.X = 3F;
            this.chart2.ChartAreas.Add(chartArea8);
            legend8.Enabled = false;
            legend8.Name = "Legend1";
            this.chart2.Legends.Add(legend8);
            this.chart2.Location = new System.Drawing.Point(0, 435);
            this.chart2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chart2.Name = "chart2";
            this.chart2.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            series8.ChartArea = "ChartArea1";
            series8.IsVisibleInLegend = false;
            series8.Legend = "Legend1";
            series8.Name = "Series1";
            this.chart2.Series.Add(series8);
            this.chart2.Size = new System.Drawing.Size(989, 383);
            this.chart2.TabIndex = 3;
            this.chart2.Text = "chart2";
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(0, 400);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(989, 21);
            this.hScrollBar1.TabIndex = 4;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // sRec
            // 
            this.sRec.BackColor = System.Drawing.Color.Black;
            this.sRec.ForeColor = System.Drawing.Color.White;
            this.sRec.Location = new System.Drawing.Point(17, 97);
            this.sRec.Name = "sRec";
            this.sRec.Size = new System.Drawing.Size(134, 38);
            this.sRec.TabIndex = 5;
            this.sRec.Text = "Start Rec";
            this.sRec.UseVisualStyleBackColor = false;
            this.sRec.Click += new System.EventHandler(this.sRec_Click_1);
            // 
            // stRec
            // 
            this.stRec.BackColor = System.Drawing.Color.Black;
            this.stRec.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.stRec.Location = new System.Drawing.Point(167, 97);
            this.stRec.Name = "stRec";
            this.stRec.Size = new System.Drawing.Size(134, 38);
            this.stRec.TabIndex = 6;
            this.stRec.Text = "Stop Rec";
            this.stRec.UseVisualStyleBackColor = false;
            this.stRec.Click += new System.EventHandler(this.stRec_Click);
            // 
            // sPlay
            // 
            this.sPlay.BackColor = System.Drawing.Color.Black;
            this.sPlay.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sPlay.Location = new System.Drawing.Point(17, 152);
            this.sPlay.Name = "sPlay";
            this.sPlay.Size = new System.Drawing.Size(134, 38);
            this.sPlay.TabIndex = 7;
            this.sPlay.Text = "Start Play";
            this.sPlay.UseVisualStyleBackColor = false;
            this.sPlay.Click += new System.EventHandler(this.sPlay_Click_1);
            // 
            // stPlay
            // 
            this.stPlay.BackColor = System.Drawing.Color.Black;
            this.stPlay.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.stPlay.Location = new System.Drawing.Point(167, 152);
            this.stPlay.Name = "stPlay";
            this.stPlay.Size = new System.Drawing.Size(134, 38);
            this.stPlay.TabIndex = 8;
            this.stPlay.Text = "Stop Play";
            this.stPlay.UseVisualStyleBackColor = false;
            this.stPlay.Click += new System.EventHandler(this.stPlay_Click_1);
            // 
            // dftButton
            // 
            this.dftButton.BackColor = System.Drawing.Color.Black;
            this.dftButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dftButton.Location = new System.Drawing.Point(208, 110);
            this.dftButton.Name = "dftButton";
            this.dftButton.Size = new System.Drawing.Size(93, 38);
            this.dftButton.TabIndex = 9;
            this.dftButton.Text = "DFT";
            this.dftButton.UseVisualStyleBackColor = false;
            this.dftButton.Click += new System.EventHandler(this.dftButton_Click);
            // 
            // filterButton
            // 
            this.filterButton.BackColor = System.Drawing.Color.Black;
            this.filterButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.filterButton.Location = new System.Drawing.Point(107, 104);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(108, 44);
            this.filterButton.TabIndex = 10;
            this.filterButton.Text = "Apply Filter";
            this.filterButton.UseVisualStyleBackColor = false;
            this.filterButton.Click += new System.EventHandler(this.filterButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox4);
            this.groupBox1.Controls.Add(this.comboBox3);
            this.groupBox1.Controls.Add(this.sRec);
            this.groupBox1.Controls.Add(this.stRec);
            this.groupBox1.Controls.Add(this.sPlay);
            this.groupBox1.Controls.Add(this.stPlay);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox1.Location = new System.Drawing.Point(1004, 46);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 220);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recording";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "Quantization";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Sample Rate (Hz)";
            // 
            // comboBox4
            // 
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "16 bit",
            "32 bit"});
            this.comboBox4.Location = new System.Drawing.Point(167, 57);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(134, 24);
            this.comboBox4.TabIndex = 10;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "11025Hz",
            "22050Hz",
            "44100Hz"});
            this.comboBox3.Location = new System.Drawing.Point(17, 57);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(134, 24);
            this.comboBox3.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.comboBox5);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.comboBox2);
            this.groupBox2.Controls.Add(this.dftButton);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox2.Location = new System.Drawing.Point(1004, 435);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(320, 162);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Discrete Fourier";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 17);
            this.label5.TabIndex = 16;
            this.label5.Text = "Performance";
            // 
            // comboBox5
            // 
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Items.AddRange(new object[] {
            "Single Thread",
            "Multithread",
            "Single vs Multithread Test"});
            this.comboBox5.Location = new System.Drawing.Point(17, 55);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(284, 24);
            this.comboBox5.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "Windowing";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Rectangular",
            "Hamming",
            "Hanning"});
            this.comboBox2.Location = new System.Drawing.Point(17, 118);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(168, 24);
            this.comboBox2.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "Performance";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Basic",
            "OpenCL",
            "OpenCL vs Basic Test"});
            this.comboBox1.Location = new System.Drawing.Point(17, 59);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(284, 24);
            this.comboBox1.TabIndex = 10;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.filterButton);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox3.Location = new System.Drawing.Point(1004, 639);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(320, 167);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filtering";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1362, 827);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.ToolStripMenuItem newEditorToolStripMenuItem;
    }
}

