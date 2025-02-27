using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
    // Форма для декодирования текста, закодированного с помощью квадрата Полибия
    public partial class PolybiusForm : Form
    {
        // Размер квадрата Полибия (6x6)
        private const int MATRIX_SIZE = 6;

        // Тестовые данные из задания:
        // Текст для декодирования – последовательность числовых кодов (координат в квадрате)
        private const string TestEncodedText = "3 32 42 63 41 41 51 16 52 46 53 32 45 11 61 43 46 53 31 66 51 23 42 54 21 64 15 66 66 56 14 23 46 53 31 63 61 12 34 33 23 33 46 4";
        // Матрица символов, где через запятую разделены строки квадрата Полибия
        private const string TestMatrix = "АБВГДЕ, ЁЖЗИЙК, ЛМНОПР, СТУФХЦ, ЧШЩЪЫЬ, ЭЮЯ. ,";

        // Конструктор формы. Инициализирует компоненты формы.
        public PolybiusForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Метод для проверки корректности матрицы символов.
        /// Проверяет, что матрица состоит из 6 строк, каждая строка (кроме последней) содержит ровно 6 символов.
        /// Последняя строка может быть короче, но если да, то дополняется пробелами.
        /// </summary>
        /// <param name="rows">Массив строк, введённых пользователем</param>
        /// <param name="errorMessage">Сообщение об ошибке в случае некорректного ввода</param>
        /// <param name="processedRows">Обработанный массив строк (с дополненными пробелами, если необходимо)</param>
        /// <returns>Возвращает true, если матрица корректна, иначе false</returns>
        private bool ValidateMatrix(string[] rows, out string errorMessage, out string[] processedRows)
        {
            errorMessage = string.Empty;
            processedRows = rows.ToArray();

            // Проверяем, что число строк равно ожидаемому размеру (6)
            if (rows.Length != MATRIX_SIZE)
            {
                errorMessage = $"Матрица должна состоять из {MATRIX_SIZE} строк.";
                return false;
            }

            // Проверяем первые 5 строк матрицы
            for (int i = 0; i < MATRIX_SIZE - 1; i++)
            {
                // Если строка пуста или содержит только пробелы, выводим ошибку
                if (string.IsNullOrWhiteSpace(rows[i]))
                {
                    errorMessage = $"Строка {i + 1} матрицы пуста.";
                    return false;
                }

                // Если длина строки не равна 6 символам, выводим ошибку
                if (rows[i].Length != MATRIX_SIZE)
                {
                    errorMessage = $"Строка {i + 1} матрицы должна содержать {MATRIX_SIZE} символов. Текущая длина: {rows[i].Length}";
                    return false;
                }
            }

            // Обработка последней строки матрицы
            // Последняя строка не должна быть пустой
            if (string.IsNullOrWhiteSpace(rows[MATRIX_SIZE - 1]))
            {
                errorMessage = "Последняя строка матрицы не может быть пустой.";
                return false;
            }

            // Последняя строка не должна превышать 6 символов
            if (rows[MATRIX_SIZE - 1].Length > MATRIX_SIZE)
            {
                errorMessage = $"Последняя строка матрицы не может содержать больше {MATRIX_SIZE} символов. Текущая длина: {rows[MATRIX_SIZE - 1].Length}";
                return false;
            }

            // Если последняя строка короче 6 символов, дополняем её справа пробелами
            if (rows[MATRIX_SIZE - 1].Length < MATRIX_SIZE)
            {
                processedRows[MATRIX_SIZE - 1] = rows[MATRIX_SIZE - 1].PadRight(MATRIX_SIZE, ' ');
            }

            return true;
        }

        /// <summary>
        /// Обработчик события нажатия кнопки "Декодировать".
        /// Выполняет декодирование введённого текста, используя квадрат Полибия.
        /// </summary>
        private void buttonDecode_Click(object sender, EventArgs e)
        {
            // Получаем введённый пользователем текст для декодирования и матрицу символов
            string encodedText = textBoxEncoded.Text.Trim();
            string matrixText = textBoxMatrix.Text.Trim();

            // Если матрица не введена, выводим сообщение об ошибке
            if (string.IsNullOrWhiteSpace(matrixText))
            {
                MessageBox.Show("Введите матрицу символов.");
                return;
            }

            // Если текст для декодирования не введён, выводим сообщение об ошибке
            if (string.IsNullOrWhiteSpace(encodedText))
            {
                MessageBox.Show("Введите текст для декодирования.");
                return;
            }
            //!!!!!!------
            // Разбиваем введённую строку матрицы на отдельные строки, используя запятую в качестве разделителя
            string[] rows = matrixText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())
                                    .ToArray();

            // Валидируем матрицу, чтобы убедиться, что она соответствует необходимому формату
            if (!ValidateMatrix(rows, out string errorMessage, out string[] processedRows))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            // Инициализируем двумерный массив для хранения квадрата Полибия
            char[,] polybiusSquare = new char[MATRIX_SIZE, MATRIX_SIZE];
            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                for (int j = 0; j < MATRIX_SIZE; j++)
                {
                    // Заполняем квадрат Полибия символами из обработанных строк
                    polybiusSquare[i, j] = processedRows[i][j];
                }
            }

            // Разбиваем закодированный текст на отдельные коды (токены), разделённые пробелами
            string[] tokens = encodedText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Используем StringBuilder для формирования декодированного сообщения
            StringBuilder decodedMessage = new StringBuilder();
            foreach (string token in tokens)
            {
                // Если токен состоит из одного символа, добавляем ведущий ноль (например, "3" становится "03")
                string processedToken = token.Length == 1 ? "0" + token : token;

                // Если длина токена после обработки не равна 2, выводим сообщение об ошибке
                if (processedToken.Length != 2)
                {
                    MessageBox.Show($"Некорректный код: {token}");
                    return;
                }

                // Преобразуем каждый символ токена в число для определения координат в квадрате
                if (!int.TryParse(processedToken[0].ToString(), out int rowDigit) ||
                    !int.TryParse(processedToken[1].ToString(), out int colDigit))
                {
                    MessageBox.Show($"Некорректный код: {token}");
                    return;
                }

                // Если токен состоял из одного символа, по умолчанию считаем, что он относится к первой строке
                if (token.Length == 1)
                {
                    rowDigit = 1;
                }

                // Проверяем, что полученные координаты находятся в допустимом диапазоне (от 1 до 6)
                if (rowDigit < 1 || rowDigit > MATRIX_SIZE || colDigit < 1 || colDigit > MATRIX_SIZE)
                {
                    MessageBox.Show($"Некорректные координаты: {token}. Допустимый диапазон: 1-{MATRIX_SIZE}");
                    return;
                }

                // Преобразуем координаты из 1-индексации в 0-индексацию для доступа к элементам массива
                int rowIndex = rowDigit - 1;
                int colIndex = colDigit - 1;

                // Извлекаем символ из квадрата Полибия по заданным координатам и добавляем его к результату
                decodedMessage.Append(polybiusSquare[rowIndex, colIndex]);
            }

            // Выводим декодированное сообщение в текстовое поле
            textBoxDecoded.Text = decodedMessage.ToString();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки для вставки тестовых данных.
        /// Заполняет поля ввода тестовыми данными для удобства проверки работы алгоритма.
        /// </summary>
        private void buttonInsertTestData_Click(object sender, EventArgs e)
        {
            // Устанавливаем тестовый зашифрованный текст
            textBoxEncoded.Text = TestEncodedText;
            // Устанавливаем тестовую матрицу символов
            textBoxMatrix.Text = TestMatrix;
        }
    }
}
