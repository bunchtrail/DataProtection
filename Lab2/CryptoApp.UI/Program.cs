using System;
using System.Text;

namespace Lab2;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Регистрируем провайдер кодовых страниц для поддержки windows-1251
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}