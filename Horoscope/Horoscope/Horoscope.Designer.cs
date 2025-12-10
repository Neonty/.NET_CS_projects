namespace Horoscope
{
    partial class Horoscope
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
            this.ResultName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.ResultDescription = new System.Windows.Forms.Label();
            this.back = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ResultName
            // 
            this.ResultName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ResultName.AutoSize = true;
            this.ResultName.Font = new System.Drawing.Font("Pristina", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultName.Location = new System.Drawing.Point(500, 29);
            this.ResultName.Name = "ResultName";
            this.ResultName.Size = new System.Drawing.Size(0, 44);
            this.ResultName.TabIndex = 5;
            this.ResultName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.Font = new System.Drawing.Font("Pristina", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(173, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(883, 158);
            this.label2.TabIndex = 3;
            this.label2.Text = "Туған күніңізді енгізіңіз:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.Font = new System.Drawing.Font("Pristina", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(388, 409);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(465, 60);
            this.button1.TabIndex = 4;
            this.button1.Text = "Жорамалымды білу!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dateTimePicker2.CalendarTitleBackColor = System.Drawing.Color.AliceBlue;
            this.dateTimePicker2.Font = new System.Drawing.Font("Pristina", 25F);
            this.dateTimePicker2.Location = new System.Drawing.Point(395, 279);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(446, 51);
            this.dateTimePicker2.TabIndex = 1;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // ResultDescription
            // 
            this.ResultDescription.AutoSize = true;
            this.ResultDescription.Font = new System.Drawing.Font("Pristina", 25F);
            this.ResultDescription.Location = new System.Drawing.Point(330, 100);
            this.ResultDescription.Name = "ResultDescription";
            this.ResultDescription.Size = new System.Drawing.Size(0, 45);
            this.ResultDescription.TabIndex = 6;
            this.ResultDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // back
            // 
            this.back.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.back.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.back.Location = new System.Drawing.Point(1015, 29);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(179, 55);
            this.back.TabIndex = 7;
            this.back.Text = "Артқа";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // Horoscope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1229, 627);
            this.Controls.Add(this.back);
            this.Controls.Add(this.ResultDescription);
            this.Controls.Add(this.ResultName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePicker2);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Horoscope";
            this.Text = "Horoscope";
            this.Load += new System.EventHandler(this.Horoscope_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label ResultName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label ResultDescription;
        private System.Windows.Forms.Button back;
    }
}