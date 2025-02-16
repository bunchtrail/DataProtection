namespace Lab2
{
    partial class RSAForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBoxInput = new TextBox();
            textBoxEncrypted = new TextBox();
            textBoxDecrypted = new TextBox();
            buttonEncrypt = new Button();
            buttonDecrypt = new Button();
            labelPublicKey = new Label();
            labelPrivateKey = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            
            SuspendLayout();
            
            // textBoxInput
            textBoxInput.Location = new Point(12, 32);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.ScrollBars = ScrollBars.Vertical;
            textBoxInput.Size = new Size(637, 60);
            textBoxInput.TabIndex = 0;
            textBoxInput.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxInput.BackColor = Color.White;
            textBoxInput.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxInput.BorderStyle = BorderStyle.FixedSingle;
            
            // textBoxEncrypted
            textBoxEncrypted.Location = new Point(12, 118);
            textBoxEncrypted.Multiline = true;
            textBoxEncrypted.Name = "textBoxEncrypted";
            textBoxEncrypted.ScrollBars = ScrollBars.Vertical;
            textBoxEncrypted.Size = new Size(637, 60);
            textBoxEncrypted.TabIndex = 1;
            textBoxEncrypted.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxEncrypted.BackColor = Color.FromArgb(245, 245, 245);
            textBoxEncrypted.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxEncrypted.BorderStyle = BorderStyle.FixedSingle;
            
            // textBoxDecrypted
            textBoxDecrypted.Location = new Point(12, 204);
            textBoxDecrypted.Multiline = true;
            textBoxDecrypted.Name = "textBoxDecrypted";
            textBoxDecrypted.ReadOnly = true;
            textBoxDecrypted.ScrollBars = ScrollBars.Vertical;
            textBoxDecrypted.Size = new Size(637, 60);
            textBoxDecrypted.TabIndex = 2;
            textBoxDecrypted.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxDecrypted.BackColor = Color.FromArgb(245, 245, 245);
            textBoxDecrypted.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxDecrypted.BorderStyle = BorderStyle.FixedSingle;
            
            // buttonEncrypt
            buttonEncrypt.Location = new Point(12, 270);
            buttonEncrypt.Name = "buttonEncrypt";
            buttonEncrypt.Size = new Size(200, 30);
            buttonEncrypt.TabIndex = 3;
            buttonEncrypt.Text = "Зашифровать";
            buttonEncrypt.FlatStyle = FlatStyle.Flat;
            buttonEncrypt.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonEncrypt.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonEncrypt.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonEncrypt.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonEncrypt.BackColor = Color.FromArgb(0, 120, 215);
            buttonEncrypt.ForeColor = Color.White;
            buttonEncrypt.Cursor = Cursors.Hand;
            buttonEncrypt.Click += buttonEncrypt_Click;
            
            // buttonDecrypt
            buttonDecrypt.Location = new Point(218, 270);
            buttonDecrypt.Name = "buttonDecrypt";
            buttonDecrypt.Size = new Size(200, 30);
            buttonDecrypt.TabIndex = 4;
            buttonDecrypt.Text = "Расшифровать";
            buttonDecrypt.FlatStyle = FlatStyle.Flat;
            buttonDecrypt.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonDecrypt.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonDecrypt.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonDecrypt.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonDecrypt.BackColor = Color.White;
            buttonDecrypt.ForeColor = Color.FromArgb(0, 120, 215);
            buttonDecrypt.Cursor = Cursors.Hand;
            buttonDecrypt.Click += buttonDecrypt_Click;
            
            // labelPublicKey
            labelPublicKey.AutoSize = true;
            labelPublicKey.Location = new Point(12, 310);
            labelPublicKey.Name = "labelPublicKey";
            labelPublicKey.Size = new Size(200, 15);
            labelPublicKey.TabIndex = 5;
            labelPublicKey.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelPublicKey.ForeColor = Color.FromArgb(64, 64, 64);
            
            // labelPrivateKey
            labelPrivateKey.AutoSize = true;
            labelPrivateKey.Location = new Point(12, 335);
            labelPrivateKey.Name = "labelPrivateKey";
            labelPrivateKey.Size = new Size(200, 15);
            labelPrivateKey.TabIndex = 6;
            labelPrivateKey.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelPrivateKey.ForeColor = Color.FromArgb(64, 64, 64);
            
            // label1
            label1.AutoSize = true;
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(200, 15);
            label1.TabIndex = 7;
            label1.Text = "Исходное сообщение:";
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(64, 64, 64);
            
            // label2
            label2.AutoSize = true;
            label2.Location = new Point(12, 98);
            label2.Name = "label2";
            label2.Size = new Size(200, 15);
            label2.TabIndex = 8;
            label2.Text = "Зашифрованное сообщение:";
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(64, 64, 64);
            
            // label3
            label3.AutoSize = true;
            label3.Location = new Point(12, 184);
            label3.Name = "label3";
            label3.Size = new Size(200, 15);
            label3.TabIndex = 9;
            label3.Text = "Расшифрованное сообщение:";
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.FromArgb(64, 64, 64);
            
            // RSAForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(661, 408);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(labelPrivateKey);
            Controls.Add(labelPublicKey);
            Controls.Add(buttonDecrypt);
            Controls.Add(buttonEncrypt);
            Controls.Add(textBoxDecrypted);
            Controls.Add(textBoxEncrypted);
            Controls.Add(textBoxInput);
            Name = "RSAForm";
            Text = "RSA";
            BackColor = Color.White;
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox textBoxInput;
        private TextBox textBoxEncrypted;
        private TextBox textBoxDecrypted;
        private Button buttonEncrypt;
        private Button buttonDecrypt;
        private Label labelPublicKey;
        private Label labelPrivateKey;
        private Label label1;
        private Label label2;
        private Label label3;
    }
} 