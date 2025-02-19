using System;
using System.Text;
using System.Windows.Forms;

namespace CryptoApp.UI;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void buttonDecode_Click(object sender, EventArgs e)
    {
        // Get input data
        string encodedText = textBoxEncoded.Text;
        string matrixText = textBoxMatrix.Text;

        // Split matrix string by commas and trim whitespace
        string[] rows = matrixText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())
                                    .ToArray();

        // Check that matrix has 6 rows
        if (rows.Length != 6)
        {
            MessageBox.Show("Матрица должна состоять из 6 строк.");
            return;
        }

        // Create 2D array for Polybius Square
        char[,] polybiusSquare = new char[6, 6];
        for (int i = 0; i < 6; i++)
        {
            if (rows[i].Length != 6)
            {
                MessageBox.Show($"Строка {i + 1} матрицы должна содержать 6 символов.");
                return;
            }

            for (int j = 0; j < 6; j++)
            {
                polybiusSquare[i, j] = rows[i][j];
            }
        }

        // Split input string into tokens by spaces
        string[] tokens = encodedText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        StringBuilder decodedMessage = new StringBuilder();
        foreach (string token in tokens)
        {
            // Expect each code to be 2 digits
            if (token.Length != 2)
            {
                continue;
            }

            // Convert characters to digits
            if (!int.TryParse(token[0].ToString(), out int rowDigit) ||
                !int.TryParse(token[1].ToString(), out int colDigit))
            {
                MessageBox.Show($"Некорректный код: {token}");
                return;
            }

            // Check that row and column numbers are in range 1..6
            if (rowDigit < 1 || rowDigit > 6 || colDigit < 1 || colDigit > 6)
            {
                MessageBox.Show($"Некорректные координаты: {token}");
                return;
            }

            // Convert coordinates to array indices (0-5)
            int rowIndex = rowDigit - 1;
            int colIndex = colDigit - 1;

            // Add corresponding character to result
            decodedMessage.Append(polybiusSquare[rowIndex, colIndex]);
        }

        // Display decryption result
        textBoxDecoded.Text = decodedMessage.ToString();
    }
}
