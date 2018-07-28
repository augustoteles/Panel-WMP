using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rp4video
{
    public partial class Conf : Form
    {
        public Conf()
        {
            InitializeComponent();
        }

        public string _portNUmber = "";

        private void Conf_Load(object sender, EventArgs e)
        {
            this.BuscarPuertos();

        }

        private void BuscarPuertos()
        {
            cboPuerto.Items.Clear();
            string[] PuertosDisponibles = SerialPort.GetPortNames();
            foreach (string puerto_simple in PuertosDisponibles)
                cboPuerto.Items.Add(puerto_simple);

            if (cboPuerto.Items.Count == 0)
                return;

            cboPuerto.SelectedIndex = 0;
            _portNUmber = cboPuerto.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboPuerto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPuerto.SelectedIndex == -1)
                return;

            _portNUmber = cboPuerto.Text;
        }
    }
}
