using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        public static int OtsuThreshold(Image<Gray, byte> inputImage)
        {
            var histogram = Utils.ComputeNormalizedHistogram(inputImage); ;

            double n = inputImage.Width * inputImage.Height;
            var p = new double[256];
            for (int k = 0; k <= 255; k++)
            {
                p[k] = (double)(histogram[k] / (double)n);
            }
            var threshold = 0;
            double maxInterSigma = 0;
            for (int t = 1; t <= 254; t++)
            {
                double p1, p2, mu1, mu2;
                p1 = p2 = mu1 = mu2 = 0;
                for (int i = 0; i <= t; i++)
                {
                    p1 += p[i];
                    mu1 += i * p[i];
                }
                for (int i = t + 1; i <= 255; i++)
                {
                    p2 += p[i];
                    mu2 += i * p[i];
                }

                if (p1 > 0 && p2 > 0)
                {
                    mu1 /= p1;
                    mu2 /= p2;
                    double interSigma = p1 * p2 * (mu1 - mu2) * (mu1 - mu2);
                    if (interSigma > maxInterSigma)
                    {
                        maxInterSigma = interSigma;
                        threshold = t;
                    }
                }
            }
            return threshold;
        }
    }
}