//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
////using Unity.Barracuda;
//using System.Linq;
//using UnityEngine.Profiling;
//using Unity.Jobs;
//using Unity.Collections;

//public class yolo_Utils
//{
//    public class DimensionsBase
//    {
//        public float X { get; set; }
//        public float Y { get; set; }
//        public float Height { get; set; }
//        public float Width { get; set; }
//    }

//    public class BoundingBoxDimensions : DimensionsBase { }

//    public class CellDimensions : DimensionsBase { }

//    public struct ResultBox
//    {
//        public BoundingBoxDimensions Dimensions { get; set; }
//        public int Label { get; set; }
//        public float Confidence { get; set; }
//        public bool Used { get; set; }
//        public Rect Rect
//        {
//            get { return new Rect(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height); }
//        }
//        public override string ToString()
//        {
//            return $"{Label}:{Confidence}, {Dimensions.X}:{Dimensions.Y} - {Dimensions.Width}:{Dimensions.Height}";
//        }
//    }

//    static float[] anchors = new float[] { 10F, 14F, 23F, 27F, 37F, 58F, 81F, 82F, 135F, 169F, 344F, 319F };

//    private const int _image_size = 416;
//    public const int BOXES_PER_CELL = 3;
//    public const int BOX_INFO_FEATURE_COUNT = 5;
//    public static int CLASS_COUNT = 1;


//    //public static List<ResultBox> DecodeNNOut(Tensor Output, float threshold, Dictionary<string, int> parameters)
//    //{
//    //    var boxes = new List<ResultBox>();

//    //    for (int cy = 0; cy < parameters["COL_COUNT"]; cy++)
//    //    {
//    //        for (int cx = 0; cx < parameters["ROW_COUNT"]; cx++)
//    //        {
//    //            for (int box = 0; box < BOXES_PER_CELL; box++)
//    //            {
//    //                var channel = (box * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT));
//    //                var bbd = ExtractBoundingBoxDimensions(Output, cx, cy, channel);
//    //                float confidence = GetConfidence(Output, cx, cy, channel);

//    //                if (confidence < threshold)
//    //                {
//    //                    continue;
//    //                }

//    //                float[] predictedClasses = ExtractClasses(Output, cx, cy, channel);
//    //                var(topResultIndex, topResultScore) = GetTopResult(predictedClasses);
//    //                var topScore = topResultScore * confidence;
//    //                Debug.Log("DEBUG: results: " + topResultIndex.ToString());

//    //                if (topScore < threshold)
//    //                {
//    //                    continue;
//    //                }

//    //                var mappedBoundingBox = MapBoundingBoxToCell(cx, cy, box, bbd, parameters);
//    //                boxes.Add(new ResultBox
//    //                {
//    //                    Dimensions = new BoundingBoxDimensions
//    //                    {
//    //                        X = (mappedBoundingBox.X - mappedBoundingBox.Width / 2)/ _image_size ,
//    //                        Y = (mappedBoundingBox.Y - mappedBoundingBox.Height / 2)/ _image_size ,
//    //                        Width = mappedBoundingBox.Width/ _image_size,
//    //                        Height = mappedBoundingBox.Height/_image_size,
//    //                    },
//    //                    Confidence = topScore,
//    //                    Label = topResultIndex,
//    //                    Used = false
//    //                });
//    //            }
//    //        }
//    //    }

//    //    return boxes;
//    //}


//    public static float Sigmoid(float value)
//    {
//        var k = (float)Math.Exp(value);

//        return k / (1.0f + k);
//    }


//    public static float[] Softmax(float[] values)
//    {
//        var maxVal = values.Max();
//        var exp = values.Select(v => Math.Exp(v - maxVal));
//        var sumExp = exp.Sum();

//        return exp.Select(v => (float)(v / sumExp)).ToArray();
//    }


//    //public static BoundingBoxDimensions ExtractBoundingBoxDimensions(Tensor modelOutput, int x, int y, int channel)
//    //{
//    //    return new BoundingBoxDimensions
//    //    {
//    //        X = modelOutput[0, x, y, channel],
//    //        Y = modelOutput[0, x, y, channel + 1],
//    //        Width = modelOutput[0, x, y, channel + 2],
//    //        Height = modelOutput[0, x, y, channel + 3]
//    //    };
//    //}


//    //public static float GetConfidence(Tensor modelOutput, int x, int y, int channel)
//    //{
//    //    //Debug.Log("ModelOutput " + modelOutput);
//    //    return Sigmoid(modelOutput[0, x, y, channel + 4]);
//    //}


//    public static CellDimensions MapBoundingBoxToCell(int x, int y, int box, BoundingBoxDimensions boxDimensions, Dictionary<string, int> parameters)
//    {
//        return new CellDimensions
//        {
//            X = ((float)y + Sigmoid(boxDimensions.X)) * parameters["CELL_WIDTH"],
//            Y = ((float)x + Sigmoid(boxDimensions.Y)) * parameters["CELL_HEIGHT"],
//            Width = (float)Math.Exp(boxDimensions.Width) * anchors[6 + box * 2],
//            Height = (float)Math.Exp(boxDimensions.Height) * anchors[6 + box * 2 + 1],
//        };
//    }


//    public static float[] ExtractClasses(Tensor modelOutput, int x, int y, int channel)
//    {
//        float[] predictedClasses = new float[CLASS_COUNT];
//        int predictedClassOffset = channel + BOX_INFO_FEATURE_COUNT;

//        for (int predictedClass = 0; predictedClass < CLASS_COUNT; predictedClass++)
//        {
//            predictedClasses[predictedClass] = modelOutput[0, x, y, predictedClass + predictedClassOffset];
//        }

//        return Softmax(predictedClasses);
//    }


//    public static ValueTuple<int, float> GetTopResult(float[] predictedClasses)
//    {
//        return predictedClasses
//            .Select((predictedClass, index) => (Index: index, Value: predictedClass))
//            .OrderByDescending(result => result.Value)
//            .First();
//    }


//    public static float IntersectionOverUnion(Rect boundingBoxA, Rect boundingBoxB)
//    {
//        var areaA = boundingBoxA.width * boundingBoxA.height;

//        if (areaA <= 0)
//            return 0;

//        var areaB = boundingBoxB.width * boundingBoxB.height;

//        if (areaB <= 0)
//            return 0;

//        var minX = Math.Max(boundingBoxA.xMin, boundingBoxB.xMin);
//        var minY = Math.Max(boundingBoxA.yMin, boundingBoxB.yMin);
//        var maxX = Math.Min(boundingBoxA.xMax, boundingBoxB.xMax);
//        var maxY = Math.Min(boundingBoxA.yMax, boundingBoxB.yMax);

//        var intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0);

//        return intersectionArea / (areaA + areaB - intersectionArea);
//    }


//    public static List<ResultBox> FilterBoundingBoxes(List<ResultBox> boxes, int limit, float threshold)
//    {
//        var activeCount = boxes.Count;
//        var isActiveBoxes = new bool[boxes.Count];

//        for (int i = 0; i < isActiveBoxes.Length; i++)
//        {
//            isActiveBoxes[i] = true;
//        }

//        var sortedBoxes = boxes.Select((b, i) => new { Box = b, Index = i })
//                .OrderByDescending(b => b.Box.Confidence)
//                .ToList();

//        var results = new List<ResultBox>();

//        for (int i = 0; i < boxes.Count; i++)
//        {
//            if (isActiveBoxes[i])
//            {
//                var boxA = sortedBoxes[i].Box;
//                results.Add(boxA);

//                if (results.Count >= limit)
//                    break;

//                for (var j = i + 1; j < boxes.Count; j++)
//                {
//                    if (isActiveBoxes[j])
//                    {
//                        var boxB = sortedBoxes[j].Box;

//                        if (IntersectionOverUnion(boxA.Rect, boxB.Rect) > threshold)
//                        {
//                            isActiveBoxes[j] = false;
//                            activeCount--;

//                            if (activeCount <= 0)
//                                break;
//                        }
//                    }
//                }

//                if (activeCount <= 0)
//                    break;
//            }
//        }
//        return results;
//    }
//}
