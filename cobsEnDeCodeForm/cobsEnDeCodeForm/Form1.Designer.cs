namespace WindowsFormsApplication1
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
            this.textDataIn = new System.Windows.Forms.TextBox();
            this.textEncoded = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textDecode = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textDataIn
            // 
            this.textDataIn.Location = new System.Drawing.Point(109, 23);
            this.textDataIn.Name = "textDataIn";
            this.textDataIn.Size = new System.Drawing.Size(426, 20);
            this.textDataIn.TabIndex = 0;
            // 
            // textEncoded
            // 
            this.textEncoded.Location = new System.Drawing.Point(109, 65);
            this.textEncoded.Name = "textEncoded";
            this.textEncoded.Size = new System.Drawing.Size(426, 20);
            this.textEncoded.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Encode";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 62);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Decode";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textDecode
            // 
            this.textDecode.Location = new System.Drawing.Point(109, 107);
            this.textDecode.Name = "textDecode";
            this.textDecode.Size = new System.Drawing.Size(426, 20);
            this.textDecode.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 391);
            this.Controls.Add(this.textDecode);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textEncoded);
            this.Controls.Add(this.textDataIn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textDataIn;
        private System.Windows.Forms.TextBox textEncoded;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textDecode;
    }
}

