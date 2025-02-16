using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
    public partial class PolybiusForm : Form
    {
        private const int MATRIX_SIZE = 6;
        
        // Тестовые данные из задания
        private const string TestEncodedText = "3 32 42 63 41 41 51 16 52 46 53 32 45 11 61 43 46 53 31 66 51 23 42 54 21 64 15 66 66 56 14 23 46 53 31 63 61 12 34 33 23 33 46 4";
        private const string TestMatrix = "АБВГДЕ, ЁЖЗИЙК, ЛМНОПР, СТУФХЦ, ЧШЩЪЫЬ, ЭЮЯ. ,";

        public PolybiusForm()
        {
            InitializeComponent();
        }

        private bool ValidateMatrix(string[] rows, out string errorMessage, out string[] processedRows)
        {
            errorMessage = string.Empty;
            processedRows = rows.ToArray();
            
            if (rows.Length != MATRIX_SIZE)
            {
                errorMessage = $"Матрица должна состоять из {MATRIX_SIZE} строк.";
                return false;
            }

            // Проверяем первые 5 строк (с индексами 0-4)
            for (int i = 0; i < MATRIX_SIZE - 1; i++)
            {
                if (string.IsNullOrWhiteSpace(rows[i]))
                {
                    errorMessage = $"Строка {i + 1} матрицы пуста.";
                    return false;
                }

                if (rows[i].Length != MATRIX_SIZE)
                {
                    errorMessage = $"Строка {i + 1} матрицы должна содержать {MATRIX_SIZE} символов. Текущая длина: {rows[i].Length}";
                    return false;
                }
            }

            // Особая обработка последней строки
            if (string.IsNullOrWhiteSpace(rows[MATRIX_SIZE - 1]))
            {
                errorMessage = "Последняя строка матрицы не может быть пустой.";
                return false;
            }

            if (rows[MATRIX_SIZE - 1].Length > MATRIX_SIZE)
            {
                errorMessage = $"Последняя строка матрицы не может содержать больше {MATRIX_SIZE} символов. Текущая длина: {rows[MATRIX_SIZE - 1].Length}";
                return false;
            }

            // Если последняя строка короче MATRIX_SIZE, дополняем её пробелами
            if (rows[MATRIX_SIZE - 1].Length < MATRIX_SIZE)
            {
                processedRows[MATRIX_SIZE - 1] = rows[MATRIX_SIZE - 1].PadRight(MATRIX_SIZE, ' ');
            }

            return true;
        }

        private void buttonDecode_Click(object sender, EventArgs e)
        {
            string encodedText = textBoxEncoded.Text.Trim();
            string matrixText = textBoxMatrix.Text.Trim();

            if (string.IsNullOrWhiteSpace(matrixText))
            {
                MessageBox.Show("Введите матрицу символов.");
                return;
            }

            if (string.IsNullOrWhiteSpace(encodedText))
            {
                MessageBox.Show("Введите текст для декодирования.");
                return;
            }

            string[] rows = matrixText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())
                                    .ToArray();

            if (!ValidateMatrix(rows, out string errorMessage, out string[] processedRows))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            char[,] polybiusSquare = new char[MATRIX_SIZE, MATRIX_SIZE];
            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                for (int j = 0; j < MATRIX_SIZE; j++)
                {
                    polybiusSquare[i, j] = processedRows[i][j];
                }
            }

            string[] tokens = encodedText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder decodedMessage = new StringBuilder();
            foreach (string token in tokens)
            {
                string processedToken = token.Length == 1 ? "0" + token : token;

                if (processedToken.Length != 2)
                {
                    MessageBox.Show($"Некорректный код: {token}");
                    return;
                }

                if (!int.TryParse(processedToken[0].ToString(), out int rowDigit) ||
                    !int.TryParse(processedToken[1].ToString(), out int colDigit))
                {
                    MessageBox.Show($"Некорректный код: {token}");
                    return;
                }

                if (token.Length == 1)
                {
                    rowDigit = 1;
                }

                if (rowDigit < 1 || rowDigit > MATRIX_SIZE || colDigit < 1 || colDigit > MATRIX_SIZE)
                {
                    MessageBox.Show($"Некорректные координаты: {token}. Допустимый диапазон: 1-{MATRIX_SIZE}");
                    return;
                }

                int rowIndex = rowDigit - 1;
                int colIndex = colDigit - 1;

                decodedMessage.Append(polybiusSquare[rowIndex, colIndex]);
            }

            textBoxDecoded.Text = decodedMessage.ToString();
        }

        private void buttonInsertTestData_Click(object sender, EventArgs e)
        {
            textBoxEncoded.Text = TestEncodedText;
            textBoxMatrix.Text = TestMatrix;
        }
    }
}