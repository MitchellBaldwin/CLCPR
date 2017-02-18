namespace MIITTLCalibration
{
    partial class LoadWorkboatSplashscreen
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
            this.LNDLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LNDLabel
            // 
            this.LNDLabel.AutoSize = true;
            this.LNDLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LNDLabel.Location = new System.Drawing.Point(224, 107);
            this.LNDLabel.Name = "LNDLabel";
            this.LNDLabel.Size = new System.Drawing.Size(492, 39);
            this.LNDLabel.TabIndex = 0;
            this.LNDLabel.Text = "Loading normalization data,...";
            this.LNDLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoadWorkboatSplashscreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(941, 253);
            this.Controls.Add(this.LNDLabel);
            this.Name = "LoadWorkboatSplashscreen";
            this.Text = "Loading Normalization Data";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LNDLabel;
    }
}