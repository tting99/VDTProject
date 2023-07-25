namespace TestEmgu
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            btnAiProcess = new Button();
            lbPoseResult = new Label();
            btncap = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(242, 25);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(601, 411);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btnAiProcess
            // 
            btnAiProcess.Location = new Point(12, 12);
            btnAiProcess.Name = "btnAiProcess";
            btnAiProcess.RightToLeft = RightToLeft.No;
            btnAiProcess.Size = new Size(172, 87);
            btnAiProcess.TabIndex = 1;
            btnAiProcess.Text = "Ai처리";
            btnAiProcess.UseVisualStyleBackColor = true;
            btnAiProcess.Click += btnAiProcess_Click;
            // 
            // lbPoseResult
            // 
            lbPoseResult.AutoSize = true;
            lbPoseResult.Font = new Font("맑은 고딕", 20F, FontStyle.Bold, GraphicsUnit.Point);
            lbPoseResult.Location = new Point(75, 198);
            lbPoseResult.Name = "lbPoseResult";
            lbPoseResult.Size = new Size(71, 37);
            lbPoseResult.TabIndex = 2;
            lbPoseResult.Text = "병명";
            // 
            // btncap
            // 
            btncap.Location = new Point(55, 305);
            btncap.Name = "btncap";
            btncap.Size = new Size(75, 23);
            btncap.TabIndex = 3;
            btncap.Text = "캡처";
            btncap.UseVisualStyleBackColor = true;
            btncap.Click += BtnCap_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(906, 507);
            Controls.Add(btncap);
            Controls.Add(lbPoseResult);
            Controls.Add(btnAiProcess);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button btnAiProcess;
        private Label lbPoseResult;
        private Button btncap;
    }
}