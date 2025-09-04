using System;
using System.Drawing;
using System.Windows.Forms;

public class FormaNoviPaket : Form
{
    // Kontrole za unos
    private Label lblNaziv, lblDestinacija, lblCena, lblTrajanje, lblOpis;
    private TextBox txtNaziv, txtDestinacija, txtCena, txtTrajanje, txtOpis;
    private Button btnSacuvaj, btnOdustani;

    // Svojstva koja će čuvati unete podatke
    public string Naziv { get; private set; }
    public string Destinacija { get; private set; }
    public string Cena { get; private set; }
    public string Trajanje { get; private set; }
    public string Opis { get; private set; }

    public FormaNoviPaket()
    {
        this.Text = "Novi Paket";
        this.Size = new Size(400, 380);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Inicijalizacija i postavljanje kontrola
        int y = 20;

        lblNaziv = NapraviLabelu("Naziv:", 20, y);
        txtNaziv = NapraviTextBox(150, y, 200, false);

        lblDestinacija = NapraviLabelu("Destinacija:", 20, y += 40);
        txtDestinacija = NapraviTextBox(150, y, 200, false);

        lblCena = NapraviLabelu("Cena:", 20, y += 40);
        txtCena = NapraviTextBox(150, y, 200, false);

        lblTrajanje = NapraviLabelu("Trajanje:", 20, y += 40);
        txtTrajanje = NapraviTextBox(150, y, 200, false);

        lblOpis = NapraviLabelu("Opis:", 20, y += 40);
        txtOpis = NapraviTextBox(150, y, 200, true);

        btnSacuvaj = NapraviDugme("Sačuvaj", 50, y + 90);
        btnOdustani = NapraviDugme("Odustani", 200, y + 90);

        // Dodavanje kontrola na formu
        this.Controls.AddRange(new Control[] {
            lblNaziv, txtNaziv, lblDestinacija, txtDestinacija, lblCena, txtCena,
            lblTrajanje, txtTrajanje, lblOpis, txtOpis, btnSacuvaj, btnOdustani
        });

        // Event handler za dugmad
        btnSacuvaj.Click += (s, e) => {
            if (string.IsNullOrWhiteSpace(txtNaziv.Text) || string.IsNullOrWhiteSpace(txtDestinacija.Text) || string.IsNullOrWhiteSpace(txtCena.Text))
            {
                MessageBox.Show("Polja Naziv, Destinacija i Cena su obavezna.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Naziv = txtNaziv.Text;
            this.Destinacija = txtDestinacija.Text;
            this.Cena = txtCena.Text;
            this.Trajanje = txtTrajanje.Text;
            this.Opis = txtOpis.Text;

            this.DialogResult = DialogResult.OK;
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

    private TextBox NapraviTextBox(int x, int y, int sirina, bool multiLine)
    {
        return new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(sirina, multiLine ? 70 : 25),
            Multiline = multiLine
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