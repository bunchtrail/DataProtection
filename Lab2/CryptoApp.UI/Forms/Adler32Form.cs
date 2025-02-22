using System;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Adler32Form : Form
    {
        public Adler32Form()
        {
            InitializeComponent();
        }

        private void buttonCompute_Click(object sender, EventArgs e)
        {
            // Берём текст из поля ввода
            string inputText = textBoxInput.Text;

            // Преобразуем строку в массив байт (windows-1251)
            byte[] data = Encoding.GetEncoding("windows-1251").GetBytes(inputText);

            // Вычисляем хеш
            uint hash = ComputeAdler32(data);

            // Преобразуем в шестнадцатеричную строку
            string hashHex = hash.ToString("X8");

            // Выводим результат
            textBoxOutput.Text = hashHex;
        }

        /// <summary>
        /// Реализация алгоритма Adler-32.
        /// </summary>
        private uint ComputeAdler32(byte[] data)
        {
            const uint MOD_ADLER = 65521;

            uint A = 1;
            uint B = 0;

            foreach (byte b in data)
            {
                A = (A + b) % MOD_ADLER;
                B = (B + A) % MOD_ADLER;
            }

            // Итоговое 32-битное значение: старшие 16 бит = B, младшие 16 бит = A
            return (B << 16) | A;
        }

        private void buttonInsertTestData_Click(object sender, EventArgs e)
        {
            textBoxInput.Text = "Важно то, что дважды два четыре, а остальное все пустяки.";
        }
    }
} 