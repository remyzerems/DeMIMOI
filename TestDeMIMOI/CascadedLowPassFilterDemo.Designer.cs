namespace TestDeMIMOI
{
    partial class CascadedLowPassFilterDemo
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CascadedLowPassFilterDemo));
            this.trackBar_stage1 = new System.Windows.Forms.TrackBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar_stage2 = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBar_stage3 = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.trackBar_stage4 = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBar_stage5 = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage5)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackBar_stage1
            // 
            this.trackBar_stage1.Enabled = false;
            this.trackBar_stage1.Location = new System.Drawing.Point(164, 85);
            this.trackBar_stage1.Maximum = 1000;
            this.trackBar_stage1.Name = "trackBar_stage1";
            this.trackBar_stage1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_stage1.Size = new System.Drawing.Size(45, 307);
            this.trackBar_stage1.TabIndex = 7;
            this.trackBar_stage1.TickFrequency = 10;
            this.trackBar_stage1.Value = 500;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(71, 85);
            this.trackBar1.Maximum = 1000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 307);
            this.trackBar1.TabIndex = 6;
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.Value = 500;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(346, 428);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 32);
            this.button1.TabIndex = 11;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(692, 73);
            this.label3.TabIndex = 10;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(131, 395);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 26);
            this.label2.TabIndex = 9;
            this.label2.Text = "Stage 1\r\n1st order output";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 395);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Input value";
            // 
            // trackBar_stage2
            // 
            this.trackBar_stage2.Enabled = false;
            this.trackBar_stage2.Location = new System.Drawing.Point(275, 85);
            this.trackBar_stage2.Maximum = 1000;
            this.trackBar_stage2.Name = "trackBar_stage2";
            this.trackBar_stage2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_stage2.Size = new System.Drawing.Size(45, 307);
            this.trackBar_stage2.TabIndex = 12;
            this.trackBar_stage2.TickFrequency = 10;
            this.trackBar_stage2.Value = 500;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(242, 395);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 26);
            this.label4.TabIndex = 13;
            this.label4.Text = "Stage 2\r\n2nd order output";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBar_stage3
            // 
            this.trackBar_stage3.Enabled = false;
            this.trackBar_stage3.Location = new System.Drawing.Point(386, 85);
            this.trackBar_stage3.Maximum = 1000;
            this.trackBar_stage3.Name = "trackBar_stage3";
            this.trackBar_stage3.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_stage3.Size = new System.Drawing.Size(45, 307);
            this.trackBar_stage3.TabIndex = 14;
            this.trackBar_stage3.TickFrequency = 10;
            this.trackBar_stage3.Value = 500;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(353, 395);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 26);
            this.label5.TabIndex = 15;
            this.label5.Text = "Stage 3\r\n3rd order output";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBar_stage4
            // 
            this.trackBar_stage4.Enabled = false;
            this.trackBar_stage4.Location = new System.Drawing.Point(499, 85);
            this.trackBar_stage4.Maximum = 1000;
            this.trackBar_stage4.Name = "trackBar_stage4";
            this.trackBar_stage4.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_stage4.Size = new System.Drawing.Size(45, 307);
            this.trackBar_stage4.TabIndex = 16;
            this.trackBar_stage4.TickFrequency = 10;
            this.trackBar_stage4.Value = 500;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(466, 395);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 26);
            this.label6.TabIndex = 17;
            this.label6.Text = "Stage 4\r\n4th order output";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBar_stage5
            // 
            this.trackBar_stage5.Enabled = false;
            this.trackBar_stage5.Location = new System.Drawing.Point(609, 85);
            this.trackBar_stage5.Maximum = 1000;
            this.trackBar_stage5.Name = "trackBar_stage5";
            this.trackBar_stage5.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_stage5.Size = new System.Drawing.Size(45, 307);
            this.trackBar_stage5.TabIndex = 18;
            this.trackBar_stage5.TickFrequency = 10;
            this.trackBar_stage5.Value = 500;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(576, 395);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 26);
            this.label7.TabIndex = 19;
            this.label7.Text = "Stage 5\r\n5th order output";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(689, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 419);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "GraphViz source code";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(209, 389);
            this.textBox1.TabIndex = 0;
            // 
            // CascadedLowPassFilterDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 472);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.trackBar_stage5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.trackBar_stage4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.trackBar_stage3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.trackBar_stage2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trackBar_stage1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CascadedLowPassFilterDemo";
            this.Text = "CascadedLowPassFilter";
            this.Load += new System.EventHandler(this.CascadedLowPassFilter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_stage5)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar_stage1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar_stage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackBar_stage3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackBar_stage4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar trackBar_stage5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
    }
}