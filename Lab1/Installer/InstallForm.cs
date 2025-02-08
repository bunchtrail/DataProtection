using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace Installer
{
    public partial class InstallForm : Form
    {
        private Label titleLabel;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Button nextButton;
        private Button cancelButton;
        private TextBox pathTextBox;
        private Button browseButton;
        private Panel containerPanel;
        private Label descriptionLabel;

        private readonly string defaultPath;
        private InstallationStep currentStep = InstallationStep.Welcome;
        private bool installationCancelled = false;

        public InstallForm(string defaultInstallPath)
        {
            this.defaultPath = defaultInstallPath;
            InitializeComponents();
            UpdateUIForStep();
        }

        private void InitializeComponents()
        {
            // Настройка формы
            this.Text = "Установка Protected Application";
            this.Width = 600;
            this.Height = 400;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            // Создание основной панели
            containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            this.Controls.Add(containerPanel);

            // Заголовок
            titleLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            containerPanel.Controls.Add(titleLabel);

            // Описание
            descriptionLabel = new Label
            {
                AutoSize = true,
                Location = new Point(20, 60),
                MaximumSize = new Size(540, 0)
            };
            containerPanel.Controls.Add(descriptionLabel);

            // Путь установки
            pathTextBox = new TextBox
            {
                Location = new Point(20, 140),
                Width = 420,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pathTextBox.Text = defaultPath;
            containerPanel.Controls.Add(pathTextBox);

            // Кнопка обзора
            browseButton = new Button
            {
                Text = "Обзор...",
                Location = new Point(450, 139),
                Width = 100,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            browseButton.Click += BrowseButton_Click;
            containerPanel.Controls.Add(browseButton);

            // Статус установки
            statusLabel = new Label
            {
                AutoSize = true,
                Location = new Point(20, 200)
            };
            containerPanel.Controls.Add(statusLabel);

            // Прогресс бар
            progressBar = new ProgressBar
            {
                Location = new Point(20, 230),
                Width = 530,
                Height = 20,
                Style = ProgressBarStyle.Continuous
            };
            containerPanel.Controls.Add(progressBar);

            // Кнопки управления
            var buttonPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom
            };
            containerPanel.Controls.Add(buttonPanel);

            cancelButton = new Button
            {
                Text = "Отмена",
                Width = 100,
                Height = 30,
                Location = new Point(450, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            cancelButton.Click += CancelButton_Click;
            buttonPanel.Controls.Add(cancelButton);

            nextButton = new Button
            {
                Text = "Далее",
                Width = 100,
                Height = 30,
                Location = new Point(340, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            nextButton.Click += NextButton_Click;
            buttonPanel.Controls.Add(nextButton);

            // Скрываем элементы по умолчанию
            pathTextBox.Visible = false;
            browseButton.Visible = false;
            progressBar.Visible = false;
            statusLabel.Visible = false;
        }

        private void UpdateUIForStep()
        {
            switch (currentStep)
            {
                case InstallationStep.Welcome:
                    titleLabel.Text = "Установка Protected Application";
                    descriptionLabel.Text = "Добро пожаловать в мастер установки Protected Application.\n\n" +
                                         "Этот мастер поможет вам установить приложение на ваш компьютер.\n\n" +
                                         "Нажмите 'Далее' для продолжения или 'Отмена' для выхода из программы установки.";
                    nextButton.Text = "Далее";
                    pathTextBox.Visible = false;
                    browseButton.Visible = false;
                    progressBar.Visible = false;
                    statusLabel.Visible = false;
                    break;

                case InstallationStep.SelectPath:
                    titleLabel.Text = "Выбор папки установки";
                    descriptionLabel.Text = "Выберите папку, в которую будет установлено приложение:";
                    nextButton.Text = "Установить";
                    pathTextBox.Visible = true;
                    browseButton.Visible = true;
                    progressBar.Visible = false;
                    statusLabel.Visible = false;
                    break;

                case InstallationStep.Installing:
                    titleLabel.Text = "Установка...";
                    descriptionLabel.Text = "Пожалуйста, подождите, идет установка приложения.";
                    nextButton.Enabled = false;
                    pathTextBox.Visible = false;
                    browseButton.Visible = false;
                    progressBar.Visible = true;
                    statusLabel.Visible = true;
                    break;

                case InstallationStep.Completed:
                    titleLabel.Text = "Установка завершена";
                    descriptionLabel.Text = "Приложение успешно установлено на ваш компьютер.\n\n" +
                                         "Ярлыки созданы на рабочем столе и в меню 'Пуск'.";
                    nextButton.Text = "Завершить";
                    nextButton.Enabled = true;  // Делаем кнопку активной
                    pathTextBox.Visible = false;
                    browseButton.Visible = false;
                    progressBar.Visible = false;
                    statusLabel.Visible = false;
                    cancelButton.Visible = false;
                    break;
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            switch (currentStep)
            {
                case InstallationStep.Welcome:
                    currentStep = InstallationStep.SelectPath;
                    UpdateUIForStep();
                    break;

                case InstallationStep.SelectPath:
                    currentStep = InstallationStep.Installing;
                    UpdateUIForStep();
                    StartInstallation();
                    break;

                case InstallationStep.Completed:
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (currentStep == InstallationStep.Installing)
            {
                if (MessageBox.Show(
                    "Вы действительно хотите прервать установку?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    installationCancelled = true;
                }
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Выберите папку для установки приложения",
                UseDescriptionForTitle = true,
                SelectedPath = Path.GetDirectoryName(pathTextBox.Text)
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = Path.Combine(dialog.SelectedPath, Program.AppName);
            }
        }

        public void UpdateProgress(int progress, string status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateProgress(progress, status)));
                return;
            }

            progressBar.Value = progress;
            statusLabel.Text = status;
        }

        public void OnInstallationComplete(bool success)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnInstallationComplete(success)));
                return;
            }

            if (success)
            {
                currentStep = InstallationStep.Completed;
                UpdateUIForStep();
            }
            else
            {
                MessageBox.Show(
                    "Произошла ошибка при установке. Проверьте журнал установки для получения дополнительной информации.",
                    "Ошибка установки",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private async void StartInstallation()
        {
            try
            {
                progressBar.Value = 0;
                await Task.Run(() => Program.Install(pathTextBox.Text, this));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при установке: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        public bool IsCancelled => installationCancelled;
    }

    public enum InstallationStep
    {
        Welcome,
        SelectPath,
        Installing,
        Completed
    }
}