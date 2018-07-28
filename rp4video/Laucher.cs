using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rp4video
{
    public partial class Launcher : Form
    {
        private delegate void delegadoAcceso(string accion);
        private string strBufferIn;
        private string _portNumber;

        private string video { get; set; }
        private string UltimoVideo { get; set; } = "";

        private string _mainPath = @"c:\tmp\WMPlayer\";
        private string _pathNameScwitcher = @"currentVideo.txt";

        public Launcher()
        {
            InitializeComponent();
        }

        private void Laucher_Load(object sender, EventArgs e)
        {
            Conf conf = new Conf();
            conf.ShowDialog(this);
            _portNumber = conf._portNUmber;

            if (this.Conectar())
                timer1.Enabled = true;

            axWindowsMediaPlayer1.settings.setMode("loop", true);
        }

        private void AccesoForm(string accion)
        {
            strBufferIn = accion;
            this.Text = strBufferIn;
            File.WriteAllText(($"{_mainPath}{_pathNameScwitcher}").ToLower(), strBufferIn);
        }

        private void AccesoInterrupcion(string accion)
        {
            delegadoAcceso varDelegado = new delegadoAcceso(AccesoForm);
            object[] arg = { accion };
            base.Invoke(varDelegado, arg);
        }

        private void DatoRecibido(object sender, SerialDataReceivedEventArgs e)
        {
            AccesoInterrupcion(spPuerto.ReadExisting());
        }

        public bool Conectar()
        {
            bool result = false;

            spPuerto.BaudRate = 9600;
            spPuerto.DataBits = 8;
            spPuerto.Parity = Parity.None;
            spPuerto.StopBits = StopBits.One;
            spPuerto.Handshake = Handshake.None;
            try
            {
                spPuerto.PortName = _portNumber;
                spPuerto.Open();
                result = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }

            return result;
        }

        #region Video

        public void Reproducir(string video)
        {
            if (video == "0")
                axWindowsMediaPlayer1.settings.setMode("loop", true);
            else
                axWindowsMediaPlayer1.settings.setMode("false", true);

            axWindowsMediaPlayer1.URL = ($@"{_mainPath}{video}.mp4").ToLower();
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //leemos el archivo
            List<string> fileList = File.ReadAllLines(($"{_mainPath}{_pathNameScwitcher}").ToLower()).ToList();
            this.UltimoVideo = fileList.First();
            //validamos si el numero de video a cambiado
            if (this.UltimoVideo != this.video)
            {   //si cambia cambiamos el video
                this.Reproducir(this.UltimoVideo);
                this.video = this.UltimoVideo;
            }

        }

        #endregion

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            this.Text = axWindowsMediaPlayer1.playState.ToString();
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
                File.WriteAllText(($"{_mainPath}{_pathNameScwitcher}").ToLower(), "0");

            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
        }

        private void axWindowsMediaPlayer1_SizeChanged(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
        }
    }
}
