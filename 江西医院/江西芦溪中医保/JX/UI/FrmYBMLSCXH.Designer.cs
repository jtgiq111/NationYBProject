
namespace yb_interfaces
{
    partial class FrmYBMLSC
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
            this.label_file_path = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.textBox_maxver = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_download = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tabControl_ml = new System.Windows.Forms.TabControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_info = new System.Windows.Forms.Label();
            this.label_pro = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_bulk = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_repCount = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_file_path
            // 
            this.label_file_path.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_file_path.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_file_path.ForeColor = System.Drawing.Color.Red;
            this.label_file_path.Location = new System.Drawing.Point(1080, 13);
            this.label_file_path.Name = "label_file_path";
            this.label_file_path.Size = new System.Drawing.Size(298, 23);
            this.label_file_path.TabIndex = 3;
            this.label_file_path.Click += new System.EventHandler(this.label_file_path_Click);
            // 
            // textBox_maxver
            // 
            this.textBox_maxver.Location = new System.Drawing.Point(56, 13);
            this.textBox_maxver.Name = "textBox_maxver";
            this.textBox_maxver.Size = new System.Drawing.Size(193, 28);
            this.textBox_maxver.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 19);
            this.label3.TabIndex = 14;
            this.label3.Text = "版本";
            // 
            // button_download
            // 
            this.button_download.Location = new System.Drawing.Point(445, 10);
            this.button_download.Margin = new System.Windows.Forms.Padding(4);
            this.button_download.Name = "button_download";
            this.button_download.Size = new System.Drawing.Size(159, 29);
            this.button_download.TabIndex = 10;
            this.button_download.Text = "下载最新数据";
            this.button_download.UseVisualStyleBackColor = true;
            this.button_download.Click += new System.EventHandler(this.button_download_Click);
            // 
            // button4
            // 
            this.button4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button4.Location = new System.Drawing.Point(801, 10);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(159, 29);
            this.button4.TabIndex = 13;
            this.button4.Text = "同步到数据库";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // tabControl_ml
            // 
            this.tabControl_ml.Location = new System.Drawing.Point(14, 44);
            this.tabControl_ml.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl_ml.Name = "tabControl_ml";
            this.tabControl_ml.SelectedIndex = 0;
            this.tabControl_ml.Size = new System.Drawing.Size(1356, 617);
            this.tabControl_ml.TabIndex = 9;
            this.tabControl_ml.SelectedIndexChanged += new System.EventHandler(this.tabControl_ml_SelectedIndexChanged);
            this.tabControl_ml.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_ml_Selecting);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel1.Controls.Add(this.label_info);
            this.panel1.Controls.Add(this.label_pro);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Location = new System.Drawing.Point(12, 668);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1360, 27);
            this.panel1.TabIndex = 12;
            // 
            // label_info
            // 
            this.label_info.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_info.AutoSize = true;
            this.label_info.Location = new System.Drawing.Point(312, 7);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(0, 19);
            this.label_info.TabIndex = 2;
            // 
            // label_pro
            // 
            this.label_pro.AutoSize = true;
            this.label_pro.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label_pro.Location = new System.Drawing.Point(124, 7);
            this.label_pro.Name = "label_pro";
            this.label_pro.Size = new System.Drawing.Size(29, 19);
            this.label_pro.TabIndex = 1;
            this.label_pro.Text = "0%";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.progressBar1.Location = new System.Drawing.Point(3, 3);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(303, 21);
            this.progressBar1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(966, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 19);
            this.label1.TabIndex = 16;
            this.label1.Text = "文件路径";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBox_bulk
            // 
            this.textBox_bulk.Location = new System.Drawing.Point(326, 13);
            this.textBox_bulk.Name = "textBox_bulk";
            this.textBox_bulk.Size = new System.Drawing.Size(100, 28);
            this.textBox_bulk.TabIndex = 17;
            this.textBox_bulk.Text = "30";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 19);
            this.label2.TabIndex = 18;
            this.label2.Text = "下载条数";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(625, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 19);
            this.label4.TabIndex = 20;
            this.label4.Text = "同步条数";
            // 
            // textBox_repCount
            // 
            this.textBox_repCount.Location = new System.Drawing.Point(695, 12);
            this.textBox_repCount.Name = "textBox_repCount";
            this.textBox_repCount.Size = new System.Drawing.Size(100, 28);
            this.textBox_repCount.TabIndex = 19;
            this.textBox_repCount.Text = "2000";
            // 
            // FrmYBMLSC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1390, 707);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_repCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_bulk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_maxver);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_download);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.tabControl_ml);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label_file_path);
            this.Font = new System.Drawing.Font("宋体", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmYBMLSC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "医保目录";
            this.Load += new System.EventHandler(this.FrmYBMLSC_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_file_path;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox textBox_maxver;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_download;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabControl tabControl_ml;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_info;
        private System.Windows.Forms.Label label_pro;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_bulk;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_repCount;
    }
}