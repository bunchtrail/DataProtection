namespace CryptoApp.UI;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private TextBox textBoxEncoded;
    private TextBox textBoxDecoded;
    private TextBox textBoxMatrix;
    private Button buttonDecode;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        textBoxEncoded = new TextBox();
        textBoxDecoded = new TextBox();
        textBoxMatrix = new TextBox();
        buttonDecode = new Button();

        // 
        // textBoxEncoded
        // 
        textBoxEncoded.Location = new System.Drawing.Point(12, 12);
        textBoxEncoded.Multiline = true;
        textBoxEncoded.Size = new System.Drawing.Size(300, 100);

        // 
        // textBoxMatrix
        // 
        textBoxMatrix.Location = new System.Drawing.Point(12, 118);
        textBoxMatrix.Size = new System.Drawing.Size(300, 23);

        // 
        // buttonDecode
        // 
        buttonDecode.Location = new System.Drawing.Point(12, 147);
        buttonDecode.Size = new System.Drawing.Size(75, 23);
        buttonDecode.Text = "Decode";
        buttonDecode.Click += buttonDecode_Click;

        // 
        // textBoxDecoded
        // 
        textBoxDecoded.Location = new System.Drawing.Point(12, 176);
        textBoxDecoded.Multiline = true;
        textBoxDecoded.Size = new System.Drawing.Size(300, 100);

        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(324, 291);
        Controls.Add(textBoxEncoded);
        Controls.Add(textBoxMatrix);
        Controls.Add(buttonDecode);
        Controls.Add(textBoxDecoded);
        Name = "Form1";
        Text = "Polybius Square Decoder";
    }

    #endregion
}
