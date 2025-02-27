using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Lab2;

// Форма для демонстрации работы алгоритма "Кузнечик" (ГОСТ 34.12-2018)
// Здесь реализована функция вычисления имитовставки (MAC) для заданного сообщения и ключа.
public partial class KuznechikForm : Form
{
    // Таблица подстановок (S-box) из стандарта ГОСТ 34.12-2018.
    // Используется для нелинейного преобразования данных.
    private static readonly byte[] Sbox = new byte[] {
        0xFC, 0xEE, 0xDD, 0x11, 0xCF, 0x6E, 0x31, 0x16, 0xFB, 0xC4, 0xFA, 0xDA, 0x23, 0xC5, 0x04, 0x4D,
        0xE9, 0x77, 0xF0, 0xDB, 0x93, 0x2E, 0x99, 0xBA, 0x17, 0x36, 0xF1, 0xBB, 0x14, 0xCD, 0x5F, 0xC1,
        0xF9, 0x18, 0x65, 0x5A, 0xE2, 0x5C, 0xEF, 0x21, 0x81, 0x1C, 0x3C, 0x42, 0x8B, 0x01, 0x8E, 0x4F,
        0x05, 0x84, 0x02, 0xAE, 0xE3, 0x6A, 0x8F, 0xA0, 0x06, 0x0B, 0xED, 0x98, 0x7F, 0xD4, 0xD3, 0x1F,
        0xEB, 0x34, 0x2C, 0x51, 0xEA, 0xC8, 0x48, 0xAB, 0xF2, 0x2A, 0x68, 0xA2, 0xFD, 0x3A, 0xCE, 0xCC,
        0xB5, 0x70, 0x0E, 0x56, 0x08, 0x0C, 0x76, 0x12, 0xBF, 0x72, 0x13, 0x47, 0x9C, 0xB7, 0x5D, 0x87,
        0x15, 0xA1, 0x96, 0x29, 0x10, 0x7B, 0x9A, 0xC7, 0xF3, 0x91, 0x78, 0x6F, 0x9D, 0x9E, 0xB2, 0xB1,
        0x32, 0x75, 0x19, 0x3D, 0xFF, 0x35, 0x8A, 0x7E, 0x6D, 0x54, 0xC6, 0x80, 0xC3, 0xBD, 0x0D, 0x57,
        0xDF, 0xF5, 0x24, 0xA9, 0x3E, 0xA8, 0x43, 0xC9, 0xD7, 0x79, 0xD6, 0xF6, 0x7C, 0x22, 0xB9, 0x03,
        0xE0, 0x0F, 0xEC, 0xDE, 0x7A, 0x94, 0xB0, 0xBC, 0xDC, 0xE8, 0x28, 0x50, 0x4E, 0x33, 0x0A, 0x4A,
        0xA7, 0x97, 0x60, 0x73, 0x1E, 0x00, 0x62, 0x44, 0x1A, 0xB8, 0x38, 0x82, 0x64, 0x9F, 0x26, 0x41,
        0xAD, 0x45, 0x46, 0x92, 0x27, 0x5E, 0x55, 0x2F, 0x8C, 0xA3, 0xA5, 0x7D, 0x69, 0xD5, 0x95, 0x3B,
        0x07, 0x58, 0xB3, 0x40, 0x86, 0xAC, 0x1D, 0xF7, 0x30, 0x37, 0x6B, 0xE4, 0x88, 0xD9, 0xE7, 0x89,
        0xE1, 0x1B, 0x83, 0x49, 0x4C, 0x3F, 0xF8, 0xFE, 0x8D, 0x53, 0xAA, 0x90, 0xCA, 0xD8, 0x85, 0x61,
        0x20, 0x71, 0x67, 0xA4, 0x2D, 0x2B, 0x09, 0x5B, 0xCB, 0x9B, 0x25, 0xD0, 0xBE, 0xE5, 0x6C, 0x52,
        0x59, 0xA6, 0x74, 0xD2, 0xE6, 0xF4, 0xB4, 0xC0, 0xD1, 0x66, 0xAF, 0xC2, 0x39, 0x4B, 0x63, 0xB6
    };

    // Матрица линейного преобразования L из ГОСТ 34.12-2018.
    // Используется для смешивания байтов блока на этапе линейного преобразования.
    private static readonly byte[] LVector = new byte[] {
        0x94, 0x20, 0x85, 0x10, 0xC2, 0xC0, 0x01, 0xFB,
        0x01, 0x94, 0x20, 0x85, 0x10, 0xC2, 0xC0, 0x01
    };

    // Тестовые данные: пример сообщения и ключа, заданные в условии.
    private const string TestMessage = "Кирпич ни с того ни с сего никому и никогда на голову не свалится.";
    private const string TestKey = "4079834365408257140643820486045333485254617662287735457960634880";

    // Конструктор формы, инициализирующий компоненты.
    public KuznechikForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Обработчик нажатия кнопки "Вставить тестовые данные".
    /// Заполняет поля ввода тестовыми сообщением и ключом.
    /// </summary>
    private void buttonInsertTestData_Click(object sender, EventArgs e)
    {
        textBoxMessage.Text = TestMessage;
        textBoxKey.Text = TestKey;
    }

    /// <summary>
    /// Обработчик нажатия кнопки "Зашифровать".
    /// Выполняет вычисление имитовставки для введённого сообщения с использованием заданного ключа.
    /// </summary>
    private void buttonEncrypt_Click(object sender, EventArgs e)
    {
        // Получаем исходное сообщение и ключ из текстовых полей
        string inputMessage = textBoxMessage.Text;
        string keyString = textBoxKey.Text;

        try
        {
            // Преобразуем строковое представление ключа в массив байт.
            byte[] key = ConvertKeyStringToByteArray(keyString);

            // Преобразуем сообщение в массив байт с использованием кодировки Windows-1251.
            // Эта кодировка часто используется в российских криптографических приложениях.
            byte[] messageBytes = Encoding.GetEncoding("windows-1251").GetBytes(inputMessage);

            // Вычисляем имитовставку для сообщения с использованием ключа
            byte[] mac = ComputeKuznechikImitovstavka(messageBytes, key);

            // Преобразуем полученный MAC в шестнадцатеричное представление для вывода
            string macHex = BitConverter.ToString(mac).Replace("-", "");
            textBoxOutput.Text = macHex;
        }
        catch (Exception ex)
        {
            // В случае ошибки выводим сообщение об ошибке
            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Вычисляет имитовставку (MAC) с использованием алгоритма "Кузнечик".
    /// Алгоритм разбивает сообщение на блоки по 16 байт, затем для каждого блока выполняется:
    /// 1. Побитовое XOR-сложение с текущим значением MAC.
    /// 2. Шифрование полученного результата с помощью блока шифрования Kuznechik.
    /// Итоговое значение MAC возвращается как результат.
    /// </summary>
    private byte[] ComputeKuznechikImitovstavka(byte[] message, byte[] key)
    {
        const int blockSize = 16; // Размер блока составляет 128 бит (16 байт)
        byte[] mac = new byte[blockSize]; // Начальное значение MAC (инициализировано нулями)

        // Паддинг (дополнение) сообщения до кратного размера блока, если необходимо
        byte[] paddedMessage = PadMessage(message, blockSize);

        // Обрабатываем сообщение блок за блоком
        for (int i = 0; i < paddedMessage.Length; i += blockSize)
        {
            byte[] block = new byte[blockSize];
            Array.Copy(paddedMessage, i, block, 0, blockSize);

            // Выполняем XOR между текущим MAC и блоком сообщения
            for (int j = 0; j < blockSize; j++)
            {
                mac[j] ^= block[j];
            }

            // Шифруем полученный результат с использованием алгоритма Kuznechik
            mac = KuznechikEncryptBlock(mac, key);
        }

        return mac;
    }

    /// <summary>
    /// Дополняет (паддинг) сообщение до размера, кратного размеру блока.
    /// Если длина сообщения уже кратна размеру блока, возвращается исходное сообщение.
    /// </summary>
    private static byte[] PadMessage(byte[] message, int blockSize)
    {
        int remainder = message.Length % blockSize;
        if (remainder == 0)
        {
            return message;
        }
        int paddingRequired = blockSize - remainder;
        byte[] padded = new byte[message.Length + paddingRequired];
        Array.Copy(message, padded, message.Length);
        return padded;
    }

    /// <summary>
    /// Преобразует строковое представление ключа в массив байт.
    /// Ключ сначала преобразуется в BigInteger, затем в массив байт.
    /// Если длина полученного массива меньше 32 байт, он дополняется нулями, а если больше – усекается до 32 байт.
    /// </summary>
    private static byte[] ConvertKeyStringToByteArray(string keyString)
    {
        // Проверка на null или пустую строку
        ArgumentException.ThrowIfNullOrEmpty(keyString);

        try
        {
            // Преобразуем строку в число BigInteger
            BigInteger bigKey = BigInteger.Parse(keyString);
            byte[] keyBytes = bigKey.ToByteArray();

            // Обеспечиваем длину ключа ровно 32 байта (256 бит)
            if (keyBytes.Length < 32)
            {
                byte[] padded = new byte[32];
                Array.Copy(keyBytes, padded, keyBytes.Length);
                keyBytes = padded;
            }
            else if (keyBytes.Length > 32)
            {
                keyBytes = keyBytes.Take(32).ToArray();
            }
            return keyBytes;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Ошибка преобразования ключа: {ex.Message}", nameof(keyString), ex);
        }
    }

    /// <summary>
    /// Шифрует один блок данных размером 16 байт с использованием алгоритма "Кузнечик".
    /// В качестве ключа используется 32-байтовый массив.
    /// Процесс включает нелинейное преобразование (S-box) и линейное преобразование (L).
    /// </summary>
    private byte[] KuznechikEncryptBlock(byte[] block, byte[] key)
    {
        // Проверка входных данных на null и корректную длину
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(key);

        if (block.Length != 16)
            throw new ArgumentException("Размер блока должен быть 16 байт (128 бит)", nameof(block));

        if (key.Length != 32)
            throw new ArgumentException("Размер ключа должен быть 32 байта (256 бит)", nameof(key));

        // Копируем исходный блок в рабочий массив state
        byte[] state = new byte[16];
        Array.Copy(block, state, 16);

        // Применяем нелинейное преобразование S-box к каждому байту блока
        for (int i = 0; i < 16; i++)
        {
            state[i] = Sbox[state[i]];
        }

        // Применяем линейное преобразование L к блоку.
        // Выполняется 16 итераций, на каждой из которых вычисляется новый байт x через умножение в поле Галуа GF(2^8)
        for (int j = 0; j < 16; j++)
        {
            byte x = 0;
            for (int i = 0; i < 16; i++)
            {
                // Инициализируем temp текущим байтом state[i]
                byte temp = state[i];
                // Для каждого бита байта выполняем умножение в поле GF(2^8)
                for (int k = 0; k < 8; k++)
                {
                    // Если соответствующий бит в LVector установлен, выполняем XOR с temp
                    if ((LVector[i] & (1 << (7 - k))) != 0)
                    {
                        x ^= temp;
                    }
                    // Определяем перенос (carry) старшего бита
                    byte carry = (byte)(temp & 0x80);
                    // Сдвигаем temp влево на 1 бит
                    temp <<= 1;
                    // Если был перенос, выполняем XOR с полиномом (0xC3 соответствует x^8 + x^7 + x^6 + x + 1)
                    if (carry != 0)
                    {
                        temp ^= 0xC3;
                    }
                }
            }
            // Выполняем циклический сдвиг: копируем state, сдвигая байты влево на 1 позицию, и устанавливаем последний байт равным x
            byte[] newState = new byte[16];
            Array.Copy(state, 1, newState, 0, 15);
            newState[15] = x;
            state = newState;
        }

        // Возвращаем преобразованный блок как результат шифрования
        return state;
    }
}
