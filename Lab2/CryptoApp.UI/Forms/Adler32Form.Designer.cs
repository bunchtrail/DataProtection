namespace Lab2
{
    partial class Adler32Form
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
            textBoxOutput = new TextBox();
            buttonCompute = new Button();
            buttonInsertTestData = new Button();
            label1 = new Label();
            label2 = new Label();
            
            SuspendLayout();
            
            // textBoxInput
            textBoxInput.Location = new Point(12, 32);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.ScrollBars = ScrollBars.Vertical;
            textBoxInput.Size = new Size(637, 100);
            textBoxInput.TabIndex = 0;
            textBoxInput.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxInput.BackColor = Color.White;
            textBoxInput.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxInput.BorderStyle = BorderStyle.FixedSingle;
            
            // textBoxOutput
            textBoxOutput.Location = new Point(12, 158);
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.Size = new Size(637, 23);
            textBoxOutput.TabIndex = 1;
            textBoxOutput.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxOutput.BackColor = Color.FromArgb(245, 245, 245);
            textBoxOutput.ForeColor = Color.FromArgb(64, 64, 64);
            textBoxOutput.BorderStyle = BorderStyle.FixedSingle;
            textBoxOutput.TextAlign = HorizontalAlignment.Center;
            
            // buttonCompute
            buttonCompute.Location = new Point(12, 200);
            buttonCompute.Name = "buttonCompute";
            buttonCompute.Size = new Size(200, 30);
            buttonCompute.TabIndex = 2;
            buttonCompute.Text = "Вычислить хеш";
            buttonCompute.FlatStyle = FlatStyle.Flat;
            buttonCompute.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonCompute.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonCompute.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonCompute.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonCompute.BackColor = Color.FromArgb(0, 120, 215);
            buttonCompute.ForeColor = Color.White;
            buttonCompute.Cursor = Cursors.Hand;
            buttonCompute.Click += buttonCompute_Click;
            
            // buttonInsertTestData
            buttonInsertTestData.Location = new Point(218, 200);
            buttonInsertTestData.Name = "buttonInsertTestData";
            buttonInsertTestData.Size = new Size(200, 30);
            buttonInsertTestData.TabIndex = 3;
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
            label1.TabIndex = 4;
            label1.Text = "Исходное сообщение:";
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(64, 64, 64);
            
            // label2
            label2.AutoSize = true;
            label2.Location = new Point(12, 138);
            label2.Name = "label2";
            label2.Size = new Size(200, 15);
            label2.TabIndex = 5;
            label2.Text = "Хеш Adler-32 (hex):";
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(64, 64, 64);
            
            // Adler32Form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(661, 408);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(buttonInsertTestData);
            Controls.Add(buttonCompute);
            Controls.Add(textBoxOutput);
            Controls.Add(textBoxInput);
            Name = "Adler32Form";
            Text = "Adler-32";
            BackColor = Color.White;
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox textBoxInput;
        private TextBox textBoxOutput;
        private Button buttonCompute;
        private Button buttonInsertTestData;
        private Label label1;
        private Label label2;
    }
} 