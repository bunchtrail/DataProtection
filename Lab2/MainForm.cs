using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab2;

public partial class MainForm : Form
{
    private Form? currentForm;
    private readonly Color[] taskColors = new Color[] 
    {
        Color.FromArgb(0, 120, 215),    // Синий
        Color.FromArgb(0, 153, 188),    // Бирюзовый
        Color.FromArgb(0, 120, 215),    // Синий (RSA)
        Color.FromArgb(0, 153, 188),    // Бирюзовый (Adler32)
        Color.FromArgb(122, 117, 116)   // Серый (для неактивных задач)
    };

    public MainForm()
    {
        InitializeComponent();
        LoadTasks();
    }

    private void LoadTasks()
    {
        comboBoxTasks.Items.Add("Задание №1: Шифр Квадрат Полибия");
        comboBoxTasks.Items.Add("Задание №2: Шифрование Кузнечик");
        comboBoxTasks.Items.Add("Задание №3: RSA");
        comboBoxTasks.Items.Add("Задание №4: Функция Adler32");
        comboBoxTasks.Items.Add("Задание №5: Электронная цифровая подпись");

        comboBoxTasks.SelectedIndex = 0;
    }

    private void comboBoxTasks_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        // Получаем ComboBox
        ComboBox combo = (ComboBox)sender;
        string text = combo.Items[e.Index].ToString() ?? string.Empty;
        
        // Определяем цвета
        Color textColor = taskColors[e.Index];
        Color backColor = e.BackColor;
        
        // При наведении или выборе меняем цвет фона
        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected ||
            (e.State & DrawItemState.Focus) == DrawItemState.Focus)
        {
            backColor = Color.FromArgb(229, 241, 251);
            using var backBrush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(backBrush, e.Bounds);
        }
        else
        {
            using var backBrush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(backBrush, e.Bounds);
        }
        
        // Рисуем маркер слева
        using (var markerBrush = new SolidBrush(textColor))
        {
            var markerBounds = new Rectangle(
                e.Bounds.X + 3,
                e.Bounds.Y + 4,
                3,
                e.Bounds.Height - 8
            );
            e.Graphics.FillRectangle(markerBrush, markerBounds);
        }
        
        // Рисуем текст
        var textBounds = new Rectangle(
            e.Bounds.X + 10,
            e.Bounds.Y,
            e.Bounds.Width - 12,
            e.Bounds.Height
        );
        
        TextRenderer.DrawText(
            e.Graphics,
            text,
            e.Font,
            textBounds,
            textColor,
            backColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left
        );
    }

    private void LoadFormToPanel(Form form)
    {
        currentForm?.Close();
        currentForm = form;
        
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        
        contentPanel.Controls.Clear();
        contentPanel.Controls.Add(form);
        form.Show();
        
        buttonBack.Visible = true;
        comboBoxTasks.Enabled = false;
        buttonOpenTask.Enabled = false;
    }

    private void buttonOpenTask_Click(object sender, EventArgs e)
    {
        Form? taskForm = null;
        string selectedTask = comboBoxTasks.SelectedItem?.ToString() ?? string.Empty;

        if (selectedTask.Contains("Задание №1"))
        {
            taskForm = new PolybiusForm();
        }
        else if (selectedTask.Contains("Задание №2"))
        {
            taskForm = new KuznechikForm();
        }
        else if (selectedTask.Contains("Задание №3"))
        {
            taskForm = new RSAForm();
        }
        else if (selectedTask.Contains("Задание №4"))
        {
            taskForm = new Adler32Form();
        }
        else if (selectedTask.Contains("Задание №5"))
        {
            MessageBox.Show("Задание №5 будет реализовано позже");
            return;
        }

        if (taskForm is not null)
        {
            LoadFormToPanel(taskForm);
        }
    }

    private void buttonBack_Click(object sender, EventArgs e)
    {
        currentForm?.Close();
        currentForm = null;
        contentPanel.Controls.Clear();
        
        buttonBack.Visible = false;
        comboBoxTasks.Enabled = true;
        buttonOpenTask.Enabled = true;
    }
}