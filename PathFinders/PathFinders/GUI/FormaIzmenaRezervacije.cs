using System;
using System.Drawing;
using System.Windows.Forms;

public class FormaIzmenaRezervacije : Form
{
    private Label lbl;
    private NumericUpDown num;
    private Button btnOk, btnCancel;

    public int NoviBrojOsoba { get; private set; }

    public FormaIzmenaRezervacije(int trenutniBroj)
    {
        Text = "Izmena broja osoba";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(360, 160);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        lbl = new Label { Left = 12, Top = 18, AutoSize = true, Text = "Broj osoba:" };
        Controls.Add(lbl);

        num = new NumericUpDown
        {
            Left = 12,
            Top = 42,
            Width = 120,
            Minimum = 1,
            Maximum = 1000,
            Value = Math.Max(1, Math.Min(trenutniBroj, 1000))
        };
        Controls.Add(num);

        btnOk = new Button { Text = "Sačuvaj", Left = 180, Top = 42, Width = 75 };
        btnOk.Click += (s, e) => { NoviBrojOsoba = (int)num.Value; DialogResult = DialogResult.OK; };
        Controls.Add(btnOk);

        btnCancel = new Button { Text = "Otkaži", Left = 260, Top = 42, Width = 75 };
        btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        Controls.Add(btnCancel);
    }
}