using System;
using System.Drawing;
using System.Windows.Forms;

public class FormaNoviKlijent : Form
{
    // Kontrole za unos
    private Label lblIme, lblPrezime, lblBrojPasosa, lblEmail, lblTelefon, lblDatumRodjenja;
    private TextBox txtIme, txtPrezime, txtBrojPasosa, txtEmail, txtTelefon;
    private DateTimePicker dtpDatumRodjenja;
    private Button btnSacuvaj, btnOdustani;

    // Svojstva koja će čuvati unete podatke
    public string Ime { get; private set; }
    public string Prezime { get; private set; }
    public string BrojPasosa { get; private set; }
    public string Email { get; private set; }
    public string Telefon { get; private set; }
    public DateTime DatumRodjenja { get; private set; }

    public FormaNoviKlijent()
    {
        this.Text = "Novi Klijent";
        this.Size = new Size(400, 370);
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

        lblBrojPasosa = NapraviLabelu("Broj pasoša:", 20, y += 40);
        txtBrojPasosa = NapraviTextBox(150, y);

        lblEmail = NapraviLabelu("Email:", 20, y += 40);
        txtEmail = NapraviTextBox(150, y);

        lblTelefon = NapraviLabelu("Telefon:", 20, y += 40);
        txtTelefon = NapraviTextBox(150, y);

        lblDatumRodjenja = NapraviLabelu("Datum rođenja:", 20, y += 40);
        dtpDatumRodjenja = new DateTimePicker
        {
            Location = new Point(150, y),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short
        };

        btnSacuvaj = NapraviDugme("Sačuvaj", 50, y += 60);
        btnOdustani = NapraviDugme("Odustani", 200, y);

        // Dodavanje kontrola na formu
        this.Controls.AddRange(new Control[] {
            lblIme, txtIme, lblPrezime, txtPrezime, lblBrojPasosa, txtBrojPasosa,
            lblEmail, txtEmail, lblTelefon, txtTelefon, lblDatumRodjenja, dtpDatumRodjenja,
            btnSacuvaj, btnOdustani
        });

        // Event handler-i za dugmad
        btnSacuvaj.Click += (s, e) => {
            // Validacija unosa
            if (string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text) || string.IsNullOrWhiteSpace(txtBrojPasosa.Text))
            {
                MessageBox.Show("Molimo popunite sva obavezna polja.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Prenošenje podataka
            this.Ime = txtIme.Text;
            this.Prezime = txtPrezime.Text;
            this.BrojPasosa = txtBrojPasosa.Text;
            this.Email = txtEmail.Text;
            this.Telefon = txtTelefon.Text;
            this.DatumRodjenja = dtpDatumRodjenja.Value;

            this.DialogResult = DialogResult.OK; // Postavi rezultat dijaloga na OK
            this.Close();
        };

        btnOdustani.Click += (s, e) => {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        };
    }

    // Pomoćne metode za kreiranje kontrola
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