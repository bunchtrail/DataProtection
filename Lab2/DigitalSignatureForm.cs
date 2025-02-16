using System;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
    public partial class DigitalSignatureForm : Form
    {
        // Задаем простые числа p и q (как в задании №3)
        private const int P = 337;
        private const int Q = 461;
        
        // Пара RSA-ключей: открытый (e, n) и закрытый (d, n)
        private BigInteger e;
        private BigInteger d;
        private BigInteger n;
        
        public DigitalSignatureForm()
        {
            InitializeComponent();
            GenerateKeys();
        }
        
        /// <summary>
        /// Генерация RSA-ключей на основе p и q.
        /// </summary>
        private void GenerateKeys()
        {
            // Вычисляем модуль n и функцию Эйлера phi(n)
            n = P * Q;                      // n = 337 * 461 = 155357
            BigInteger phi = (P - 1) * (Q - 1);  // phi = 336 * 460 = 154560
            
            // Выбираем стандартное значение e
            e = 65537;
            if (BigInteger.GreatestCommonDivisor(e, phi) != 1)
            {
                MessageBox.Show("e и phi(n) не взаимно просты. Попробуйте другое e.");
                return;
            }
            
            // Вычисляем d – мультипликативную обратную величину e по модулю phi
            d = ModInverse(e, phi);
            
            // Отобразим ключи
            labelPublicKey.Text = $"Открытый ключ (e, n): ({e}, {n})";
            labelPrivateKey.Text = $"Закрытый ключ (d, n): ({d}, {n})";
        }
        
        /// <summary>
        /// Поиск мультипликативной обратной величины d для a по модулю m.
        /// d = a^(-1) mod m
        /// (Расширенный алгоритм Евклида)
        /// </summary>
        private BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger x0 = 0, x1 = 1;
            
            if (m == 1)
                return 0;
            
            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;
                m = a % m;
                a = t;
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }
            
            if (x1 < 0)
                x1 += m0;
            
            return x1;
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки для вычисления ЭЦП.
        /// </summary>
        private void buttonSign_Click(object sender, EventArgs e)
        {
            // Заданная строка для подписи
            string message = "Красота спасет мир.";
            
            // Вычисляем хеш-образ строки с помощью алгоритма Adler-32
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            uint hash = ComputeAdler32(messageBytes);
            
            // Поскольку наш RSA-модуль n мал, приводим хеш к диапазону n
            BigInteger hashValue = new BigInteger(hash);
            hashValue = hashValue % n;
            
            // Вычисляем ЭЦП: подпись s = (hashValue)^d mod n
            BigInteger signature = BigInteger.ModPow(hashValue, d, n);
            
            // Отображаем подпись
            textBoxSignature.Text = signature.ToString();
        }
        
        /// <summary>
        /// Реализация алгоритма Adler-32.
        /// </summary>
        private uint ComputeAdler32(byte[] data)
        {
            const uint MOD_ADLER = 65521;
            uint a = 1, b = 0;
            
            foreach (byte bt in data)
            {
                a = (a + bt) % MOD_ADLER;
                b = (b + a) % MOD_ADLER;
            }
            
            return (b << 16) | a;
        }
    }
} 