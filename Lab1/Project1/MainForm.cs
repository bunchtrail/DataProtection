using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using LicenseCore.Interfaces;
using LicenseCore.Models;

namespace ProtectedApp
{
    public partial class MainForm : Form
    {
        private bool _isDebuggerDetected = false;
        private readonly IHardwareInfoProvider _hardwareInfoProvider;
        private System.Windows.Forms.Timer _updateTimer;
        private Panel mainPanel;
        private Panel headerPanel;
        private Panel contentPanel;
        private Label headerLabel;
        private Panel infoPanel;
        private Label infoTitleLabel;
        private RichTextBox infoTextBox;
        private Button exitButton;
        private Label statusLabel;

        public MainForm(IHardwareInfoProvider hardwareInfoProvider)
        {
            _hardwareInfoProvider = hardwareInfoProvider;
            InitializeComponents();
            StartAntiDebugThread();
            StartInfoUpdateTimer();
        }

        private void StartAntiDebugThread()
        {
            var thread = new System.Threading.Thread(() =>
            {
                while (!_isDebuggerDetected)
                {
                    if (Debugger.IsAttached)
                    {
                        _isDebuggerDetected = true;
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("Обнаружена попытка отладки. Приложение будет закрыто.",
                                "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Application.Exit();
                        }));
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void StartInfoUpdateTimer()
        {
            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 1000; // Обновление каждую секунду
            _updateTimer.Tick += UpdateHardwareInfo;
            _updateTimer.Start();
        }

        private void UpdateHardwareInfo(object sender, EventArgs e)
        {
            var hardwareInfo = _hardwareInfoProvider.GetHardwareInfo();
            infoTextBox.Text = GetHardwareInfoText(hardwareInfo);

            // Обновляем статус защиты с анимацией
            statusLabel.Text = _isDebuggerDetected ? 
                "⚠️ Обнаружена попытка отладки!" : 
                "🛡️ Защита активна";
            statusLabel.ForeColor = _isDebuggerDetected ? 
                Color.FromArgb(231, 76, 60) : 
                Color.FromArgb(46, 204, 113);
        }

        private string GetHardwareInfoText(HardwareInfo info)
        {
            return $"CPU ID:\t\t{info.CpuId}\n\n" +
                   $"MAC-адрес:\t{info.MacAddress}\n\n" +
                   $"Серийный номер диска:\n{info.DiskSerialNumber}\n\n" +
                   $"Серийный номер материнской платы:\n{info.MotherboardSerialNumber}\n\n" +
                   $"Серийный номер BIOS:\n{info.BiosSerialNumber}\n\n" +
                   $"Windows ID:\t{info.WindowsId}\n\n" +
                   $"Время:\t\t{DateTime.Now:HH:mm:ss}";
        }

        private void InitializeComponents()
        {
            this.Text = "Protected Application";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None; // Убираем стандартную рамку
            this.BackColor = Color.FromArgb(245, 246, 247);
            this.Font = new Font("Segoe UI", 9F);

            // Основная панель с тенью
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            this.Controls.Add(mainPanel);

            // Панель заголовка
            headerPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(52, 152, 219),
                Padding = new Padding(20, 0, 20, 0)
            };
            mainPanel.Controls.Add(headerPanel);

            // Заголовок
            headerLabel = new Label
            {
                Text = "Защищенное приложение",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(headerLabel);

            // Кнопка закрытия в заголовке
            var closeButton = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(headerPanel.Width - 50, 10),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();
            headerPanel.Controls.Add(closeButton);

            // Основная панель содержимого
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            mainPanel.Controls.Add(contentPanel);

            // Панель информации
            infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            contentPanel.Controls.Add(infoPanel);

            // Заголовок информации
            infoTitleLabel = new Label
            {
                Text = "Информация о системе",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            infoPanel.Controls.Add(infoTitleLabel);

            // Текстовое поле с информацией
            infoTextBox = new RichTextBox
            {
                Location = new Point(10, 40),
                Size = new Size(infoPanel.Width - 20, infoPanel.Height - 100),
                Font = new Font("Consolas", 10F),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            infoPanel.Controls.Add(infoTextBox);

            // Статус защиты
            statusLabel = new Label
            {
                Text = "🛡️ Защита активна",
                ForeColor = Color.FromArgb(46, 204, 113),
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(10, infoPanel.Height - 40),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            infoPanel.Controls.Add(statusLabel);

            // Кнопка выхода
            exitButton = new Button
            {
                Text = "Выход",
                Size = new Size(120, 35),
                Location = new Point(infoPanel.Width - 130, infoPanel.Height - 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += (s, e) => this.Close();
            infoPanel.Controls.Add(exitButton);

            // Добавляем возможность перетаскивания окна
            headerPanel.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    NativeWin32.ReleaseCapture();
                    NativeWin32.SendMessage(Handle, NativeWin32.WM_NCLBUTTONDOWN, NativeWin32.HT_CAPTION, 0);
                }
            };

            // Перекрашивание кнопок при наведении
            foreach (var button in new[] { exitButton, closeButton })
            {
                button.MouseEnter += (s, e) =>
                {
                    if (button == exitButton)
                        button.BackColor = Color.FromArgb(192, 57, 43);
                    else
                        button.BackColor = Color.FromArgb(231, 76, 60);
                };
                button.MouseLeave += (s, e) =>
                {
                    if (button == exitButton)
                        button.BackColor = Color.FromArgb(231, 76, 60);
                    else
                        button.BackColor = Color.FromArgb(52, 152, 219);
                };
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _isDebuggerDetected = true;
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }

    // Вспомогательный класс для перетаскивания окна
    internal static class NativeWin32
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}