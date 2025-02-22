using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Lab2;

public partial class KuznechikForm : Form
{
    // Таблица подстановок (S-box) из ГОСТ 34.12-2018
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

    // Матрица линейного преобразования L из ГОСТ 34.12-2018
    private static readonly byte[] LVector = new byte[] {
        0x94, 0x20, 0x85, 0x10, 0xC2, 0xC0, 0x01, 0xFB,
        0x01, 0x94, 0x20, 0x85, 0x10, 0xC2, 0xC0, 0x01
    };

    // Тестовые данные из задания
    private const string TestMessage = "Кирпич ни с того ни с сего никому и никогда на голову не свалится.";
    private const string TestKey = "4079834365408257140643820486045333485254617662287735457960634880";

    public KuznechikForm()
    {
        InitializeComponent();
    }

    private void buttonInsertTestData_Click(object sender, EventArgs e)
    {
        textBoxMessage.Text = TestMessage;
        textBoxKey.Text = TestKey;
    }

    private void buttonEncrypt_Click(object sender, EventArgs e)
    {
        string inputMessage = textBoxMessage.Text;
        string keyString = textBoxKey.Text;

        try
        {
            // Преобразуем ключ из строки в массив байт
            byte[] key = ConvertKeyStringToByteArray(keyString);

            // Преобразуем текст в массив байт с использованием Windows-1251
            // Используем эту кодировку для совместимости с ГОСТ-приложениями,
            // где она часто используется для русского текста
            byte[] messageBytes = Encoding.GetEncoding("windows-1251").GetBytes(inputMessage);

            // Вычисляем имитовставку
            byte[] mac = ComputeKuznechikImitovstavka(messageBytes, key);

            // Преобразуем результат в шестнадцатеричную строку
            string macHex = BitConverter.ToString(mac).Replace("-", "");
            textBoxOutput.Text = macHex;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private byte[] ComputeKuznechikImitovstavka(byte[] message, byte[] key)
    {
        const int blockSize = 16; // Размер блока 128 бит
        byte[] mac = new byte[blockSize]; // Начальное значение MAC

        // Паддинг сообщения
        byte[] paddedMessage = PadMessage(message, blockSize);

        // Проходим по каждому блоку
        for (int i = 0; i < paddedMessage.Length; i += blockSize)
        {
            byte[] block = new byte[blockSize];
            Array.Copy(paddedMessage, i, block, 0, blockSize);

            // XOR текущего MAC с блоком сообщения
            for (int j = 0; j < blockSize; j++)
            {
                mac[j] ^= block[j];
            }

            // Шифруем полученный блок
            mac = KuznechikEncryptBlock(mac, key);
        }

        return mac;
    }

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

    private static byte[] ConvertKeyStringToByteArray(string keyString)
    {
        ArgumentException.ThrowIfNullOrEmpty(keyString);

        try
        {
            BigInteger bigKey = BigInteger.Parse(keyString);
            byte[] keyBytes = bigKey.ToByteArray();

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

    private byte[] KuznechikEncryptBlock(byte[] block, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(key);

        if (block.Length != 16)
            throw new ArgumentException("Размер блока должен быть 16 байт (128 бит)", nameof(block));
        
        if (key.Length != 32)
            throw new ArgumentException("Размер ключа должен быть 32 байта (256 бит)", nameof(key));

        // Копируем входной блок
        byte[] state = new byte[16];
        Array.Copy(block, state, 16);

        // Применяем нелинейное преобразование (S-box)
        for (int i = 0; i < 16; i++)
        {
            state[i] = Sbox[state[i]];
        }

        // Применяем линейное преобразование L
        for (int j = 0; j < 16; j++)
        {
            byte x = 0;
            for (int i = 0; i < 16; i++)
            {
                // Умножение в поле Галуа GF(2^8)
                byte temp = state[i];
                for (int k = 0; k < 8; k++)
                {
                    if ((LVector[i] & (1 << (7 - k))) != 0)
                    {
                        x ^= temp;
                    }
                    byte carry = (byte)(temp & 0x80);
                    temp <<= 1;
                    if (carry != 0)
                    {
                        temp ^= 0xC3; // x^8 + x^7 + x^6 + x + 1
                    }
                }
            }
            // Сохраняем результат
            byte[] newState = new byte[16];
            Array.Copy(state, 1, newState, 0, 15);
            newState[15] = x;
            state = newState;
        }

        return state;
    }
} 