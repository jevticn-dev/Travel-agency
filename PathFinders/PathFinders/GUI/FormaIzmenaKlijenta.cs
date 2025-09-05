using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathFinders.GUI
{
    public partial class FormaIzmenaKlijenta : Form
    {
        private TextBox txtIme, txtPrezime, txtBrojPasosa, txtEmail, txtTelefon;
        private DateTimePicker dtpDatum;
        private Button btnSacuvaj, btnOdustani;

        // Svojstva za povratak vrednosti
        public string Ime { get; private set; }
        public string Prezime { get; private set; }
        public string BrojPasosa { get; private set; }
        public string Email { get; private set; }
        public string Telefon { get; private set; }
        public DateTime DatumRodjenja { get; private set; }

        public FormaIzmenaKlijenta(
            string ime,
            string prezime,
            string brojPasosa,
            string email,
            string telefon,
            DateTime datumRodjenja
        )
        {
            this.Text = "Izmena podataka klijenta";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(420, 360);

            Label lblIme = new Label { Text = "Ime:", Left = 20, Top = 20, Width = 120 };
            txtIme = new TextBox { Left = 160, Top = 18, Width = 220, Text = ime };

            Label lblPrezime = new Label { Text = "Prezime:", Left = 20, Top = 60, Width = 120 };
            txtPrezime = new TextBox { Left = 160, Top = 58, Width = 220, Text = prezime };

            Label lblBrojPasosa = new Label { Text = "Broj pasoša:", Left = 20, Top = 100, Width = 120 };
            txtBrojPasosa = new TextBox { Left = 160, Top = 98, Width = 220, Text = brojPasosa };

            Label lblEmail = new Label { Text = "Email:", Left = 20, Top = 140, Width = 120 };
            txtEmail = new TextBox { Left = 160, Top = 138, Width = 220, Text = email };

            Label lblTelefon = new Label { Text = "Telefon:", Left = 20, Top = 180, Width = 120 };
            txtTelefon = new TextBox { Left = 160, Top = 178, Width = 220, Text = telefon };

            Label lblDatum = new Label { Text = "Datum rođenja:", Left = 20, Top = 220, Width = 120 };
            dtpDatum = new DateTimePicker
            {
                Left = 160,
                Top = 218,
                Width = 220,
                Format = DateTimePickerFormat.Short,
                Value = datumRodjenja
            };

            btnSacuvaj = new Button
            {
                Text = "Sačuvaj",
                Left = 160,
                Top = 270,
                Width = 100,
                Height = 32
            };
            btnSacuvaj.Click += (s, e) =>
            {
                // Osnovna validacija (po želji proširi)
                if (string.IsNullOrWhiteSpace(txtIme.Text) ||
                    string.IsNullOrWhiteSpace(txtPrezime.Text))
                {
                    MessageBox.Show("Ime i prezime su obavezni.", "Greška",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Ime = txtIme.Text.Trim();
                Prezime = txtPrezime.Text.Trim();
                BrojPasosa = txtBrojPasosa.Text.Trim();
                Email = txtEmail.Text.Trim();
                Telefon = txtTelefon.Text.Trim();
                DatumRodjenja = dtpDatum.Value.Date;

                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnOdustani = new Button
            {
                Text = "Otkaži",
                Left = 280,
                Top = 270,
                Width = 100,
                Height = 32
            };
            btnOdustani.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            Controls.AddRange(new Control[]
            {
                lblIme, txtIme,
                lblPrezime, txtPrezime,
                lblBrojPasosa, txtBrojPasosa,
                lblEmail, txtEmail,
                lblTelefon, txtTelefon,
                lblDatum, dtpDatum,
                btnSacuvaj, btnOdustani
            });
        }
    }
}
