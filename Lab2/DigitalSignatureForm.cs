using System;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
    // Форма для демонстрации цифровой подписи.
    // Здесь реализован механизм генерации RSA-ключей и создания ЭЦП (электронной цифровой подписи)
    public partial class DigitalSignatureForm : Form
    {
        // Задаем простые числа p и q (используемые для формирования RSA-модуля)
        private const int P = 337;
        private const int Q = 461;

        // Пара RSA-ключей:
        // Открытый ключ представлен парой (e, n)
        // Закрытый ключ представлен парой (d, n)
        private BigInteger e;
        private BigInteger d;
        private BigInteger n;

        // Конструктор формы.
        // При инициализации формы сразу генерируются RSA-ключи.
        public DigitalSignatureForm()
        {
            InitializeComponent();
            GenerateKeys();
        }

        /// <summary>
        /// Генерация RSA-ключей на основе простых чисел p и q.
        /// Шаги:
        /// 1. Вычисление модуля n = p * q и функции Эйлера phi(n) = (p - 1) * (q - 1).
        /// 2. Выбор стандартного значения e (обычно 65537).
        /// 3. Проверка взаимной простоты e и phi(n).
        /// 4. Вычисление закрытой экспоненты d как мультипликативной обратной величины к e по модулю phi(n).
        /// </summary>
        private void GenerateKeys()
        {
            // Вычисляем модуль n и функцию Эйлера phi(n)
            n = P * Q;                      // n = 337 * 461 = 155357
            BigInteger phi = (P - 1) * (Q - 1);  // phi = 336 * 460 = 154560

            // Выбираем стандартное значение открытой экспоненты e
            e = 65537;
            // Проверяем, что e и phi(n) взаимно просты (их НОД должен быть равен 1)
            if (BigInteger.GreatestCommonDivisor(e, phi) != 1)
            {
                MessageBox.Show("e и phi(n) не взаимно просты. Попробуйте другое e.");
                return;
            }

            // Вычисляем d – мультипликативную обратную величину e по модулю phi(n)
            d = ModInverse(e, phi);

            // Отображаем сгенерированные ключи на форме
            labelPublicKey.Text = $"Открытый ключ (e, n): ({e}, {n})";
            labelPrivateKey.Text = $"Закрытый ключ (d, n): ({d}, {n})";
        }

        /// <summary>
        /// Поиск мультипликативной обратной величины d для a по модулю m.
        /// Используется расширенный алгоритм Евклида.
        /// То есть требуется найти такое число d, что (a * d) mod m = 1.
        /// </summary>
        private BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            // Сохраняем начальное значение модуля для корректировки отрицательного результата
            BigInteger m0 = m;
            BigInteger x0 = 0, x1 = 1;

            // Если модуль равен 1, обратное не существует (возвращаем 0)
            if (m == 1)
                return 0;

            // Основной цикл расширенного алгоритма Евклида
            while (a > 1)
            {
                // Вычисляем частное от деления a на m
                BigInteger q = a / m;
                // Сохраняем значение m во временную переменную
                BigInteger t = m;
                // Обновляем m: новое значение m равно остатку от деления a на m
                m = a % m;
                // Переопределяем a
                a = t;
                // Сохраняем x0 во временную переменную и обновляем коэффициенты
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }

            // Если найденный коэффициент отрицательный, корректируем его, добавив исходный модуль
            if (x1 < 0)
                x1 += m0;

            return x1;
        }

        /// <summary>
        /// Обработчик нажатия кнопки для вычисления ЭЦП.
        /// Алгоритм выполняет следующие шаги:
        /// 1. Задает сообщение для подписи.
        /// 2. Вычисляет хеш-образ сообщения с помощью алгоритма Adler-32.
        /// 3. Приводит хеш к диапазону модуля n (так как n достаточно мал).
        /// 4. Вычисляет подпись: s = (hashValue)^d mod n.
        /// 5. Отображает полученную цифровую подпись.
        /// </summary>
        private void buttonSign_Click(object sender, EventArgs e)
        {
            // Заданная строка, для которой создается подпись
            string message = "Красота спасет мир.";

            // Преобразуем сообщение в массив байтов с использованием кодировки UTF-8
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            // Вычисляем хеш сообщения с помощью алгоритма Adler-32
            uint hash = ComputeAdler32(messageBytes);

            // Приводим хеш к диапазону модуля n, поскольку n невелик
            BigInteger hashValue = new BigInteger(hash);
            hashValue = hashValue % n;

            // Вычисляем ЭЦП: подпись s = (hashValue)^d mod n,
            // где d – закрытая экспонента RSA
            BigInteger signature = BigInteger.ModPow(hashValue, d, n);

            // Выводим полученную подпись в текстовое поле
            textBoxSignature.Text = signature.ToString();
        }

        /// <summary>
        /// Реализация алгоритма Adler-32 для вычисления контрольной суммы.
        /// Алгоритм использует две переменные: a и b.
        /// Начальные значения: a = 1, b = 0.
        /// Для каждого байта данных:
        ///   a = (a + байт) mod MOD_ADLER;
        ///   b = (b + a) mod MOD_ADLER;
        /// Итоговый хеш формируется объединением b (старшие 16 бит) и a (младшие 16 бит).
        /// </summary>
        private uint ComputeAdler32(byte[] data)
        {
            const uint MOD_ADLER = 65521;
            uint a = 1, b = 0;

            // Обрабатываем каждый байт данных
            foreach (byte bt in data)
            {
                a = (a + bt) % MOD_ADLER;
                b = (b + a) % MOD_ADLER;
            }

            // Формируем итоговое 32-битное значение: B занимает старшие 16 бит, A – младшие 16 бит
            return (b << 16) | a;
        }
    }
}
