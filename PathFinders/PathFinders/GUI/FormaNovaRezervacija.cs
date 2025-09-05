using System;
using System.Drawing;
using System.IO.Packaging;
using System.Windows.Forms;
using PathFinders.Models;
using PathFinders.GUI;

public class FormaNovaRezervacija : Form
{
    private Label lblPaket;
    private TextBox txtPaket;
    private Button btnIzbor;

    private Label lblBrojOsoba;
    private NumericUpDown numBroj;

    private Button btnDodaj;
    private Button btnOdustani;

    // NEMA Package; koristimo SelectedPackage iz FormaIzborPaketa:
    private FormaIzborPaketa.SelectedPackage _izabrani;

    public class RezervacijaResult
    {
        public string PaketPrikaz { get; set; }
        public string Destinacija { get; set; }
        public int BrojOsoba { get; set; }
        public DateTime DatumRezervacije { get; set; }
    }

    public RezervacijaResult Rezultat { get; private set; }

    public FormaNovaRezervacija()
    {
        Text = "Dodavanje rezervacije";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(600, 240);
        BackColor = Color.White;

        lblPaket = new Label { Text = "Izaberi paket:", Left = 12, Top = 20, AutoSize = true };
        Controls.Add(lblPaket);

        txtPaket = new TextBox
        {
            Left = 12,
            Top = 44,
            Width = 420,
            ReadOnly = true
            // Ako PlaceholderText baca grešku na tvom .NET-u, samo izostavi:
            // PlaceholderText = "Vaš paket"
        };
        Controls.Add(txtPaket);

        btnIzbor = new Button { Text = "Izbor", Left = 440, Top = 42, Width = 120 };
        btnIzbor.Click += (s, e) => OnIzborPaketa();
        Controls.Add(btnIzbor);

        lblBrojOsoba = new Label { Text = "Broj osoba:", Left = 12, Top = 94, AutoSize = true };
        Controls.Add(lblBrojOsoba);

        numBroj = new NumericUpDown { Left = 12, Top = 118, Width = 120, Minimum = 1, Maximum = 1000, Value = 1 };
        Controls.Add(numBroj);

        btnDodaj = new Button { Text = "Dodaj rezervaciju", Left = 330, Top = 160, Width = 230 };
        btnDodaj.Click += (s, e) => OnDodajRezervaciju();
        Controls.Add(btnDodaj);

        btnOdustani = new Button { Text = "Otkaži", Left = 220, Top = 160, Width = 100 };
        btnOdustani.Click += (s, e) => DialogResult = DialogResult.Cancel;
        Controls.Add(btnOdustani);
    }

    private void OnIzborPaketa()
    {
        using (var dlg = new FormaIzborPaketa())
        {
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _izabrani = dlg.Izabrani;
                txtPaket.Text = _izabrani?.Display ?? "";
            }
        }
    }

    private void OnDodajRezervaciju()
    {
        if (_izabrani == null)
        {
            MessageBox.Show("Najpre izaberi paket.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Rezultat = new RezervacijaResult
        {
            PaketPrikaz = _izabrani.Display,
            Destinacija = _izabrani.Destinacija,
            BrojOsoba = (int)numBroj.Value,
            DatumRezervacije = DateTime.Today
        };

        DialogResult = DialogResult.OK;
    }

}