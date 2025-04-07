using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;
using System.Net.NetworkInformation;
using System.Drawing.Drawing2D;
using System.Xml.Linq;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace v3
{
    public partial class Form1 : Form
    {
        int kutu = 0;
        public int sayac = 0;
        int sayac2 = 0;
        string selectedMusic1;
        int listemsayac = 0; //listbox ile treeview arasında geçiş yapar. search buttona tıklandığında listbox öne gelir.
        int listemsayac2 = 0; //listbox ile treeview arasında geçiş yapar.

        Form3 form3 = new Form3();
        Form2 form2 = new Form2();

        private const string FilePath = "treeData.txt"; //verilerin kaydedildiği yer


        private YouTubeService youtubeService;
        public Form1()
        {
            InitializeComponent();
            InitializeYouTubeService("API KEY");
            form3.Owner = this;
            treeView1.Nodes[0].Expand();
            LoadTreeData();
        }


        private void LoadTreeData()
        {
            treeView1.Nodes[0].Nodes.Clear();

            // Dosyadan verileri yükle
            if (System.IO.File.Exists(System.IO.Path.Combine(Application.StartupPath, FilePath)))
            {
                using (StreamReader reader = new StreamReader(System.IO.Path.Combine(Application.StartupPath, FilePath)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        treeView1.Nodes[0].Nodes.Add(line);
                    }
                }
            }
        }  //pc'deki verileri treeview içine yükler.

        private void SaveTreeData()
        {
            // TreeView verilerini dosyaya kaydet
            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(Application.StartupPath, FilePath)))
            {
                foreach (TreeNode node in treeView1.Nodes[0].Nodes)
                {
                    writer.WriteLine(node.Text);
                }
            }
        } //treeview içindeki verileri pc'ye kaydeder. form kapanırken çağrılır.


        private void InitializeYouTubeService(string apiKey)
        {
            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "YoutubeMusic"
            });
        }



        private List<string> SearchVideos(string query)
        {
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = 5;
            searchListRequest.Type = "video";

            var searchListResponse = searchListRequest.Execute();
            var videoList = new List<string>();

            foreach (var searchResult in searchListResponse.Items)
            {
                videoList.Add(searchResult.Snippet.Title);
            }

            return videoList;
        } //videoları servis üzerinden çeker. Aranılan sayı değiştirilebilir. Şu anda 5 tane.



        private string GetVideoThumbnailUrl(string videoId)
        {
            string thumbnailUrl = $"https://i.ytimg.com/vi/{videoId}/maxresdefault.jpg";

            // Eğer resim yüklenmezse, varsayılan bir resim döndür
            if (!ImageExists(thumbnailUrl))
            {
                thumbnailUrl = $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg";
            }

            return thumbnailUrl;
        } //hd resim bulup çeker. yoksa altındakini çalıştırır.



        private bool ImageExists(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception)
            {
                return false;
            }
        } //hd resim yoksa varsayılan resmi alır.



        private string GetVideoId(string musicTitle)
        {
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = musicTitle;
            searchListRequest.MaxResults = 1;
            searchListRequest.Type = "video";

            var searchListResponse = searchListRequest.Execute();
            string videoId = searchListResponse.Items[0].Id.VideoId;

            return videoId;
        } //video id çeker.



        //private string GetMostViewedVideoUrl(string musicTitle)
        //{
        //    var searchListRequest = youtubeService.Search.List("snippet");
        //    searchListRequest.Q = musicTitle;
        //    searchListRequest.MaxResults = 1;
        //    searchListRequest.Type = "video";
        //    searchListRequest.Order = SearchResource.ListRequest.OrderEnum.ViewCount;

        //    var searchListResponse = searchListRequest.Execute();
        //    string videoId = searchListResponse.Items[0].Id.VideoId;

        //    return $"https://www.youtube.com/watch?v={videoId}";
        //}
        // en çok izlenen videoyu bulur.



        private void SearchButton_Click_1(object sender, EventArgs e) //listeye arananları ekler.
        {
            string searchTerm = musicTextBox.Text;
            List<string> searchResults = SearchVideos(searchTerm);

            // ListBox'a sonuçları ekleme
            musicListBox.DataSource = searchResults;
            treeView1.SendToBack();
            if (listemsayac == 0)
            {
                treeView1.SendToBack();
                musicListBox.SelectedItem = null;
                treeView1.SelectedNode = null;
                listemsayac = 1;
            }
            listemsayac2 = 1;

        }


        //private void musicListBox_MouseClick(object sender, MouseEventArgs e)
        //{
        //    int index = musicListBox.IndexFromPoint(e.Location);
        //    if (index != ListBox.NoMatches)
        //    {
        //        string selectedMusic = musicListBox.Items[index].ToString();
        //        string videoId = GetVideoId(selectedMusic);

        //        // Müzik resmini göster
        //        string thumbnailUrl = GetVideoThumbnailUrl(videoId);
        //        if (!string.IsNullOrEmpty(thumbnailUrl))
        //        {
        //            // HTML sayfasını oluştur
        //            string html = $"<html><body style='margin:0; padding:0;'><img src='{thumbnailUrl}' style='max-width:100%; max-height:100%;'></body></html>";

        //            // WebBrowser kontrolüne HTML içeriğini yükle
        //            webBrowser1.DocumentText = html;

        //        }
        //    }
        //}



        public void PlayMusic()
        {
            //if (musicListBox.SelectedItem != null)
            //{
            //form3.deaktif(0);  //müziği tekrar oynatmasın diye eklendi. (286. satır)


            //selectedMusic = musicListBox.SelectedItem.ToString();
            string videoId = GetVideoId(selectedMusic1);



            // YouTube videosunu varsayılan tarayıcıda açma
            //Process.Start(new ProcessStartInfo($"https://www.youtube.com/watch?v={videoId}"));
            //string dosya = ($"https://www.youtube.com/embed/{videoId}?autoplay=1");

            //https://www.youtube.com/embed/{videoId}?autoplay=1

            textBox1.Text = (videoId); //gelistirici moduna özel



            string html = "<html><head>";
            html += "<meta content='IE=Edge' http-equiv='X-UA-Compatible'/>";
            html += "<iframe id='video' src='https://www.youtube.com/embed/{0}?autoplay=1' width='923' height='490' frameborder='0' allowfullscreen></iframe>";
            html += "</head></html>";
            form2.LoadWebBrowser(string.Format(html, videoId));
            //}
        } //seçilen müziği oynatır.



        private void musicTextBox_TextChanged(object sender, EventArgs e)  //arama kutusun içindeki labeli kaldırır ve arama tuşunu aktif eder.
        {
            if (musicTextBox.Text == "s1000rr")
            {
                button1.Enabled = true;
            }
            //else { button1.Enabled = false; }

            if (!string.IsNullOrEmpty(musicTextBox.Text)) //ara ifadesinin ayarlanması
            {
                label1.Visible = false;
            }

            else
            {
                label1.Visible = true;
            }

            if (string.IsNullOrWhiteSpace(musicTextBox.Text))
            {
                pictureBox1.Enabled = false;
            }

            else
            {
                pictureBox1.Enabled = true;
            }

            kutu = 0;

        } //içinde ara



        private void button1_Click(object sender, EventArgs e)  //geliştirici modu
        {

            if (sayac == 0)
            {

                textBox1.Visible = true;
                form2.Show();
                sayac = 1;
            }

            else
            {
                form2.Hide();
                textBox1.Visible = false;
                sayac = 0;
            }

        }


        private void musicListBox_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (kutu == 0)
            {

                musicListBox.SelectedItem = null;
                kutu = 1;

            }

            //if (selectedMusic != null) //müziği tekrar oynatmasın diye eklendi. (189. satır) çalışırken sorun çıkıyor.
            //{
            //    if (selectedMusic == musicListBox.SelectedItem.ToString())
            //    {
            //        form3.deaktif(0);
            //    }
            //    else
            //    {
            //        form3.deaktif(1);
            //    }
            //}




            int index = musicListBox.SelectedIndex;

            if (index != ListBox.NoMatches)
            {
                string selectedMusic = musicListBox.Items[index].ToString();
                //string videoId = GetVideoId(selectedMusic);

                //// Müzik resmini göster                                           //bunun için bir alttaki fonksiyonu yaptık.
                //string thumbnailUrl = GetVideoThumbnailUrl(videoId);

                //if (!string.IsNullOrEmpty(thumbnailUrl))
                //{

                //    form3.LoadPictureBox(thumbnailUrl);

                //}
                //if (sayac2 == 0)
                //{
                //    form3.Show();
                //    sayac2 = 1;
                //}

                //selectedMusic1 = musicListBox.SelectedItem.ToString();
                showPicture(selectedMusic);
                form3.musicname(selectedMusic);
            }

        }  //musicListBox içinde seçileni showPicture içine göndeririz.



        private void showPicture(string selectedMusic)    //2. formda müziğin resmini gösterir. hem listbox hem de treeview için.
        {
            string videoId = GetVideoId(selectedMusic);

            // Müzik resmini göster
            string thumbnailUrl = GetVideoThumbnailUrl(videoId);

            if (!string.IsNullOrEmpty(thumbnailUrl))
            {

                form3.LoadPictureBox(thumbnailUrl);

            }
            if (sayac2 == 0)
            {
                form3.Show();
                sayac2 = 1;
            }

            selectedMusic1 = selectedMusic.ToString();
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)//treeview içinde seçileni showPicture içine göndeririz.
        {
            string selectedMusic;
            if (e.Node != null)
            {
                if (e.Node.Parent != null && e.Node.Parent.Index == 0) //üstte olan düğümün olup olmadığını ve varsa ilk düğüm olup olmadığına bakar.
                {
                    selectedMusic = e.Node.Text;
                    showPicture(selectedMusic);
                    form3.musicname(selectedMusic);
                }
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            GraphicsPath path = new GraphicsPath();   //arama kutusunun çevresini oval yapar.
            int radius = 40; // Oval köşe yarıçapı
            path.AddArc(0, 0, radius, radius, 180, 90); // Sol üst köşe
            path.AddArc(pictureBox2.Width - radius, 0, radius, radius, 270, 90); // Sağ üst köşe
            path.AddArc(pictureBox2.Width - radius, pictureBox2.Height - radius, radius, radius, 0, 90); // Sağ alt köşe
            path.AddArc(0, pictureBox2.Height - radius, radius, radius, 90, 90); // Sol alt köşe
            pictureBox2.Region = new Region(path);

        } //form yüklenirken çalışır. arama kutusun etrafını oval yapar.

        private void button3_Click(object sender, EventArgs e)//listeyi gösterme-gizleme yapar.
        {
            if (listemsayac2 == 1)
            {
                if (listemsayac == 0)
                {
                    treeView1.SendToBack();
                    musicListBox.SelectedItem = null;
                    treeView1.SelectedNode = null;
                    listemsayac = 1;
                }
                else
                {
                    treeView1.BringToFront();
                    musicListBox.SelectedItem = null;
                    treeView1.SelectedNode = null;
                    listemsayac = 0;
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)  //listeye müzik ekleme. çookkkk zorrrr. işte boooo.
        {
            int listesayac = 0;
            int treesayi = treeView1.Nodes[0].Nodes.Count;

            if (treesayi == 0)
            {
                if (musicListBox.SelectedItem != null)
                {
                    treeView1.Nodes[0].Nodes.Add(musicListBox.SelectedItem.ToString()); // null ekle*****
                }
                else
                {
                    MessageBox.Show("Müzik seçiniz.");
                }
            }
            else if (treesayi > 0)
            {
                if (musicListBox.SelectedItem != null)
                {
                    for (int i = 0; i < treesayi; i++)
                    {

                        if (treeView1.Nodes[0].Nodes[i].Text == musicListBox.SelectedItem.ToString())
                        {
                            MessageBox.Show("Listede aynı isimde müzik var.");
                            listesayac = 1;
                            break;
                        }
                    }
                    if (listesayac == 0)
                    {
                        treeView1.Nodes[0].Nodes.Add(musicListBox.SelectedItem.ToString());
                    }
                }
            }

        }

        private void button4_Click(object sender, EventArgs e)  //listeden şarkıyı kaldırır.
        {
            if (treeView1.SelectedNode != null)
            {

                if (treeView1.SelectedNode.Parent != null && treeView1.SelectedNode.Parent.Index == 0) //üstte olan düğümün olup olmadığını ve varsa ilk düğüm olup olmadığına bakar.
                {
                    treeView1.Nodes[0].Nodes.Remove(treeView1.SelectedNode);

                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveTreeData();
            e.Cancel = false;
        } //form kapanırken treeview içindeki verileri pc'ye kaydeder.
    }
}

