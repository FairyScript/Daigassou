namespace Daigassou.Overlay
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSyncReady = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.networkDelayInput = new System.Windows.Forms.NumericUpDown();
            this.btnTimeSync = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnFileSelect = new System.Windows.Forms.Button();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trackComboBox = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.networkDelayInput)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnTimeSync);
            this.panel1.Controls.Add(this.networkDelayInput);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.btnSyncReady);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.dateTimePicker1);
            this.panel1.Location = new System.Drawing.Point(12, 127);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(357, 100);
            this.panel1.TabIndex = 6;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("微软雅黑", 10.5F);
            this.dateTimePicker1.CustomFormat = "HH-mm-ss";
            this.dateTimePicker1.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker1.Location = new System.Drawing.Point(112, 20);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.ShowUpDown = true;
            this.dateTimePicker1.Size = new System.Drawing.Size(100, 26);
            this.dateTimePicker1.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Gray;
            this.label4.Location = new System.Drawing.Point(21, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "演奏时间";
            // 
            // btnSyncReady
            // 
            this.btnSyncReady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnSyncReady.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSyncReady.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnSyncReady.ForeColor = System.Drawing.Color.White;
            this.btnSyncReady.Location = new System.Drawing.Point(252, 53);
            this.btnSyncReady.Name = "btnSyncReady";
            this.btnSyncReady.Size = new System.Drawing.Size(81, 29);
            this.btnSyncReady.TabIndex = 9;
            this.btnSyncReady.Text = "准备好了";
            this.btnSyncReady.UseVisualStyleBackColor = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(21, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 20);
            this.label8.TabIndex = 10;
            this.label8.Text = "网络延迟";
            // 
            // networkDelayInput
            // 
            this.networkDelayInput.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.networkDelayInput.Location = new System.Drawing.Point(112, 55);
            this.networkDelayInput.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.networkDelayInput.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.networkDelayInput.Name = "networkDelayInput";
            this.networkDelayInput.Size = new System.Drawing.Size(100, 26);
            this.networkDelayInput.TabIndex = 11;
            // 
            // btnTimeSync
            // 
            this.btnTimeSync.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnTimeSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimeSync.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnTimeSync.ForeColor = System.Drawing.Color.White;
            this.btnTimeSync.Location = new System.Drawing.Point(252, 18);
            this.btnTimeSync.Name = "btnTimeSync";
            this.btnTimeSync.Size = new System.Drawing.Size(81, 29);
            this.btnTimeSync.TabIndex = 12;
            this.btnTimeSync.Text = "开启抓包";
            this.btnTimeSync.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnFileSelect);
            this.panel2.Controls.Add(this.pathTextBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.trackComboBox);
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(357, 100);
            this.panel2.TabIndex = 8;
            // 
            // btnFileSelect
            // 
            this.btnFileSelect.BackColor = System.Drawing.Color.White;
            this.btnFileSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFileSelect.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFileSelect.ForeColor = System.Drawing.Color.Black;
            this.btnFileSelect.Location = new System.Drawing.Point(291, 19);
            this.btnFileSelect.Name = "btnFileSelect";
            this.btnFileSelect.Size = new System.Drawing.Size(45, 26);
            this.btnFileSelect.TabIndex = 12;
            this.btnFileSelect.Text = "···";
            this.btnFileSelect.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnFileSelect.UseVisualStyleBackColor = false;
            // 
            // pathTextBox
            // 
            this.pathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pathTextBox.Enabled = false;
            this.pathTextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pathTextBox.ForeColor = System.Drawing.Color.Gray;
            this.pathTextBox.Location = new System.Drawing.Point(93, 19);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(192, 26);
            this.pathTextBox.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "导入Mid";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(16, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "选择音轨";
            // 
            // trackComboBox
            // 
            this.trackComboBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.trackComboBox.ForeColor = System.Drawing.Color.Gray;
            this.trackComboBox.FormattingEnabled = true;
            this.trackComboBox.Location = new System.Drawing.Point(93, 54);
            this.trackComboBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trackComboBox.Name = "trackComboBox";
            this.trackComboBox.Size = new System.Drawing.Size(243, 28);
            this.trackComboBox.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 272);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.networkDelayInput)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnTimeSync;
        private System.Windows.Forms.NumericUpDown networkDelayInput;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSyncReady;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnFileSelect;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox trackComboBox;
    }
}