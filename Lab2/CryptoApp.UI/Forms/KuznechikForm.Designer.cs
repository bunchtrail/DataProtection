namespace Lab2
{
    partial class KuznechikForm
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
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.buttonEncrypt = new System.Windows.Forms.Button();
            this.buttonInsertTestData = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(12, 32);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessage.Size = new System.Drawing.Size(637, 60);
            this.textBoxMessage.TabIndex = 0;
            this.textBoxMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxMessage.BackColor = System.Drawing.Color.White;
            this.textBoxMessage.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // textBoxKey
            // 
            this.textBoxKey.Location = new System.Drawing.Point(12, 118);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(637, 23);
            this.textBoxKey.TabIndex = 1;
            this.textBoxKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxKey.BackColor = System.Drawing.Color.White;
            this.textBoxKey.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.textBoxKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Location = new System.Drawing.Point(12, 204);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxOutput.Size = new System.Drawing.Size(637, 60);
            this.textBoxOutput.TabIndex = 2;
            this.textBoxOutput.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxOutput.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.textBoxOutput.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.textBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // buttonEncrypt
            // 
            this.buttonEncrypt.Location = new System.Drawing.Point(12, 270);
            this.buttonEncrypt.Name = "buttonEncrypt";
            this.buttonEncrypt.Size = new System.Drawing.Size(200, 30);
            this.buttonEncrypt.TabIndex = 3;
            this.buttonEncrypt.Text = "Вычислить имитовставку";
            this.buttonEncrypt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEncrypt.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.buttonEncrypt.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 241, 251);
            this.buttonEncrypt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(204, 228, 247);
            this.buttonEncrypt.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.buttonEncrypt.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.buttonEncrypt.ForeColor = System.Drawing.Color.White;
            this.buttonEncrypt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonEncrypt.Click += new System.EventHandler(this.buttonEncrypt_Click);
            // 
            // buttonInsertTestData
            // 
            this.buttonInsertTestData.Location = new System.Drawing.Point(218, 270);
            this.buttonInsertTestData.Name = "buttonInsertTestData";
            this.buttonInsertTestData.Size = new System.Drawing.Size(200, 30);
            this.buttonInsertTestData.TabIndex = 4;
            this.buttonInsertTestData.Text = "Вставить тестовые данные";
            this.buttonInsertTestData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonInsertTestData.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.buttonInsertTestData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 241, 251);
            this.buttonInsertTestData.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(204, 228, 247);
            this.buttonInsertTestData.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.buttonInsertTestData.BackColor = System.Drawing.Color.White;
            this.buttonInsertTestData.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.buttonInsertTestData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonInsertTestData.Click += new System.EventHandler(this.buttonInsertTestData_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Сообщение:";
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Ключ (256 бит):";
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(200, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Имитовставка:";
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            // 
            // KuznechikForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 408);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonInsertTestData);
            this.Controls.Add(this.buttonEncrypt);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.textBoxKey);
            this.Controls.Add(this.textBoxMessage);
            this.Name = "KuznechikForm";
            this.Text = "Шифр Кузнечик";
            this.BackColor = System.Drawing.Color.White;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button buttonEncrypt;
        private System.Windows.Forms.Button buttonInsertTestData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
} 