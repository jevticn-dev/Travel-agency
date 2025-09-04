using System;
using System.Drawing;
using System.Windows.Forms;

public class FormaNovaRezervacija : Form
{
    // Kontrole za unos
    private Label lblIme, lblPrezime, lblPaket, lblDatum, lblStatus;
    private TextBox txtIme, txtPrezime;
    private ComboBox cmbPaket, cmbStatus;
    private DateTimePicker dtpDatum;
    private Button btnSacuvaj, btnOdustani;

    // Svojstva koja će čuvati unete podatke
    public string Ime { get; private set; }
    public string Prezime { get; private set; }
    public string Paket { get; private set; }
    public DateTime Datum { get; private set; }
    public string Status { get; private set; }

    public FormaNovaRezervacija()
    {
        this.Text = "Nova Rezervacija";
        this.Size = new Size(400, 350);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Inicijalizacija i postavljanje kontrola
        int y = 20;

        lblIme = NapraviLabelu("Ime:", 20, y);
        txtIme = NapraviTextBox(150, y);

        lblPrezime = NapraviLabelu("Prezime:", 20, y += 40);
        txtPrezime = NapraviTextBox(150, y);

        lblPaket = NapraviLabelu("Paket:", 20, y += 40);
        cmbPaket = new ComboBox { Location = new Point(150, y), Size = new Size(200, 25) };
        cmbPaket.Items.AddRange(new string[] { "Letovanje - Grčka", "Zimovanje - Kopaonik", "Vikend u Pragu" }); // Primeri paketa

        lblDatum = NapraviLabelu("Datum:", 20, y += 40);
        dtpDatum = new DateTimePicker
        {
            Location = new Point(150, y),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short
        };

        lblStatus = NapraviLabelu("Status:", 20, y += 40);
        cmbStatus = new ComboBox { Location = new Point(150, y), Size = new Size(200, 25) };
        cmbStatus.Items.AddRange(new string[] { "Potvrđeno", "Na čekanju", "Otkazano" });
        cmbStatus.SelectedIndex = 0;

        btnSacuvaj = NapraviDugme("Sačuvaj", 50, y + 60);
        btnOdustani = NapraviDugme("Odustani", 200, y + 60);

        // Dodavanje kontrola na formu
        this.Controls.AddRange(new Control[] {
            lblIme, txtIme, lblPrezime, txtPrezime, lblPaket, cmbPaket,
            lblDatum, dtpDatum, lblStatus, cmbStatus, btnSacuvaj, btnOdustani
        });

        // Event handler za dugmad
        btnSacuvaj.Click += (s, e) => {
            if (string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text) || cmbPaket.SelectedItem == null)
            {
                MessageBox.Show("Molimo popunite sva obavezna polja.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Ime = txtIme.Text;
            this.Prezime = txtPrezime.Text;
            this.Paket = cmbPaket.SelectedItem.ToString();
            this.Datum = dtpDatum.Value;
            this.Status = cmbStatus.SelectedItem.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        };

        btnOdustani.Click += (s, e) => {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        };
    }

    // Pomoćne metode
    private Label NapraviLabelu(string tekst, int x, int y)
    {
        return new Label
        {
            Text = tekst,
            Location = new Point(x, y),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            AutoSize = true
        };
    }

    private TextBox NapraviTextBox(int x, int y)
    {
        return new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(200, 25)
        };
    }

    private Button NapraviDugme(string tekst, int x, int y)
    {
        return new Button
        {
            Text = tekst,
            Location = new Point(x, y),
            Size = new Size(120, 35),
            BackColor = Color.FromArgb(41, 128, 185),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
    }
}