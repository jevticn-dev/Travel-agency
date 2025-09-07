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
    public partial class FormaNovaUsluga : Form
    {
        private TextBox txtNaziv, txtCena;
        private Button btnSacuvaj, btnOdustani;

        public string Naziv { get; private set; }
        public string Cena { get; private set; }

        public FormaNovaUsluga()
        {
            Text = "Nova usluga";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 180);

            var lblNaziv = new Label { Text = "Naziv:", Left = 20, Top = 20, AutoSize = true };
            txtNaziv = new TextBox { Left = 120, Top = 16, Width = 260 };

            var lblCena = new Label { Text = "Cena:", Left = 20, Top = 60, AutoSize = true };
            txtCena = new TextBox { Left = 120, Top = 56, Width = 260 };

            btnSacuvaj = new Button { Text = "Sačuvaj", Left = 200, Top = 110, Width = 90 };
            btnOdustani = new Button { Text = "Otkaži", Left = 290, Top = 110, Width = 90 };

            btnOdustani.Click += (s, e) => DialogResult = DialogResult.Cancel;
            btnSacuvaj.Click += (s, e) =>
            {
                var naziv = (txtNaziv.Text ?? "").Trim();
                var cena = (txtCena.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(naziv))
                {
                    MessageBox.Show("Unesite naziv.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(cena))
                {
                    MessageBox.Show("Unesite cenu.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Naziv = naziv;
                Cena = cena;
                DialogResult = DialogResult.OK;
            };

            Controls.AddRange(new Control[] { lblNaziv, txtNaziv, lblCena, txtCena, btnSacuvaj, btnOdustani });
        }
    }
}
