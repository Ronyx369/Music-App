using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi; //sesi ayarlamak için kütüphane

namespace v3
{
    public partial class Form3 : Form
    {
        private MMDeviceEnumerator deviceEnumerator;
        private MMDevice defaultDevice;
        
        public Form3()
        {
            InitializeComponent();
            deviceEnumerator = new MMDeviceEnumerator();
            defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            
        }


        public void LoadPictureBox(string pbname)
        {
            pictureBox1.Load(pbname);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        } //müziğin resmini picturebox'a yükler.


        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (Owner is Form1 form1)
            {
                form1.PlayMusic();
            }
        


        } //oynatma tuşu

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            AdjustVolume(10);
        } //ses artır

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            AdjustVolume(-10);
        } //ses azalt

        private void AdjustVolume(int volumeChange)
        {
            int newVolume = (int)Math.Max(0, Math.Min(100, defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100 + volumeChange));

            defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = newVolume / 100.0f;
        } //ses ayarlama methodu

        public void musicname(string selectedMusic)
        {
            label1.Text = selectedMusic;
        } //seçilen müziğin adını gösterir

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
