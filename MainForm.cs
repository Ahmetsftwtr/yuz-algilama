

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Diagnostics;

namespace MultiFaceRec
{
    public partial class FrmPrincipal : Form
    {
        //Declararation of all variables, vectors and haarcascades
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade haargoz;
        HaarCascade haaragiz;
        HaarCascade haaryuz;



        
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels= new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;


        public FrmPrincipal()
        {
            InitializeComponent();
            //Load haarcascades for face detection
            haaryuz = new HaarCascade("haarcascade_frontalface_default.xml");
            haargoz = new HaarCascade("haarcascade_eye.xml");
            haaragiz = new HaarCascade("haarcascade_mcs_mouth.xml");

            //eye = new HaarCascade("haarcascade_eye.xml");
            try
            {
                //Load of previus trainned faces and labels for each image
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 1; tf < NumLabels+1; tf++)
                {
                    LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/" + LoadFaces));
                    labels.Add(Labels[tf]);
                }
            
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString());
                MessageBox.Show("Lütfen 1 den fazla database kayıdı eklemeyiniz.Yüz kaydı eklemek için kamera başlat butonuna basınız.", "Tanımlı Yüz Mevcut", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Initialize the capture device
            grabber = new Capture();

            grabber.QueryFrame();
            //Initialize the FrameGraber event
            Application.Idle += new EventHandler(FrameGrabber);
            button1.Enabled = false;
        }


        private void button2_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Trained face counter // tanımlanan yüzün sayacı
                ContTrain = ContTrain + 1;

                //Get a gray frame from capture device
                gray = grabber.QueryGrayFrame().Resize(800, 600, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                //Face Detector


                MCvAvgComp[][] Yuzler = gray.DetectHaarCascade(haaryuz, 1.2, 5, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(15, 15));

                MCvAvgComp[][] Gozler = gray.DetectHaarCascade(haargoz, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(15, 15));
                MCvAvgComp[][] Agizlar = gray.DetectHaarCascade(haaragiz, 1.2, 100, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(15, 15));


                foreach (MCvAvgComp yuz in Yuzler[0])
                    currentFrame.Draw(yuz.rect, new Bgr(Color.Red), 2);
                foreach (MCvAvgComp goz in Gozler[0])
                    currentFrame.Draw(goz.rect, new Bgr(Color.Black), 2);
                foreach (MCvAvgComp agiz in Agizlar[0])
                    currentFrame.Draw(agiz.rect, new Bgr(Color.Blue), 2);



                //resize face detected image for force to compare the same size with the 
                //test image with cubic interpolation type method
                TrainedFace = result.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                trainingImages.Add(TrainedFace);
                labels.Add(textBox1.Text);

                //Show face added in gray scale
                imageBox1.Image = TrainedFace;
         
                //Write the number of triained faces in a file text for further load
                File.WriteAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + "%");

                //Write the labels of triained faces in a file text for further load
                for (int i = 2; i < trainingImages.ToArray().Length + 1; i++)
                {
                    trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/TrainedFaces/face" + i + ".bmp");
                    File.AppendAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", labels.ToArray()[i - 1] + "%");
                }

                MessageBox.Show(textBox1.Text + " yüzü Tanımlandı.", "Tanımlama Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            catch
            {
                MessageBox.Show("Öncelikle Yüz tanımlamayı başlatınız.", "Tanımlama Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        void FrameGrabber(object sender, EventArgs e)
        {
            label3.Text = "0";
            //label4.Text = "";
            NamePersons.Add("");


            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(800, 660, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                    //Convert it to Grayscale
                    gray = currentFrame.Convert<Gray, Byte>();

                    //Face Detector
 

                    MCvAvgComp[][] Yuzler = gray.DetectHaarCascade(haaryuz, 1.2, 5, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(15, 15));
                    MCvAvgComp[][] Gozler = gray.DetectHaarCascade(haargoz, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(15, 15));
                    MCvAvgComp[][] Agizlar = gray.DetectHaarCascade(haaragiz, 1.2, 100, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(15, 15));



                

                    //Action for each element detected
                    foreach (MCvAvgComp yuz in Yuzler[0])
                        currentFrame.Draw(yuz.rect, new Bgr(Color.Red), 2);
                    foreach (MCvAvgComp goz in Gozler[0])
                        currentFrame.Draw(goz.rect, new Bgr(Color.Black), 2);
                    foreach (MCvAvgComp agiz in Agizlar[0])
                        currentFrame.Draw(agiz.rect, new Bgr(Color.Blue), 2);


                    foreach (MCvAvgComp f in Yuzler[0])
                    {
                        t = t + 1;
                        result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                        currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);


                        if (trainingImages.ToArray().Length != 0)
                        {

                            MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);


                            EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                               trainingImages.ToArray(),
                               labels.ToArray(),
                               3000,
                               ref termCrit);

                            name = recognizer.Recognize(result);


                            currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));

                        }

                        NamePersons[t - 1] = name;
                        NamePersons.Add("");



                        label4.Text = Yuzler[0].Length.ToString();




                    }

                    t = 0;


                    for (int nnn = 0; nnn < Yuzler[0].Length; nnn++)
                    {
                        names = names + NamePersons[nnn] + ", ";
                    }

                    imageBoxFrameGrabber.Image = currentFrame;
                    label4.Text = names;
                    names = "";

                    NamePersons.Clear();

        }


    }
}
