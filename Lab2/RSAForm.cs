using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
    public partial class RSAForm : Form
    {
        // Параметры, заданные в условии
        private const int P = 337;
        private const int Q = 461;

        // Открытый ключ (e, n) и закрытый (d, n)
        private BigInteger e;
        private BigInteger d;
        private BigInteger n;

        public RSAForm()
        {
            InitializeComponent();

            // Генерируем ключи при загрузке формы
            GenerateKeys();
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            string message = textBoxInput.Text;

            // Шифруем
            List<BigInteger> encryptedBlocks = EncryptMessage(message);

            // Преобразуем список блоков в строку для вывода (например, через пробел)
            string cipherText = string.Join(" ", encryptedBlocks);

            textBoxEncrypted.Text = cipherText;
        }

        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                // Разбираем строку с зашифрованными блоками
                List<BigInteger> encryptedBlocks = textBoxEncrypted.Text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => BigInteger.Parse(s))
                    .ToList();

                // Расшифровываем
                string decryptedMessage = DecryptMessage(encryptedBlocks);

                textBoxDecrypted.Text = decryptedMessage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расшифровании: {ex.Message}");
            }
        }

        /// <summary>
        /// Генерация открытого и закрытого ключей RSA
        /// </summary>
        private void GenerateKeys()
        {
            // 1. Вычисляем n и phi(n)
            n = P * Q;                      // 337 * 461 = 155357
            BigInteger phi = (P - 1) * (Q - 1);  // 336 * 460 = 154560

            // 2. Выбираем e
            // Чаще всего используют 65537
            e = 65537;

            // 3. Проверяем, что gcd(e, phi) = 1
            if (BigInteger.GreatestCommonDivisor(e, phi) != 1)
            {
                MessageBox.Show("e и phi(n) не взаимно просты! Попробуйте другой e.");
                return;
            }

            // 4. Находим d = e^(-1) mod phi
            d = ModInverse(e, phi);

            // Отобразим ключи
            labelPublicKey.Text = $"Открытый ключ (e, n): ({e}, {n})";
            labelPrivateKey.Text = $"Закрытый ключ (d, n): ({d}, {n})";
        }

        /// <summary>
        /// Наивное шифрование: разбиваем сообщение по 2 байта,
        /// каждый блок возводим в степень e по модулю n.
        /// </summary>
        private List<BigInteger> EncryptMessage(string message)
        {
            // Кодируем строку в байты (windows-1251, чтобы корректно обработать русские символы)
            byte[] messageBytes = Encoding.GetEncoding("windows-1251").GetBytes(message);

            // Список для зашифрованных блоков
            List<BigInteger> encryptedBlocks = new List<BigInteger>();

            // Берём по 2 байта (чтобы значение блока точно < n)
            // При n=155357 блок до 2^17 (131072), т.е. 2 байта - макс 65535, что меньше 155357
            for (int i = 0; i < messageBytes.Length; i += 2)
            {
                // Формируем число из 1 или 2 байт
                BigInteger blockValue = 0;
                int blockSize = Math.Min(2, messageBytes.Length - i);
                for (int b = 0; b < blockSize; b++)
                {
                    // Смещаем на 8*b бит и прибавляем байт
                    blockValue += (BigInteger)messageBytes[i + b] << (8 * b);
                }

                // Шифруем: c = blockValue^e mod n
                BigInteger c = BigInteger.ModPow(blockValue, e, n);
                encryptedBlocks.Add(c);
            }

            return encryptedBlocks;
        }

        /// <summary>
        /// Наивная расшифровка:
        /// c^d mod n => исходный блок (2 байта).
        /// </summary>
        private string DecryptMessage(List<BigInteger> encryptedBlocks)
        {
            List<byte> decryptedBytes = new List<byte>();

            foreach (BigInteger c in encryptedBlocks)
            {
                // m = c^d mod n
                BigInteger m = BigInteger.ModPow(c, d, n);

                // Преобразуем обратно в 2 (или 1) байта
                // Мы знаем, что блок < 65536
                byte b0 = (byte)(m & 0xFF);         // младший байт
                byte b1 = (byte)((m >> 8) & 0xFF);  // старший байт
                decryptedBytes.Add(b0);

                // Если второй байт != 0, значит у нас было 2 байта
                if (b1 != 0)
                    decryptedBytes.Add(b1);
            }

            // Преобразуем байты обратно в строку
            return Encoding.GetEncoding("windows-1251").GetString(decryptedBytes.ToArray());
        }

        /// <summary>
        /// Поиск мультипликативной обратной величины d для e по модулю m (Extended Euclidean Algorithm)
        /// d = e^(-1) mod m
        /// </summary>
        private BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            // Расширенный алгоритм Евклида
            BigInteger m0 = m;
            (BigInteger x0, BigInteger x1) = (0, 1);

            if (m == 1)
                return 0;

            while (a > 1)
            {
                BigInteger q = a / m;
                (a, m) = (m, a % m);
                (x0, x1) = (x1 - q * x0, x0);
            }

            if (x1 < 0)
                x1 += m0;

            return x1;
        }
    }
} 