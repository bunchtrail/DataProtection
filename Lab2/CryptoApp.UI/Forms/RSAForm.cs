using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
    // Форма для демонстрации работы алгоритма RSA: шифрование и расшифровка текста.
    public partial class RSAForm : Form
    {
        // Заданные в условии простые числа P и Q
        private const int P = 337;
        private const int Q = 461;

        // Открытый ключ состоит из пары (e, n), а закрытый – из пары (d, n)
        private BigInteger e; // Открытая экспонента
        private BigInteger d; // Закрытая экспонента
        private BigInteger n; // Модуль, равный произведению P и Q

        public RSAForm()
        {
            InitializeComponent();

            // При загрузке формы сразу генерируем RSA-ключи
            GenerateKeys();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Зашифровать".
        /// Берёт введённое сообщение, шифрует его с помощью RSA и выводит результат.
        /// </summary>
        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            string message = textBoxInput.Text;

            // Получаем зашифрованное сообщение в виде списка числовых блоков
            List<BigInteger> encryptedBlocks = EncryptMessage(message);

            // Преобразуем список блоков в строку (разделяем пробелами) для вывода
            string cipherText = string.Join(" ", encryptedBlocks);

            textBoxEncrypted.Text = cipherText;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Расшифровать".
        /// Читает зашифрованный текст, преобразует его в числовые блоки,
        /// выполняет дешифрование и выводит исходное сообщение.
        /// </summary>
        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                // Разбиваем строку зашифрованного текста на отдельные числовые блоки
                List<BigInteger> encryptedBlocks = textBoxEncrypted.Text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => BigInteger.Parse(s))
                    .ToList();

                // Расшифровываем блоки и получаем исходное сообщение
                string decryptedMessage = DecryptMessage(encryptedBlocks);

                textBoxDecrypted.Text = decryptedMessage;
            }
            catch (Exception ex)
            {
                // Выводим сообщение об ошибке в случае проблем с расшифровкой
                MessageBox.Show($"Ошибка при расшифровании: {ex.Message}");
            }
        }

        /// <summary>
        /// Генерация открытого и закрытого ключей RSA.
        /// Шаги:
        /// 1. Вычисление n = P * Q и функции Эйлера phi(n) = (P - 1) * (Q - 1).
        /// 2. Выбор открытой экспоненты e (чаще всего используют 65537).
        /// 3. Проверка взаимной простоты e и phi(n).
        /// 4. Вычисление закрытой экспоненты d, являющейся мультипликативной обратной к e по модулю phi(n).
        /// </summary>
        private void GenerateKeys()
        {
            // 1. Вычисляем модуль n и значение функции Эйлера phi(n)
            n = P * Q;                      // Например: 337 * 461 = 155357
            BigInteger phi = (P - 1) * (Q - 1);  // Например: 336 * 460 = 154560, определяет количество чисел, взаимно простых с n

            // 2. Выбираем стандартное значение открытой экспоненты e
            e = 65537;

            // 3. Проверяем, что e и phi(n) взаимно просты (их НОД должен быть 1)
            if (BigInteger.GreatestCommonDivisor(e, phi) != 1)
            {
                MessageBox.Show("e и phi(n) не взаимно просты! Попробуйте другой e.");
                return;
            }

            // 4. Вычисляем закрытую экспоненту d, используя расширенный алгоритм Евклида:
            //    d = e^(-1) mod phi
            d = ModInverse(e, phi);

            // Отображаем сгенерированные ключи на форме
            labelPublicKey.Text = $"Открытый ключ (e, n): ({e}, {n})";
            labelPrivateKey.Text = $"Закрытый ключ (d, n): ({d}, {n})";
        }

        /// <summary>
        /// шифрование RSA:
        /// Сообщение разбивается на блоки по 2 байта, чтобы числовое значение блока было меньше n.
        /// Каждый блок шифруется по формуле: c = blockValue^e mod n.
        /// </summary>
        /// <param name="message">Исходное сообщение для шифрования</param>
        /// <returns>Список зашифрованных числовых блоков</returns>
        private List<BigInteger> EncryptMessage(string message)
        {
            // Кодируем строку в массив байтов с использованием кодировки Windows-1251
            byte[] messageBytes = Encoding.GetEncoding("windows-1251").GetBytes(message);

            // Список для хранения зашифрованных блоков
            List<BigInteger> encryptedBlocks = new List<BigInteger>();

            // Обрабатываем сообщение по 2 байта, чтобы значение блока точно было меньше n
            // При n = 155357, максимальное значение блока (2 байта) будет 65535, что меньше n
            for (int i = 0; i < messageBytes.Length; i += 2)
            {
                // Формируем число из 1 или 2 байт
                BigInteger blockValue = 0;
                int blockSize = Math.Min(2, messageBytes.Length - i);
                for (int b = 0; b < blockSize; b++)
                {
                    // Сдвигаем байт на 8 * b бит и добавляем его к значению блока
                    blockValue += (BigInteger)messageBytes[i + b] << (8 * b);
                }

                // Шифруем блок по формуле RSA: c = blockValue^e mod n
                BigInteger c = BigInteger.ModPow(blockValue, e, n);
                encryptedBlocks.Add(c);
            }

            return encryptedBlocks;
        }   

        /// <summary>
        /// расшифровка RSA:
        /// Каждый зашифрованный блок расшифровывается по формуле: m = c^d mod n,
        /// после чего полученное число преобразуется обратно в исходные 1 или 2 байта.
        /// </summary>
        /// <param name="encryptedBlocks">Список зашифрованных блоков</param>
        /// <returns>Расшифрованное сообщение</returns>
        private string DecryptMessage(List<BigInteger> encryptedBlocks)
        {
            // Список для хранения расшифрованных байтов
            List<byte> decryptedBytes = new List<byte>();

            foreach (BigInteger c in encryptedBlocks)
            {
                // Выполняем расшифровку: m = c^d mod n
                BigInteger m = BigInteger.ModPow(c, d, n);

                // Преобразуем полученное число обратно в 2 (или 1) байт.
                // Известно, что значение блока меньше 65536.
                byte b0 = (byte)(m & 0xFF);         // Извлекаем младший байт
                byte b1 = (byte)((m >> 8) & 0xFF);    // Извлекаем старший байт
                decryptedBytes.Add(b0);

                // Если второй байт не равен 0, значит в блоке было 2 байта
                if (b1 != 0)
                    decryptedBytes.Add(b1);
            }

            // Преобразуем массив байтов обратно в строку с использованием кодировки Windows-1251
            return Encoding.GetEncoding("windows-1251").GetString(decryptedBytes.ToArray());
        }

        /// <summary>
        /// Вычисление мультипликативной обратной величины d для a по модулю m.
        /// Используется расширенный алгоритм Евклида для нахождения d, удовлетворяющего условию:
        /// d = a^(-1) mod m.
        /// </summary>
        /// <param name="a">Число, для которого ищется обратное (например, e)</param>
        /// <param name="m">Модуль (например, phi(n))</param>
        /// <returns>Мультипликативная обратная величина d</returns>
        private BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            // Сохраняем начальное значение модуля для корректировки результата в конце
            BigInteger m0 = m;
            // Инициализируем коэффициенты расширенного алгоритма Евклида
            (BigInteger x0, BigInteger x1) = (0, 1);

            // Если модуль равен 1, обратное не существует
            if (m == 1)
                return 0;

            // Основной цикл расширенного алгоритма Евклида
            while (a > 1)
            {
                // Вычисляем частное от деления a на m
                BigInteger q = a / m;
                // Обновляем a и m: (a, m) = (m, a mod m)
                (a, m) = (m, a % m);
                // Обновляем коэффициенты: (x0, x1) = (x1 - q * x0, x0)
                (x0, x1) = (x1 - q * x0, x0);
            }

            // Если результат отрицательный, корректируем его добавлением исходного модуля
            if (x1 < 0)
                x1 += m0;

            return x1;
        }
        private void buttonInsertTestData_Click(object sender, EventArgs e)
        {
            textBoxInput.Text = "Все теории стоят одна другой";
        }
    }
}
