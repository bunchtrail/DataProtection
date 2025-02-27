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
            // 
            // textBoxInput
            // 
            textBoxInput.BackColor = Color.White;
            textBoxInput.BorderStyle = BorderStyle.FixedSingle;
            textBoxInput.Font = new Font("Segoe UI", 9F);
            textBoxInput.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxInput.Location = new Point(14, 43);
            textBoxInput.Margin = new Padding(3, 4, 3, 4);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.ScrollBars = ScrollBars.Vertical;
            textBoxInput.Size = new Size(728, 79);
            textBoxInput.TabIndex = 0;
            // 
            // textBoxEncrypted
            // 
            textBoxEncrypted.BackColor = Color.FromArgb(245, 245, 245);
            textBoxEncrypted.BorderStyle = BorderStyle.FixedSingle;
            textBoxEncrypted.Font = new Font("Segoe UI", 9F);
            textBoxEncrypted.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxEncrypted.Location = new Point(14, 157);
            textBoxEncrypted.Margin = new Padding(3, 4, 3, 4);
            textBoxEncrypted.Multiline = true;
            textBoxEncrypted.Name = "textBoxEncrypted";
            textBoxEncrypted.ScrollBars = ScrollBars.Vertical;
            textBoxEncrypted.Size = new Size(728, 79);
            textBoxEncrypted.TabIndex = 1;
            // 
            // textBoxDecrypted
            // 
            textBoxDecrypted.BackColor = Color.FromArgb(245, 245, 245);
            textBoxDecrypted.BorderStyle = BorderStyle.FixedSingle;
            textBoxDecrypted.Font = new Font("Segoe UI", 9F);
            textBoxDecrypted.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxDecrypted.Location = new Point(14, 272);
            textBoxDecrypted.Margin = new Padding(3, 4, 3, 4);
            textBoxDecrypted.Multiline = true;
            textBoxDecrypted.Name = "textBoxDecrypted";
            textBoxDecrypted.ReadOnly = true;
            textBoxDecrypted.ScrollBars = ScrollBars.Vertical;
            textBoxDecrypted.Size = new Size(728, 79);
            textBoxDecrypted.TabIndex = 2;
            // 
            // buttonEncrypt
            // 
            buttonEncrypt.BackColor = Color.FromArgb(0, 120, 215);
            buttonEncrypt.Cursor = Cursors.Hand;
            buttonEncrypt.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonEncrypt.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonEncrypt.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonEncrypt.FlatStyle = FlatStyle.Flat;
            buttonEncrypt.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            buttonEncrypt.ForeColor = Color.White;
            buttonEncrypt.Location = new Point(14, 360);
            buttonEncrypt.Margin = new Padding(3, 4, 3, 4);
            buttonEncrypt.Name = "buttonEncrypt";
            buttonEncrypt.Size = new Size(229, 40);
            buttonEncrypt.TabIndex = 3;
            buttonEncrypt.Text = "Зашифровать";
            buttonEncrypt.UseVisualStyleBackColor = false;
            buttonEncrypt.Click += buttonEncrypt_Click;
            // 
            // buttonDecrypt
            // 
            buttonDecrypt.BackColor = Color.White;
            buttonDecrypt.Cursor = Cursors.Hand;
            buttonDecrypt.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonDecrypt.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonDecrypt.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonDecrypt.FlatStyle = FlatStyle.Flat;
            buttonDecrypt.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            buttonDecrypt.ForeColor = Color.FromArgb(0, 120, 215);
            buttonDecrypt.Location = new Point(249, 360);
            buttonDecrypt.Margin = new Padding(3, 4, 3, 4);
            buttonDecrypt.Name = "buttonDecrypt";
            buttonDecrypt.Size = new Size(229, 40);
            buttonDecrypt.TabIndex = 4;
            buttonDecrypt.Text = "Расшифровать";
            buttonDecrypt.UseVisualStyleBackColor = false;
            buttonDecrypt.Click += buttonDecrypt_Click;
            // 
            // labelPublicKey
            // 
            labelPublicKey.AutoSize = true;
            labelPublicKey.Font = new Font("Segoe UI", 9F);
            labelPublicKey.ForeColor = Color.FromArgb(64, 64, 64);
            labelPublicKey.Location = new Point(14, 413);
            labelPublicKey.Name = "labelPublicKey";
            labelPublicKey.Size = new Size(0, 20);
            labelPublicKey.TabIndex = 5;
            // 
            // labelPrivateKey
            // 
            labelPrivateKey.AutoSize = true;
            labelPrivateKey.Font = new Font("Segoe UI", 9F);
            labelPrivateKey.ForeColor = Color.FromArgb(64, 64, 64);
            labelPrivateKey.Location = new Point(14, 447);
            labelPrivateKey.Name = "labelPrivateKey";
            labelPrivateKey.Size = new Size(0, 20);
            labelPrivateKey.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F);
            label1.ForeColor = Color.FromArgb(64, 64, 64);
            label1.Location = new Point(14, 16);
            label1.Name = "label1";
            label1.Size = new Size(164, 20);
            label1.TabIndex = 7;
            label1.Text = "Исходное сообщение:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F);
            label2.ForeColor = Color.FromArgb(64, 64, 64);
            label2.Location = new Point(14, 131);
            label2.Name = "label2";
            label2.Size = new Size(212, 20);
            label2.TabIndex = 8;
            label2.Text = "Зашифрованное сообщение:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F);
            label3.ForeColor = Color.FromArgb(64, 64, 64);
            label3.Location = new Point(14, 245);
            label3.Name = "label3";
            label3.Size = new Size(219, 20);
            label3.TabIndex = 9;
            label3.Text = "Расшифрованное сообщение:";
            // 
            // RSAForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(755, 544);
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
            Margin = new Padding(3, 4, 3, 4);
            Name = "RSAForm";
            Text = "RSA";
            Controls.Add(buttonInsertTestData);

            ResumeLayout(false);
            PerformLayout();
            // 
            // buttonInsertTestData
            // 
            buttonInsertTestData = new Button();
            buttonInsertTestData.BackColor = Color.FromArgb(0, 120, 215);
            buttonInsertTestData.Cursor = Cursors.Hand;
            buttonInsertTestData.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonInsertTestData.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonInsertTestData.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonInsertTestData.FlatStyle = FlatStyle.Flat;
            buttonInsertTestData.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            buttonInsertTestData.ForeColor = Color.White;
            buttonInsertTestData.Location = new Point(488, 360); // выберите удобное положение
            buttonInsertTestData.Margin = new Padding(3, 4, 3, 4);
            buttonInsertTestData.Name = "buttonInsertTestData";
            buttonInsertTestData.Size = new Size(254, 40);
            buttonInsertTestData.TabIndex = 10;
            buttonInsertTestData.Text = "Вставить тестовые данные";
            buttonInsertTestData.UseVisualStyleBackColor = false;
            buttonInsertTestData.Click += buttonInsertTestData_Click;
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
        private Button buttonInsertTestData;

    }
} 