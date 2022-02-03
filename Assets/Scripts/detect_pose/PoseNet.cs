//using Unity.Barracuda;
//using UnityEngine;
//using System.Text;
//using UnityEngine.Video;
//using System.Collections.Generic;
//using System.Linq;
//using System.Collections;
//using UnityEngine.UI;
//using System;
//public class PoseNet{

//    private const int webcamHeight = 240;
//    private const int webcamWidth = 320;

//    public NNModel modelAsset;
//    public static WorkerFactory.Type workerType = WorkerFactory.Type.Auto;

//    private const int minConfidence = 70;

//    private static Model m_RunTimeModel;
//    private static IWorker engine;
//    static RenderTexture videoTexture;

//    private static string heatmapLayer = "float_heatmaps";
//    private static string offsetsLayer = "float_short_offsets";
//    private static string predictionLayer = "heatmap_predictions";

//    private const int numKeypoints = 17;

//    private static float[][] keypointLocations = new float[numKeypoints][];

//    private WebCamTexture webcamTexture;
//    private const int ImageHeight = 360;
//    private const int ImageWidth = 360;

//    public static Point temp;
//    private static double wrist_distance;

//    public struct Line
//    {
//        public Point p1;
//        public Point p2;
//    };

//    public struct Point
//    {
//        public double x;
//        public double y;
//    };

//    public static void Start(NNModel modelAsset)
//    {

//        m_RunTimeModel = ModelLoader.Load(modelAsset);
//        var modelBuilder = new ModelBuilder(m_RunTimeModel);
//        modelBuilder.Sigmoid(predictionLayer, heatmapLayer);
//        engine = WorkerFactory.CreateWorker(workerType, modelBuilder.model);

//        videoTexture = new RenderTexture(webcamWidth, webcamHeight, 24, RenderTextureFormat.ARGB32);

//    }

//    public static String Update(WebCamTexture camTexture, ComputeShader posenetShader)
//    {
//        Graphics.Blit(camTexture, videoTexture);

//        Texture2D processedImage = PreprocessImage(camTexture, posenetShader);
//        Tensor input = new Tensor(processedImage, channels: 3);
//        engine.Execute(input);
//        ProcessOutput(engine.PeekOutput(predictionLayer), engine.PeekOutput(offsetsLayer));

//        input.Dispose();
//        engine.Dispose();

//        return pose_det(keypointLocations);

//    }

//    public static String Update_image(byte[] pixels, ComputeShader posenetShader)
//    {
//        Texture2D camTexture = new Texture2D(100, 100, TextureFormat.RGBA32, false);
//        camTexture.LoadImage(pixels);

//        Graphics.Blit(camTexture, videoTexture);

//        Texture2D processedImage = PreprocessImage_image(camTexture, posenetShader);
//        Tensor input = new Tensor(processedImage, channels: 3);
//        engine.Execute(input);
//        ProcessOutput(engine.PeekOutput(predictionLayer), engine.PeekOutput(offsetsLayer));

//        input.Dispose();
//        engine.Dispose();

//        return pose_det(keypointLocations);

//    }

//    private static Texture2D PreprocessImage_image(Texture2D camTexture, ComputeShader posenetShader)
//    {
//        Texture2D imageTexture = new Texture2D(camTexture.width, camTexture.height, TextureFormat.RGB24, false);

//        Graphics.CopyTexture(camTexture, imageTexture);

//        Texture2D tempTex = Resize(imageTexture, ImageHeight, ImageWidth);

//        imageTexture = PreprocessResNet(posenetShader, tempTex);

//        return imageTexture;
//    }



//    private static Texture2D PreprocessImage(WebCamTexture camTexture, ComputeShader posenetShader)
//    {
//        Texture2D imageTexture = new Texture2D(webcamWidth, webcamHeight, TextureFormat.RGBA32, false);

//        Graphics.CopyTexture(camTexture, imageTexture);

//        Texture2D tempTex = Resize(imageTexture, ImageHeight, ImageWidth);

//        imageTexture = PreprocessResNet(posenetShader, tempTex);

//        return imageTexture;
//    }


//    private static Texture2D Resize(Texture2D image, int newWidth, int newHeight)
//    {

//        RenderTexture rTex = RenderTexture.GetTemporary(newWidth, newHeight, 24);
//        RenderTexture.active = rTex;

//        Graphics.Blit(image, rTex);

//        Texture2D nTex = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);

//        Graphics.CopyTexture(rTex, nTex);

//        RenderTexture.active = null;

//        RenderTexture.ReleaseTemporary(rTex);
//        return nTex;
//    }


//    private static Texture2D PreprocessResNet(ComputeShader posenetShader, Texture2D inputImage)
//    {

//        int numthreads = 8;
//        int kernelHandle = posenetShader.FindKernel("PreprocessResNet");
//        RenderTexture rTex = new RenderTexture(inputImage.width, inputImage.height, 24, RenderTextureFormat.ARGBHalf);
//        rTex.enableRandomWrite = true;
//        rTex.Create();

//        posenetShader.SetTexture(kernelHandle, "Result", rTex);
//        posenetShader.SetTexture(kernelHandle, "InputImage", inputImage);

//        posenetShader.Dispatch(kernelHandle, inputImage.height / numthreads, inputImage.width / numthreads, 1);
//        RenderTexture.active = rTex;

//        Texture2D nTex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBAHalf, false);

//        Graphics.CopyTexture(rTex, nTex);
//        RenderTexture.active = null;

//        return nTex;
//    }


//    private static void ProcessOutput(Tensor heatmaps, Tensor offsets)
//    {
//        float stride = (ImageHeight - 1) / (heatmaps.shape.height - 1);
//        stride -= (stride % 8);

//        int minDimension = Mathf.Min(webcamWidth, webcamHeight);
//        int maxDimension = Mathf.Max(webcamWidth, webcamHeight);
        
//        float scale = (float)minDimension / (float)Mathf.Min(ImageWidth, ImageHeight);
//        float unsqueezeScale = (float)maxDimension / (float)minDimension;

//        for (int k = 0; k < numKeypoints; k++)
//        {
//            var locationInfo = LocateKeyPointIndex(heatmaps, offsets, k);
//            var coords = locationInfo.Item1;
//            var offset_vector = locationInfo.Item2;
//            var confidenceValue = locationInfo.Item3;

//            float xPos = (ImageWidth - (coords[0] * stride + offset_vector[0])) * scale;
//            float yPos = (ImageHeight - (coords[1] * stride + offset_vector[1])) * scale;

//            xPos *= unsqueezeScale;
//            xPos = webcamWidth - xPos ;

//            keypointLocations[k] = new float[] { xPos, yPos, confidenceValue };
//        }
//    }

//    private static (float[], float[], float) LocateKeyPointIndex(Tensor heatmaps, Tensor offsets, int keypointIndex)
//    {
//        float maxConfidence = 0f;

//        float[] coords = new float[2];
//        float[] offset_vector = new float[2];

//        for (int y = 0; y < heatmaps.shape.height; y++)
//        {
//            for (int x = 0; x < heatmaps.shape.width; x++)
//            {
//                if (heatmaps[0, y, x, keypointIndex] > maxConfidence)
//                {
//                    maxConfidence = heatmaps[0, y, x, keypointIndex];
//                    coords = new float[] { x, y };
//                    offset_vector = new float[]
//                    {
//                            // X-axis offset
//                            offsets[0, y, x, keypointIndex + numKeypoints],
//                            // Y-axis offset
//                            offsets[0, y, x, keypointIndex]
//                    };
//                }
//            }
//        }

//        return (coords, offset_vector, maxConfidence);
//    }

//    static bool comparator(Point left, Point right)
//    {
//        if (left.x == right.x)
//        {
//            if (left.y <= right.y)
//                return true;
//            else
//                return false;
//        }
//        else
//        {
//            if (left.x <= right.x)
//                return true;
//            else
//                return false;
//        }
//    }

//    static int ccw(Point p1, Point p2, Point p3)
//    {
//        double cross_product = (p2.x - p1.x) * (p3.y - p1.y) - (p3.x - p1.x) * (p2.y - p1.y);

//        if (cross_product > 0)
//        {
//            return 1;
//        }
//        else if (cross_product < 0)
//        {
//            return -1;
//        }
//        else
//        {
//            return 0;
//        }
//    }

//    static bool LineIntersection(Line l1, Line l2)
//    {
//        bool ret;
//        // l1을 기준으로 l2가 교차하는 지 확인한다.
//        // Check if l2 intersects based on l1.
//        int l1_l2 = ccw(l1.p1, l1.p2, l2.p1) * ccw(l1.p1, l1.p2, l2.p2);
//        // l2를 기준으로 l1이 교차하는 지 확인한다.
//        // Check if l1 intersects based on l2.
//        int l2_l1 = ccw(l2.p1, l2.p2, l1.p1) * ccw(l2.p1, l2.p2, l1.p2);


//        // l1 과 l2가 일직선 상에 있는 경우
//        // l1 and l2 are in same line.
//        if (l1_l2 == 0 && l2_l1 == 0)
//        {
//            // Line1의 점의 크기 순서를 p1 < p2 순서로 맞춘다.
//            // Set the order of the points on Line1 in the order p1 < p2.
//            if (comparator(l1.p2, l1.p1))
//            {
//                temp = l1.p2;
//                l1.p2 = l1.p1;
//                l1.p1 = temp;
//            }
//            // Line2의 점의 크기 순서를 p1 < p2 순서로 맞춘다.
//            // Set the order of the points on Line2 in the order p1 < p2.
//            if (comparator(l2.p2, l2.p1))
//            {
//                temp = l2.p1;
//                l2.p1 = l2.p2;
//                l2.p2 = temp;
//            }

//            // l1.p1 -----------l1.p2
//            //         l2.p1 -----------l2.p2
//            // 위 조건을 만족하는 지 살펴본다.
//            // See if the above conditions are met.
//            if ((comparator(l2.p1, l1.p2)) && (comparator(l1.p1, l2.p2)))
//                ret = true;
//            else
//                ret = false;
//        }
//        // l1과 l2가 일직선 상에 있지 않는 경우
//        // l1 and l2 are not in same line.
//        else
//        {
//            if ((l1_l2 <= 0) && (l2_l1 <= 0))
//                ret = true;
//            else
//                ret = false;
//        }
//        return ret;
//    }

//    static String pose_det(float[][] keypoints) {
//        String pose;
//        Line l1, l2;

//        l1.p1.x = keypoints[7][0];
//        l1.p1.y = keypoints[7][1];
//        l1.p2.x = keypoints[9][0];
//        l1.p2.y = keypoints[9][1];

//        l2.p1.x = keypoints[8][0];
//        l2.p1.y = keypoints[8][1];
//        l2.p2.x = keypoints[10][0];
//        l2.p2.y = keypoints[10][1];

//        wrist_distance = Math.Sqrt(Math.Pow(keypoints[9][0] - keypoints[10][0], 2) - Math.Pow(keypoints[9][1] - keypoints[10][1], 2));
//        Debug.Log(keypoints[9][0]);
//        Debug.Log(keypoints[10][0]);
//        Debug.Log(keypoints[7][0]);
//        Debug.Log(keypoints[8][0]);

//        if (wrist_distance <= 4)
//        {
//            pose = "Detect Error";
//        }
//        else if (LineIntersection(l1, l2))
//        {
//            pose = "Hands X";
//        }
//        else if (keypoints[9][1] >= keypoints[0][1] && keypoints[10][1] >= keypoints[0][1])
//        {
//            if ((keypoints[7][0] - keypoints[8][0]) > (keypoints[9][0] - keypoints[10][0] + 50))
//            {
//                pose = "Hands O";
//            }
                
//            else
//                pose = "Both Hands Up";
//        }
//        else if (keypoints[9][1] >= keypoints[0][1])
//        {
//            pose = "Left Hand Up";
//        }
//        else if (keypoints[10][1] >= keypoints[0][1])
//        {
//            pose = "Right Hand Up";
//        }
//        else
//        {
//            pose = "Hands Down";
//        }

//        return pose;
//    }


//}