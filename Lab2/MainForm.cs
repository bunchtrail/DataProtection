using System;
using System.Windows.Forms;

namespace Lab2;

public partial class MainForm : Form
{
    private Form? currentForm;

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
            MessageBox.Show("Задание №3 будет реализовано позже");
            return;
        }
        else if (selectedTask.Contains("Задание №4"))
        {
            MessageBox.Show("Задание №4 будет реализовано позже");
            return;
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