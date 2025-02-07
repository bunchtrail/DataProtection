using System;
using System.Windows.Forms;
using System.Diagnostics;
using LicenseCore.Interfaces;
using LicenseCore.Models;

namespace ProtectedApp
{
    public partial class MainForm : Form
    {
        private bool _isDebuggerDetected = false;
        private readonly IHardwareInfoProvider _hardwareInfoProvider;
        private System.Windows.Forms.Timer _updateTimer;

        public MainForm(IHardwareInfoProvider hardwareInfoProvider)
        {
            _hardwareInfoProvider = hardwareInfoProvider;
            InitializeComponent();
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
            infoLabel.Text = GetHardwareInfoText(hardwareInfo);
        }

        private string GetHardwareInfoText(HardwareInfo info)
        {
            return $"Информация об оборудовании:\n\n" +
                   $"CPU ID: {info.CpuId}\n" +
                   $"MAC-адрес: {info.MacAddress}\n" +
                   $"Серийный номер диска: {info.DiskSerialNumber}\n" +
                   $"Серийный номер материнской платы: {info.MotherboardSerialNumber}\n" +
                   $"Серийный номер BIOS: {info.BiosSerialNumber}\n" +
                   $"Windows ID: {info.WindowsId}\n\n" +
                   $"Защита от отладки активна\n" +
                   $"Время: {DateTime.Now:HH:mm:ss}";
        }

        private Label infoLabel;
        private Button exitButton;

        private void InitializeComponent()
        {
            this.Text = "Защищенное приложение";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            infoLabel = new Label
            {
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular),
                Location = new System.Drawing.Point(20, 20)
            };

            exitButton = new Button
            {
                Text = "Выход",
                Size = new System.Drawing.Size(100, 30),
                Location = new System.Drawing.Point(350, 500),
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular)
            };
            exitButton.Click += (s, e) => this.Close();

            this.Controls.Add(infoLabel);
            this.Controls.Add(exitButton);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _isDebuggerDetected = true;
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
} 