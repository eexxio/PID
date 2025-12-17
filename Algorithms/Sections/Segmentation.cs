using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace Algorithms.Sections
{
    public struct HoughLine
    {
        public double R;
        public double Theta;
        public int Votes;
    }

    public class Segmentation
    {
        public static Image<Gray, byte> HoughTransform(Image<Gray, byte> inputImage, int threshold, out int[,] accumulator, out int rMax)
        {
            Image<Gray, byte> gradient = Filters.SobelEdgeDetection(inputImage);
            Image<Gray, byte> binary = Tools.Tools.Binary(gradient, threshold);

            int width = inputImage.Width;
            int height = inputImage.Height;
            rMax = (int)Math.Ceiling(Math.Sqrt(width * width + height * height));
            int thetaSteps = 271;

            accumulator = new int[2 * rMax + 1, thetaSteps];

            double[] cosTheta = new double[thetaSteps];
            double[] sinTheta = new double[thetaSteps];
            double thetaMin = -90.0;
            double thetaStep = 270.0 / (thetaSteps - 1);

            for (int t = 0; t < thetaSteps; t++)
            {
                double angle = (thetaMin + t * thetaStep) * Math.PI / 180.0;
                cosTheta[t] = Math.Cos(angle);
                sinTheta[t] = Math.Sin(angle);
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (binary.Data[y, x, 0] == 255)
                    {
                        for (int t = 0; t < thetaSteps; t++)
                        {
                            double r = x * cosTheta[t] + y * sinTheta[t];
                            int rIdx = (int)Math.Round(r) + rMax;

                            if (rIdx >= 0 && rIdx < 2 * rMax + 1)
                            {
                                accumulator[rIdx, t]++;
                            }
                        }
                    }
                }
            }

            Image<Gray, byte> houghImage = NormalizeAccumulator(accumulator, rMax, thetaSteps);

            return houghImage;
        }

        private static Image<Gray, byte> NormalizeAccumulator(int[,] accumulator, int rMax, int thetaSteps)
        {
            int rSize = 2 * rMax + 1;
            Image<Gray, byte> result = new Image<Gray, byte>(thetaSteps, rSize);

            int maxVotes = 0;
            for (int r = 0; r < rSize; r++)
            {
                for (int t = 0; t < thetaSteps; t++)
                {
                    if (accumulator[r, t] > maxVotes)
                        maxVotes = accumulator[r, t];
                }
            }

            if (maxVotes == 0) return result;

            double logMax = Math.Log(1 + maxVotes);

            for (int r = 0; r < rSize; r++)
            {
                for (int t = 0; t < thetaSteps; t++)
                {
                    if (accumulator[r, t] > 0)
                    {
                        double normalized = Math.Log(1 + accumulator[r, t]) / logMax * 255;
                        result.Data[r, t, 0] = (byte)normalized;
                    }
                    else
                    {
                        result.Data[r, t, 0] = 0;
                    }
                }
            }

            return result;
        }

        public static List<HoughLine> DetectLines(int[,] accumulator, int rMax, int maxLines, double minVotesPercent)
        {
            int rSize = accumulator.GetLength(0);
            int thetaSteps = accumulator.GetLength(1);

            int globalMax = 0;
            for (int r = 0; r < rSize; r++)
            {
                for (int t = 0; t < thetaSteps; t++)
                {
                    if (accumulator[r, t] > globalMax)
                        globalMax = accumulator[r, t];
                }
            }

            int minVotes = (int)(globalMax * minVotesPercent / 100.0);

            List<HoughLine> lines = new List<HoughLine>();
            int windowSize = 4;

            for (int r = windowSize; r < rSize - windowSize; r++)
            {
                for (int t = windowSize; t < thetaSteps - windowSize; t++)
                {
                    int currentVotes = accumulator[r, t];

                    if (currentVotes < minVotes)
                        continue;

                    bool isLocalMax = true;
                    for (int dr = -windowSize; dr <= windowSize && isLocalMax; dr++)
                    {
                        for (int dt = -windowSize; dt <= windowSize; dt++)
                        {
                            if (dr == 0 && dt == 0) continue;

                            if (accumulator[r + dr, t + dt] > currentVotes)
                            {
                                isLocalMax = false;
                                break;
                            }
                        }
                    }

                    if (isLocalMax)
                    {
                        double rValue = r - rMax;
                        double thetaMin = -90.0;
                        double thetaStep = 270.0 / (thetaSteps - 1);
                        double thetaDegrees = thetaMin + t * thetaStep;

                        lines.Add(new HoughLine
                        {
                            R = rValue,
                            Theta = thetaDegrees,
                            Votes = currentVotes
                        });
                    }
                }
            }

            lines.Sort((a, b) => b.Votes.CompareTo(a.Votes));
            if (lines.Count > maxLines)
                lines = lines.GetRange(0, maxLines);

            return lines;
        }

    }
}
