namespace Lab2
{
    partial class DigitalSignatureForm
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
            labelPublicKey = new Label();
            labelPrivateKey = new Label();
            textBoxSignature = new TextBox();
            buttonSign = new Button();
            labelMessage = new Label();
            labelSignature = new Label();
            
            SuspendLayout();
            
            // labelPublicKey
            labelPublicKey.AutoSize = true;
            labelPublicKey.Location = new Point(12, 15);
            labelPublicKey.Name = "labelPublicKey";
            labelPublicKey.Size = new Size(200, 15);
            labelPublicKey.TabIndex = 0;
            labelPublicKey.Text = "Открытый ключ:";
            labelPublicKey.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelPublicKey.ForeColor = Color.FromArgb(64, 64, 64);
            
            // labelPrivateKey
            labelPrivateKey.AutoSize = true;
            labelPrivateKey.Location = new Point(12, 40);
            labelPrivateKey.Name = "labelPrivateKey";
            labelPrivateKey.Size = new Size(200, 15);
            labelPrivateKey.TabIndex = 1;
            labelPrivateKey.Text = "Закрытый ключ:";
            labelPrivateKey.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelPrivateKey.ForeColor = Color.FromArgb(64, 64, 64);
            
            // labelMessage
            labelMessage.AutoSize = true;
            labelMessage.Location = new Point(12, 70);
            labelMessage.Name = "labelMessage";
            labelMessage.Size = new Size(264, 15);
            labelMessage.TabIndex = 4;
            labelMessage.Text = "Сообщение: \"Красота спасет мир.\"";
            labelMessage.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelMessage.ForeColor = Color.FromArgb(64, 64, 64);
            
            // labelSignature
            labelSignature.AutoSize = true;
            labelSignature.Location = new Point(12, 120);
            labelSignature.Name = "labelSignature";
            labelSignature.Size = new Size(200, 15);
            labelSignature.TabIndex = 5;
            labelSignature.Text = "Цифровая подпись:";
            labelSignature.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelSignature.ForeColor = Color.FromArgb(64, 64, 64);
            
            // textBoxSignature
            textBoxSignature.Location = new Point(12, 140);
            textBoxSignature.Multiline = true;
            textBoxSignature.Name = "textBoxSignature";
            textBoxSignature.ReadOnly = true;
            textBoxSignature.ScrollBars = ScrollBars.Vertical;
            textBoxSignature.Size = new Size(637, 60);
            textBoxSignature.TabIndex = 2;
            textBoxSignature.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxSignature.BackColor = Color.FromArgb(245, 245, 245);
            textBoxSignature.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxSignature.BorderStyle = BorderStyle.FixedSingle;
            
            // buttonSign
            buttonSign.Location = new Point(12, 220);
            buttonSign.Name = "buttonSign";
            buttonSign.Size = new Size(200, 30);
            buttonSign.TabIndex = 3;
            buttonSign.Text = "Вычислить ЭЦП";
            buttonSign.FlatStyle = FlatStyle.Flat;
            buttonSign.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonSign.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonSign.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonSign.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonSign.BackColor = Color.FromArgb(0, 120, 215);
            buttonSign.ForeColor = Color.White;
            buttonSign.Cursor = Cursors.Hand;
            buttonSign.Click += new EventHandler(this.buttonSign_Click);
            
            // DigitalSignatureForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(661, 271);
            Controls.Add(labelMessage);
            Controls.Add(labelSignature);
            Controls.Add(labelPublicKey);
            Controls.Add(labelPrivateKey);
            Controls.Add(textBoxSignature);
            Controls.Add(buttonSign);
            Name = "DigitalSignatureForm";
            Text = "Электронная цифровая подпись RSA";
            BackColor = Color.White;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelPublicKey;
        private Label labelPrivateKey;
        private TextBox textBoxSignature;
        private Button buttonSign;
        private Label labelMessage;
        private Label labelSignature;
    }
} 