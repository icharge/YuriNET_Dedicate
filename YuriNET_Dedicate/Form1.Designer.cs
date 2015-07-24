namespace YuriNET
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
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOnlineCount = new System.Windows.Forms.Label();
            this.btn_saveState = new System.Windows.Forms.Button();
            this.btn_loadState = new System.Windows.Forms.Button();
            this.rbDbgNone = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbDbgError = new System.Windows.Forms.RadioButton();
            this.rbDbgWarn = new System.Windows.Forms.RadioButton();
            this.rbDbgDebug = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(60, 146);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(205, 73);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(273, 146);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(205, 73);
            this.button3.TabIndex = 0;
            this.button3.Text = "Stop";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(109, 73);
            this.txtPort.Margin = new System.Windows.Forms.Padding(4);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(247, 32);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "4434";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 48);
            this.label2.TabIndex = 4;
            this.label2.Text = "By ThaiRA2Lovers\r\nBased on CNCNET 5";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(60, 227);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(205, 73);
            this.button2.TabIndex = 1;
            this.button2.Text = "Refresh client count";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(272, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Online: ";
            // 
            // txtOnlineCount
            // 
            this.txtOnlineCount.AutoSize = true;
            this.txtOnlineCount.Location = new System.Drawing.Point(358, 251);
            this.txtOnlineCount.Name = "txtOnlineCount";
            this.txtOnlineCount.Size = new System.Drawing.Size(21, 24);
            this.txtOnlineCount.TabIndex = 6;
            this.txtOnlineCount.Text = "0";
            // 
            // btn_saveState
            // 
            this.btn_saveState.Location = new System.Drawing.Point(273, 332);
            this.btn_saveState.Margin = new System.Windows.Forms.Padding(4);
            this.btn_saveState.Name = "btn_saveState";
            this.btn_saveState.Size = new System.Drawing.Size(205, 73);
            this.btn_saveState.TabIndex = 1;
            this.btn_saveState.Text = "Save State";
            this.btn_saveState.UseVisualStyleBackColor = true;
            this.btn_saveState.Visible = false;
            this.btn_saveState.Click += new System.EventHandler(this.btn_saveState_Click);
            // 
            // btn_loadState
            // 
            this.btn_loadState.Location = new System.Drawing.Point(60, 332);
            this.btn_loadState.Margin = new System.Windows.Forms.Padding(4);
            this.btn_loadState.Name = "btn_loadState";
            this.btn_loadState.Size = new System.Drawing.Size(205, 73);
            this.btn_loadState.TabIndex = 1;
            this.btn_loadState.Text = "Load State";
            this.btn_loadState.UseVisualStyleBackColor = true;
            this.btn_loadState.Visible = false;
            this.btn_loadState.Click += new System.EventHandler(this.btn_loadState_Click);
            // 
            // rbDbgNone
            // 
            this.rbDbgNone.AutoSize = true;
            this.rbDbgNone.Checked = true;
            this.rbDbgNone.Location = new System.Drawing.Point(20, 34);
            this.rbDbgNone.Name = "rbDbgNone";
            this.rbDbgNone.Size = new System.Drawing.Size(81, 28);
            this.rbDbgNone.TabIndex = 7;
            this.rbDbgNone.TabStop = true;
            this.rbDbgNone.Text = "None";
            this.rbDbgNone.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDbgError);
            this.groupBox1.Controls.Add(this.rbDbgWarn);
            this.groupBox1.Controls.Add(this.rbDbgDebug);
            this.groupBox1.Controls.Add(this.rbDbgNone);
            this.groupBox1.Location = new System.Drawing.Point(508, 144);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 261);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Debug Level";
            // 
            // rbDbgError
            // 
            this.rbDbgError.AutoSize = true;
            this.rbDbgError.Location = new System.Drawing.Point(20, 136);
            this.rbDbgError.Name = "rbDbgError";
            this.rbDbgError.Size = new System.Drawing.Size(78, 28);
            this.rbDbgError.TabIndex = 7;
            this.rbDbgError.Text = "Error";
            this.rbDbgError.UseVisualStyleBackColor = true;
            // 
            // rbDbgWarn
            // 
            this.rbDbgWarn.AutoSize = true;
            this.rbDbgWarn.Location = new System.Drawing.Point(20, 102);
            this.rbDbgWarn.Name = "rbDbgWarn";
            this.rbDbgWarn.Size = new System.Drawing.Size(109, 28);
            this.rbDbgWarn.TabIndex = 7;
            this.rbDbgWarn.Text = "Warning";
            this.rbDbgWarn.UseVisualStyleBackColor = true;
            // 
            // rbDbgDebug
            // 
            this.rbDbgDebug.AutoSize = true;
            this.rbDbgDebug.Location = new System.Drawing.Point(20, 68);
            this.rbDbgDebug.Name = "rbDbgDebug";
            this.rbDbgDebug.Size = new System.Drawing.Size(93, 28);
            this.rbDbgDebug.TabIndex = 7;
            this.rbDbgDebug.Text = "Debug";
            this.rbDbgDebug.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(729, 436);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtOnlineCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.btn_loadState);
            this.Controls.Add(this.btn_saveState);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YuriNET Server launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label txtOnlineCount;
        private System.Windows.Forms.Button btn_saveState;
        private System.Windows.Forms.Button btn_loadState;
        private System.Windows.Forms.RadioButton rbDbgNone;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbDbgError;
        private System.Windows.Forms.RadioButton rbDbgWarn;
        private System.Windows.Forms.RadioButton rbDbgDebug;
    }
}

