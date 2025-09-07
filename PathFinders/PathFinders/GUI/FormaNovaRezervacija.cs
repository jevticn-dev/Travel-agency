using System;
using System.Drawing;
using System.Windows.Forms;
using PathFinders.GUI;

public class FormaNovaRezervacija : Form
{
    private Label lblPaket, lblUsluga, lblBrojOsoba;
    private TextBox txtPaket, txtUsluga;
    private Button btnIzborPaketa, btnIzborUsluge, btnUndo, btnRedo;
    private NumericUpDown numBroj;
    private Button btnDodaj, btnOdustani;

    public FormaNovaRezervacija()
    {
        Text = "Dodavanje rezervacije";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(640, 360);
        BackColor = Color.White;

        lblPaket = new Label { Text = "Izaberi paket:", Left = 12, Top = 20, AutoSize = true };
        Controls.Add(lblPaket);

        txtPaket = new TextBox { Left = 12, Top = 44, Width = 440, ReadOnly = true };
        Controls.Add(txtPaket);

        btnIzborPaketa = new Button { Text = "Izbor", Left = 460, Top = 42, Width = 150 };
        btnIzborPaketa.Click += (s, e) =>
        {
            using (var dlg = new FormaIzborPaketa())
            {
                dlg.ShowDialog(this); // ne moraš da koristiš rezultat
            }
        };
        Controls.Add(btnIzborPaketa);

        lblUsluga = new Label { Text = "Izaberi uslugu (opciono):", Left = 12, Top = 92, AutoSize = true };
        Controls.Add(lblUsluga);

        txtUsluga = new TextBox { Left = 12, Top = 116, Width = 440, ReadOnly = true };
        Controls.Add(txtUsluga);

        btnIzborUsluge = new Button { Text = "Izbor", Left = 460, Top = 114, Width = 150 };
        btnIzborUsluge.Click += (s, e) =>
        {
            using (var dlg = new FormaIzborUslugaVise())
            {
                dlg.ShowDialog(this); // ovde se samo otvara forma
            }
        };
        Controls.Add(btnIzborUsluge);


        lblBrojOsoba = new Label { Text = "Broj osoba:", Left = 12, Top = 200, AutoSize = true };
        Controls.Add(lblBrojOsoba);

        numBroj = new NumericUpDown { Left = 12, Top = 224, Width = 120, Minimum = 1, Maximum = 1000, Value = 1 };
        Controls.Add(numBroj);

        btnDodaj = new Button { Text = "Dodaj rezervaciju", Left = 380, Top = 270, Width = 230 };
        Controls.Add(btnDodaj);

        btnOdustani = new Button { Text = "Otkaži", Left = 270, Top = 270, Width = 100 };
        btnOdustani.Click += (s, e) => DialogResult = DialogResult.Cancel;
        Controls.Add(btnOdustani);
    }
}
