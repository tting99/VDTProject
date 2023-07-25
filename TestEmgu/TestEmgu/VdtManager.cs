using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using System.Windows.Forms;
using Emgu.CV.Dai;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TestEmgu
{
    class AnalyzedData
    {
        public Mat AnalyzedImg;
        public string PoseResult;
    }
    class VdtManager
    {
        BackgroundWorker _backgroundWorker;
        VideoCapture _videoCapture;
        Mat _frame;
        Stopwatch stopwatch = new Stopwatch();//시간측정임 추후 삭제
        string prototxt = @"C:\openpose\models\pose\body_25\pose_deploy.prototxt";
        string modelPath = @"C:\openpose\models\pose\body_25\pose_iter_584000.caffemodel";

        public VdtManager(ProgressChangedEventHandler onProgressChanged)
        {
            _videoCapture = new VideoCapture(0);

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerReportsProgress = true; // 중간 보고 할거냐, 이걸 해줘야 중간보고를 할 수 있음
            _backgroundWorker.DoWork += new DoWorkEventHandler(DoWork); // 엔트리 포인트, 실행 할 함수를 매개변수로 줌
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(onProgressChanged); // 진행중인 진행 상활을 보고 받을거임
            _backgroundWorker.WorkerSupportsCancellation = true;
        }
        public void StartThread()
        {
            if(!_backgroundWorker.IsBusy)
            _backgroundWorker.RunWorkerAsync();
            
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            DetectPostureWithBody25(_backgroundWorker);

        }
        public double CalculateDistanceBetweenPoints(List<Point> point, int startpoint, int endpoint)
        {
           var result = Math.Sqrt(Math.Pow(point[startpoint].X - point[endpoint].X, 2)
                + Math.Pow(point[startpoint].Y - point[endpoint].Y, 2));

            return result;
        }
        public string DetectPose(List<Point> point)
        {
            List<Point> _point = point;
            double _dt17to18 = CalculateDistanceBetweenPoints(_point, 17, 18);
            double _dt2to5 = CalculateDistanceBetweenPoints(_point, 2, 5);

            if (_dt2to5 / _dt17to18 > 2)
                return "거북목";
            else
                return "정상";
        }
        public void DetectPostureWithBody25(BackgroundWorker worker)
        {
            stopwatch.Start();
            _frame = new Mat();
            Mat inputimg;   //프레임 받을 변수//Dictionaty -> 최상위는 컬렉션
 
            int inWidth = 368;
            int inHeight = 368;
            float threshold = 0.1f;
            int nPoints = 25;   //랜드마크
            var imgHeight = 0;
            var imgWidth = 0;

            var BODY_PARTS = new Dictionary<string, int>()  //Dictionaty -> 최상위는 컬렉션
                {
                    { "Nose", 0 },
                    { "Neck", 1 },
                    { "RShoulder", 2 },
                    { "RElbow", 3 },
                  //  { "RWrist", 4 },
                    {"LShoulder",5},
                    { "LElbow", 6 },
                 //   { "LWrist", 7 },
                 //   { "MidHip", 8 },
                  //  { "RHip", 9 },
                    //{ "RKnee", 10 },
                    //{"RAnkle",11},
                    { "LHip", 12 },
                    //{ "LKnee", 13 },
                  //  { "LAnkle", 14 },
                    { "REye", 15 },
                    { "LEye", 16 },
                    {"REar",17},
                    { "LEar", 18 },
                  //  { "LBigToe", 19 },
                 //   { "LSmallToe", 20 },
                 //   { "LHeel", 21 },
                 //   { "RBigToe", 22 },
                 //   {"RSmallToe",23},
                 //   { "RHeel", 24 },
                    { "Background", 25 }
                };
            int[,] point_pairs = new int[,]{
                            {1, 0}, {1, 2}, {1, 5},
                            {2, 3}, // {3, 4}, 
                            {5, 6},
                           // {6, 7},
                            {0, 15}, {15, 17},//랜드마크들의 연결 {1,0}-> 1마크와2마크는 연결
                            {0, 16}, {16, 18}
                    //{1, 8},//되어있다 간선 개념. (뼈다구)
                    // {8, 9}, 
                    //  {9, 10}, {10, 11},
                    //   {11, 22}, {22, 23}, {11, 24},
                    // {8, 12},
                    //    {12, 13}, {13, 14},
                    //    {14, 19}, {19, 20}, {14, 21}};
                };
            var net = DnnInvoke.ReadNetFromCaffe(prototxt, modelPath);
            int H = 0;
            int W = 0;
            Array HeatMap;
            List<Point> points;
            while (true)
            {
                try
                {
                    _videoCapture.Read(_frame);
                    var img = _frame;  //이미지코드 받을 객체 생성
                    imgHeight = img.Height;
                    imgWidth = img.Width;
                    //위에보면 img가 Bgr로 선언돼있고 이걸 인공지능이 처리할려면 mat으로 바꿔야한다
                    //BlobFromImage는 4차원 행렬 반환이다.
                    //BlobFromImage함수는 이미지 데이터를 입력으로 받아 딥러닝 모델에 적합한
                    //입력 블롭(blob)을 생성한다.
                    var blob = DnnInvoke.BlobFromImage(img, 1.0 / 255.0, new Size(inWidth, inHeight), new MCvScalar(0, 0, 0));
                    //  1.0 / 255.0 -> (이미지 정규화를 위해 필요한 스케일 계수)
                    //  new Size(inWidth, inHeight) -> ( 블롭의 크기로, 딥러닝 모델의 입력 크기와 일치해야 한다.)
                    //  new MCvScalar(0, 0, 0) -> (블롭의 평균(mean) 값으로, 입력 이미지의 각 채널에서 뺄셈(subtraction)에 사용)
                    net.SetInput(blob);
                    net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);
                    Debug.WriteLine(stopwatch.ElapsedMilliseconds + "ms- net.Forward()시작");//시간측정 삭제할것
                    var output = net.Forward();//젤 앞으로 옮긴다. (SetPreferableBackend에 장착한것을 맨앞으로)
                    //NCHW (BlobFromImage)에 마우스 대보기. NCHW에서 2번쨰 3번째
                    Debug.WriteLine(stopwatch.ElapsedMilliseconds + "ms- net.Forward()실행완료");
                    H = output.SizeOfDimension[2]; //히트맵 높이
                    W = output.SizeOfDimension[3]; //히트맵 너비]
                    HeatMap = output.GetData();//히트맵: 일기예보 폭염지역 빨갛게하는 개념
                                               // 랜드마크를 찍기위해 해당 영역에서 최댓값이 나온 포인트를 랜드마크로 찍는다.
                                               //여기서 히트맵은 어레이
                                               //사진이 한줄의 어레이로 된 히트맵을 인공지능이 다시 처리할 수 있게 매트릭스로
                                               //바꿔야한다. 그 방법은 아래에

                    points = new List<Point>();
                    for (int i = 0; i < nPoints; i++)//랜드마크 숫자만큼 찾겠다
                    {
                        Matrix<float> matrix = new Matrix<float>(H, W);//히트맵을 매트릭스로 바꾸기
                        for (int row = 0; row < H; row++)
                        {
                            for (int col = 0; col < W; col++)
                            {
                                matrix[row, col] = (float)HeatMap.GetValue(0, i, row, col);
                            }
                        }
                        double minVal = 0, maxVal = 0;
                        Point minLoc = default, maxLoc = default;
                        //밑에는 히트맵의 최소 최댓값을 담아달라 하고 그 아래에서 최댓값만 취하는것
                        CvInvoke.MinMaxLoc(matrix, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
                        //히트맵은 최댓값만 취한다.
                        var x = (img.Width * maxLoc.X) / W; //이걸 또 픽셀로 바꾸기 위해 이걸 씀
                        var y = (img.Height * maxLoc.Y) / H;
                        if (maxVal > threshold)
                        {
                            points.Add(new Point(x, y));//points(랜드마크)에 히트맵 최댓값을 채운다
                        }
                        else
                        {
                            points.Add(Point.Empty);    //만약 threshold(커트라인)에 걸린다면 버린다.
                                                        //포인트.empty는 (0,0)
                        }
                    }
                    // display points on image
                    for (int i = 0; i < points.Count; i++)
                    {
                        var p = points[i];
                        if (p != Point.Empty)
                        {
                                CvInvoke.Circle(img, p, 1, new MCvScalar(0, 0, 0), -1);
                                CvInvoke.PutText(img, i.ToString(), p, FontFace.HersheySimplex, 0.8, new MCvScalar(0, 0, 255), 1, LineType.AntiAlias);
                        }
                    }
                    // draw skeleton
                    for (int i = 0; i < point_pairs.GetLongLength(0); i++)
                    {
                        var startIndex = point_pairs[i, 0];
                        
                        var endIndex = point_pairs[i, 1];

                        if (points.Contains(points[startIndex]) && points.Contains(points[endIndex]))
                        {
                            if (!((startIndex == 2 && endIndex == 3) || (startIndex == 5 && endIndex == 6)))
                                if (!(points[startIndex].X == 0 || points[endIndex].X == 0))
                                {
                                    CvInvoke.Line(img, points[startIndex], points[endIndex], new MCvScalar(255, 0, 0), 2);
                                }

                        }
                    }
                    //pictureBox1.Image = img.ToBitmap(); //ima.ToBitmap하는게 더 빠르다.

                    //디텍트포즈(자세검출)메서드 호출

                    AnalyzedData analyzedata = new AnalyzedData();
                    analyzedata.AnalyzedImg = img;
                    analyzedata.PoseResult = DetectPose(points);
                    //


                    //
                    worker.ReportProgress(0, analyzedata);  //이미지 넘겨주기
                    stopwatch.Stop();   //시간측정
                    Debug.WriteLine(stopwatch.ElapsedMilliseconds+"ms");
                    //Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }//while
        }//DetectBody25
    }
}
