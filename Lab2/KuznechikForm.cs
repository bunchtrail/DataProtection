using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Lab2;

public partial class KuznechikForm : Form
{
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

        // TODO: Реализация алгоритма Кузнечик
        throw new NotImplementedException("Требуется реализация метода KuznechikEncryptBlock");
    }
} 