using Emgu.CV;
using System.Windows.Forms;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Emgu.CV.Structure;
using Emgu.CV.Dnn;  //딥러닝 서포터 라이브러리
using Emgu.CV.CvEnum;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.ComponentModel;
using System.Reflection.Emit;

namespace TestEmgu
{
    public partial class Form1 : Form
    {
        VdtManager _vdtManager;

        public Form1()
        {
            InitializeComponent();
            _vdtManager = new VdtManager(OnProgressChanged);    //VDT안에 다 꺼주는 메서드 생성할것.
        }
        public void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AnalyzedData analyzedata = e.UserState as AnalyzedData;
            Mat img = analyzedata.AnalyzedImg;
            pictureBox1.Image = img.ToBitmap();

            lbPoseResult.Text = analyzedata.PoseResult;
        }
        public void btnAiProcess_Click(object sender, EventArgs e)
        {
            _vdtManager.StartThread();
        }

        private void BtnCap_Click(object sender, EventArgs e)
        {

            Mat frame = new Mat();
            VideoCapture _cap = new VideoCapture();
            _cap.Read(frame);
            pictureBox1.Image = frame.ToBitmap();

        }
    }
}