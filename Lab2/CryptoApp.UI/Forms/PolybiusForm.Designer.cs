namespace Lab2
{
    partial class PolybiusForm
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
            textBoxEncoded = new TextBox();
            textBoxMatrix = new TextBox();
            textBoxDecoded = new TextBox();
            buttonDecode = new Button();
            buttonInsertTestData = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            
            SuspendLayout();
            
            // textBoxEncoded
            textBoxEncoded.Location = new Point(12, 32);
            textBoxEncoded.Multiline = true;
            textBoxEncoded.Name = "textBoxEncoded";
            textBoxEncoded.ScrollBars = ScrollBars.Vertical;
            textBoxEncoded.Size = new Size(637, 60);
            textBoxEncoded.TabIndex = 0;
            textBoxEncoded.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxEncoded.BackColor = Color.White;
            textBoxEncoded.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxEncoded.BorderStyle = BorderStyle.FixedSingle;
            
            // textBoxMatrix
            textBoxMatrix.Location = new Point(12, 118);
            textBoxMatrix.Multiline = true;
            textBoxMatrix.Name = "textBoxMatrix";
            textBoxMatrix.ScrollBars = ScrollBars.Vertical;
            textBoxMatrix.Size = new Size(637, 60);
            textBoxMatrix.TabIndex = 1;
            textBoxMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxMatrix.BackColor = Color.White;
            textBoxMatrix.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxMatrix.BorderStyle = BorderStyle.FixedSingle;
            
            // textBoxDecoded
            textBoxDecoded.Location = new Point(12, 204);
            textBoxDecoded.Multiline = true;
            textBoxDecoded.Name = "textBoxDecoded";
            textBoxDecoded.ReadOnly = true;
            textBoxDecoded.ScrollBars = ScrollBars.Vertical;
            textBoxDecoded.Size = new Size(637, 60);
            textBoxDecoded.TabIndex = 2;
            textBoxDecoded.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxDecoded.BackColor = Color.FromArgb(245, 245, 245);
            textBoxDecoded.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxDecoded.BorderStyle = BorderStyle.FixedSingle;
            
            // buttonDecode
            buttonDecode.Location = new Point(12, 270);
            buttonDecode.Name = "buttonDecode";
            buttonDecode.Size = new Size(200, 30);
            buttonDecode.TabIndex = 3;
            buttonDecode.Text = "Декодировать";
            buttonDecode.FlatStyle = FlatStyle.Flat;
            buttonDecode.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonDecode.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonDecode.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonDecode.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonDecode.BackColor = Color.FromArgb(0, 120, 215);
            buttonDecode.ForeColor = Color.White;
            buttonDecode.Cursor = Cursors.Hand;
            buttonDecode.Click += buttonDecode_Click;
            
            // buttonInsertTestData
            buttonInsertTestData.Location = new Point(218, 270);
            buttonInsertTestData.Name = "buttonInsertTestData";
            buttonInsertTestData.Size = new Size(200, 30);
            buttonInsertTestData.TabIndex = 4;
            buttonInsertTestData.Text = "Вставить тестовые данные";
            buttonInsertTestData.FlatStyle = FlatStyle.Flat;
            buttonInsertTestData.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonInsertTestData.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonInsertTestData.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonInsertTestData.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonInsertTestData.BackColor = Color.White;
            buttonInsertTestData.ForeColor = Color.FromArgb(0, 120, 215);
            buttonInsertTestData.Cursor = Cursors.Hand;
            buttonInsertTestData.Click += buttonInsertTestData_Click;
            
            // label1
            label1.AutoSize = true;
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(200, 15);
            label1.TabIndex = 5;
            label1.Text = "Закодированный текст:";
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(64, 64, 64);
            
            // label2
            label2.AutoSize = true;
            label2.Location = new Point(12, 98);
            label2.Name = "label2";
            label2.Size = new Size(200, 15);
            label2.TabIndex = 6;
            label2.Text = "Матрица символов (через запятую):";
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(64, 64, 64);
            
            // label3
            label3.AutoSize = true;
            label3.Location = new Point(12, 184);
            label3.Name = "label3";
            label3.Size = new Size(200, 15);
            label3.TabIndex = 7;
            label3.Text = "Декодированный текст:";
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.FromArgb(64, 64, 64);
            
            // PolybiusForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(661, 408);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(buttonInsertTestData);
            Controls.Add(buttonDecode);
            Controls.Add(textBoxDecoded);
            Controls.Add(textBoxMatrix);
            Controls.Add(textBoxEncoded);
            Name = "PolybiusForm";
            Text = "Шифр Полибия";
            BackColor = Color.White;
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox textBoxEncoded;
        private TextBox textBoxMatrix;
        private TextBox textBoxDecoded;
        private Button buttonDecode;
        private Button buttonInsertTestData;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}