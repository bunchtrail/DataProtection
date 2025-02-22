namespace Lab2
{
    partial class MainForm
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
            comboBoxTasks = new ComboBox();
            buttonOpenTask = new Button();
            contentPanel = new Panel();
            buttonBack = new Button();
            SuspendLayout();

            // comboBoxTasks
            comboBoxTasks.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTasks.FormattingEnabled = true;
            comboBoxTasks.Location = new Point(12, 12);
            comboBoxTasks.Name = "comboBoxTasks";
            comboBoxTasks.Size = new Size(580, 25);
            comboBoxTasks.TabIndex = 0;
            comboBoxTasks.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            comboBoxTasks.BackColor = Color.White;
            comboBoxTasks.ForeColor = Color.FromArgb(64, 64, 64);
            comboBoxTasks.DrawMode = DrawMode.OwnerDrawFixed;
            comboBoxTasks.ItemHeight = 25;
            comboBoxTasks.DrawItem += comboBoxTasks_DrawItem;

            // buttonOpenTask
            buttonOpenTask.Location = new Point(598, 12);
            buttonOpenTask.Name = "buttonOpenTask";
            buttonOpenTask.Size = new Size(75, 25);
            buttonOpenTask.TabIndex = 1;
            buttonOpenTask.Text = "Открыть";
            buttonOpenTask.FlatStyle = FlatStyle.Flat;
            buttonOpenTask.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonOpenTask.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonOpenTask.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonOpenTask.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonOpenTask.BackColor = Color.White;
            buttonOpenTask.ForeColor = Color.FromArgb(0, 120, 215);
            buttonOpenTask.Cursor = Cursors.Hand;
            buttonOpenTask.UseVisualStyleBackColor = false;
            buttonOpenTask.Click += buttonOpenTask_Click;

            // contentPanel
            contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            contentPanel.Location = new Point(12, 41);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(661, 408);
            contentPanel.TabIndex = 2;
            contentPanel.BackColor = Color.White;
            contentPanel.Padding = new Padding(1);
            contentPanel.BorderStyle = BorderStyle.FixedSingle;

            // buttonBack
            buttonBack.Location = new Point(679, 12);
            buttonBack.Name = "buttonBack";
            buttonBack.Size = new Size(75, 25);
            buttonBack.TabIndex = 3;
            buttonBack.Text = "Назад";
            buttonBack.FlatStyle = FlatStyle.Flat;
            buttonBack.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonBack.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251);
            buttonBack.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
            buttonBack.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonBack.BackColor = Color.White;
            buttonBack.ForeColor = Color.FromArgb(0, 120, 215);
            buttonBack.Cursor = Cursors.Hand;
            buttonBack.UseVisualStyleBackColor = false;
            buttonBack.Visible = false;
            buttonBack.Click += buttonBack_Click;

            // MainForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(buttonBack);
            Controls.Add(contentPanel);
            Controls.Add(buttonOpenTask);
            Controls.Add(comboBoxTasks);
            Name = "MainForm";
            Text = "Лабораторная работа №2";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Padding = new Padding(20);
            ResumeLayout(false);
            PerformLayout();
        }

        private ComboBox comboBoxTasks;
        private Button buttonOpenTask;
        private Panel contentPanel;
        private Button buttonBack;
    }
}