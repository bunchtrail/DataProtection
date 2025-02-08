using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Uninstaller
{
    public partial class UninstallForm : Form
    {
        private const string AppName = "ProtectedApp";
        private readonly string InstallPath;
        private readonly string LicenseFolderPath;
        
        private Label titleLabel;
        private Label descriptionLabel;
        private ProgressBar progressBar;
        private Label statusLabel;
        private Button nextButton;
        private Button cancelButton;
        private Panel containerPanel;

        private UninstallStep currentStep = UninstallStep.Welcome;
        private bool uninstallCancelled = false;

        public UninstallForm(string installPath, string licensePath)
        {
            InstallPath = installPath;
            LicenseFolderPath = licensePath;
            InitializeComponents();
            UpdateUIForStep();
        }

        private void InitializeComponents()
        {
            // Настройка формы
            this.Text = "Удаление Protected Application";
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

            // Статус
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

            // Кнопка отмены
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

            // Кнопка далее/удалить/завершить
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
            progressBar.Visible = false;
            statusLabel.Visible = false;
        }

        private void UpdateUIForStep()
        {
            switch (currentStep)
            {
                case UninstallStep.Welcome:
                    titleLabel.Text = "Удаление Protected Application";
                    descriptionLabel.Text = "Вас приветствует мастер удаления Protected Application.\n\n" +
                                         "Этот мастер поможет вам удалить приложение с вашего компьютера.\n\n" +
                                         "Нажмите 'Далее' для продолжения или 'Отмена' для выхода.";
                    nextButton.Text = "Далее";
                    nextButton.Enabled = true;
                    progressBar.Visible = false;
                    statusLabel.Visible = false;
                    break;

                case UninstallStep.Confirm:
                    titleLabel.Text = "Подтверждение удаления";
                    descriptionLabel.Text = "Вы уверены, что хотите удалить Protected Application и все его компоненты?\n\n" +
                                         "Это действие нельзя будет отменить.";
                    nextButton.Text = "Удалить";
                    nextButton.Enabled = true;
                    progressBar.Visible = false;
                    statusLabel.Visible = false;
                    break;

                case UninstallStep.Uninstalling:
                    titleLabel.Text = "Удаление...";
                    descriptionLabel.Text = "Пожалуйста, подождите, идет удаление приложения.";
                    nextButton.Enabled = false;
                    progressBar.Visible = true;
                    statusLabel.Visible = true;
                    break;

                case UninstallStep.Completed:
                    titleLabel.Text = "Удаление завершено";
                    descriptionLabel.Text = "Приложение успешно удалено с вашего компьютера.";
                    nextButton.Text = "Завершить";
                    nextButton.Enabled = true;
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
                case UninstallStep.Welcome:
                    currentStep = UninstallStep.Confirm;
                    UpdateUIForStep();
                    break;

                case UninstallStep.Confirm:
                    currentStep = UninstallStep.Uninstalling;
                    UpdateUIForStep();
                    StartUninstallation();
                    break;

                case UninstallStep.Completed:
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (currentStep == UninstallStep.Uninstalling)
            {
                if (MessageBox.Show(
                    "Вы действительно хотите прервать процесс удаления?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    uninstallCancelled = true;
                }
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
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

        public void OnUninstallComplete(bool success)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnUninstallComplete(success)));
                return;
            }

            if (success)
            {
                currentStep = UninstallStep.Completed;
                UpdateUIForStep();
            }
            else
            {
                MessageBox.Show(
                    "Произошла ошибка при удалении. Возможно, некоторые компоненты не были удалены.",
                    "Ошибка удаления",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private async void StartUninstallation()
        {
            try
            {
                progressBar.Value = 0;
                await Task.Run(() => Uninstall());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при удалении: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void Uninstall()
        {
            try
            {
                // Закрываем все процессы приложения
                UpdateProgress(0, "Завершение процессов приложения...");
                foreach (var process in Process.GetProcessesByName(AppName))
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch { }
                }

                if (uninstallCancelled) return;

                // Создаем батник для удаления
                UpdateProgress(20, "Подготовка к удалению файлов...");
                var batchPath = Path.Combine(Path.GetTempPath(), "uninstall.bat");
                var sb = new StringBuilder();
                sb.AppendLine("@echo off");
                sb.AppendLine("timeout /t 1 /nobreak > nul");
                
                UpdateProgress(40, "Удаление ярлыков...");
                // Удаляем ярлыки
                var startMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs",
                    $"{AppName}.lnk");
                sb.AppendLine($"if exist \"{startMenuPath}\" del /f /q \"{startMenuPath}\"");

                var desktopPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"{AppName}.lnk");
                sb.AppendLine($"if exist \"{desktopPath}\" del /f /q \"{desktopPath}\"");

                if (uninstallCancelled) return;

                UpdateProgress(60, "Удаление файлов лицензии...");
                // Удаляем лицензию
                sb.AppendLine($"if exist \"{LicenseFolderPath}\" rmdir /s /q \"{LicenseFolderPath}\"");

                if (uninstallCancelled) return;

                UpdateProgress(80, "Удаление файлов приложения...");
                // Удаляем основную папку установки
                sb.AppendLine($"if exist \"{InstallPath}\" rmdir /s /q \"{InstallPath}\"");

                // Удаляем сам батник
                sb.AppendLine("(goto) 2>nul & del \"%~f0\"");

                File.WriteAllText(batchPath, sb.ToString());

                UpdateProgress(90, "Завершение процесса удаления...");
                // Запускаем батник для удаления
                var startInfo = new ProcessStartInfo
                {
                    FileName = batchPath,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(startInfo);

                if (uninstallCancelled) return;

                UpdateProgress(100, "Удаление завершено");
                OnUninstallComplete(true);
            }
            catch (Exception ex)
            {
                OnUninstallComplete(false);
                throw;
            }
        }

        public bool IsCancelled => uninstallCancelled;
    }

    public enum UninstallStep
    {
        Welcome,
        Confirm,
        Uninstalling,
        Completed
    }
}