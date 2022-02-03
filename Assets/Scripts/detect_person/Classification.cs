//using UnityEngine;
//using UnityEngine.Events;
//using Unity.Barracuda;
//using System.Collections.Generic;
//using System.Linq;
//using System.Collections;
//using UnityEngine.UI;
//using System;
//using System.Runtime.InteropServices;

//public class Classification{

//    const int IMAGE_SIZE = 416;
//    const string INPUT_NAME = "input_1";
//    const string OUTPUT_NAME_L = "conv2d_10";
//    const string OUTPUT_NAME_M = "conv2d_13";
//    const float MINIMUM_CONFIDENCE = 0.3f;


//    WebCamTexture camTexture;

//    string[] labels = {"person"};
//    static byte[] pixels;

//    static Dictionary<string, int> params_l = new Dictionary<string, int>() { { "ROW_COUNT", 13 }, { "COL_COUNT", 13 }, { "CELL_WIDTH", 32 }, { "CELL_HEIGHT", 32 } };
//    static Dictionary<string, int> params_m = new Dictionary<string, int>() { { "ROW_COUNT", 26 }, { "COL_COUNT", 26 }, { "CELL_WIDTH", 16 }, { "CELL_HEIGHT", 16 } };

//    static IWorker worker;
//    Color[] colorArray = new Color[] { Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow };

//    public static void Start(NNModel modelFile)
//    {
//        var model = ModelLoader.Load(modelFile);
//        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
//    }

//    public static List<Rect> Update(byte[] pixels)
//    {
//        return RunModel(pixels);
//    }

//    public static List<Rect> RunModel(byte[] pixels)
//    {
//        Tensor inputTensor = TransformInput(pixels);
//        var inputs = new Dictionary<string, Tensor> {
//            { INPUT_NAME, inputTensor }
//        };
//        worker.Execute(inputs);
//        var outputTensor_l = worker.PeekOutput(OUTPUT_NAME_L);
//        var outputTensor_m = worker.PeekOutput(OUTPUT_NAME_M);

//        var results_l = yolo_Utils.DecodeNNOut(outputTensor_l, MINIMUM_CONFIDENCE, params_l);
//        var results_m = yolo_Utils.DecodeNNOut(outputTensor_m, MINIMUM_CONFIDENCE, params_m);
//        var results = results_l.Concat(results_m).ToList();

//        var boxes = yolo_Utils.FilterBoundingBoxes(results, 5, MINIMUM_CONFIDENCE);
//        //Debug.Log(boxes[0].Rect);

//        List<Rect> result = DrawResults(boxes);

//        inputTensor.Dispose();
//        outputTensor_l.Dispose();
//        outputTensor_m.Dispose();

//        worker.Dispose();

//        return result;
//    }

//    private static Tensor ExtractPixels(Color32[] pic)
//    {
//        float[] pixels = new float[416 * 416 * 3];

//        for (int i = 0; i < pic.Length; ++i)
//        {
//            var color = pic[i];

//            pixels[i * 3 + 0] = (color.r ) / 255.0f;
//            pixels[i * 3 + 1] = (color.g ) / 255.0f;
//            pixels[i * 3 + 2] = (color.b ) / 255.0f;
//        }

//        return new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 3, pixels); ;
//    }

//    private static Tensor TransformInput(byte[] pixels){

//        Debug.Log(pixels.Length);
//        float[] transformedPixels = new float[pixels.Length];

//        for (int i = 0; i < pixels.Length; i++){
//            if(i % 3 == 0){
//                    var k = pixels[i];
//                    var r = pixels[i+2];
//                    pixels[i] = r;
//                    pixels[i + 2] = k;
//            }

//            transformedPixels[(pixels.Length-1) - i] = (pixels[i] - 127f) / 128f;

//        }
//        return new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 3, transformedPixels);
//    }

//    private static List<Rect> DrawResults(IEnumerable<yolo_Utils.ResultBox> results)
//    {
//        List<Rect> result_ = new List<Rect>();
//        foreach (yolo_Utils.ResultBox box in results)
//        {
//            float x,y,w,h;
//            x = box.Rect.x;
//            y = box.Rect.y;
//            w = box.Rect.width;
//            h = box.Rect.height;

//            x *= 320;
//            y *= 240;
//            w *= 320;
//            h *= 240;

//            x = x * -1 + 320 - w;
//            y = y * -1 + 240 - h;

//            if (x < 0)
//            {
//                w += x;
//                x = 0;
//            }
//            if (y < 0)
//            {
//                h += y;
//                y = 0;
//            }


//            Debug.Log(x);
//            Debug.Log(y);
//            Debug.Log(w);
//            Debug.Log(h);
//            result_.Add(new Rect(x, y, w, h));
//        }

//        return result_;
//    }
   
//}
