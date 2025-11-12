using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static byte Median(int[] H, int l)
        {
            int k = 0;
            int s = 0;
            int n = (l * l) / 2;

            while (k <= 255 && s + H[k] <= n)
            {
                s = s + H[k];
                k = k + 1;
            }

            return (byte)k;
        }

        public static Image<Gray, byte> MedianFiltering(Image<Gray, byte> I, int l)
        {
            if (l % 2 == 0)
            {
                l = l + 1;
            }

            Image<Gray, byte> result = new Image<Gray, byte>(I.Width, I.Height);
            int[] H = new int[256];
            int r = l / 2;

            for (int y = r; y < I.Height - r; y++)
            {
                for (int x = r; x < I.Width - r; x++)
                {
                    if (x == r)
                    {
                        for (int i = 0; i < 256; i++)
                        {
                            H[i] = 0;
                        }

                        for (int i = -r; i <= r; i++)
                        {
                            for (int j = -r; j <= r; j++)
                            {
                                H[I.Data[y + i, x + j, 0]]++;
                            }
                        }
                    }
                    else
                    {
                        for (int k = -r; k <= r; k++)
                        {
                            H[I.Data[y + k, x - r - 1, 0]]--;
                            H[I.Data[y + k, x + r, 0]]++;
                        }
                    }

                    result.Data[y, x, 0] = Median(H, l);
                }
            }

            return result;
        }

        public static Image<Gray, byte> SobelDirectionalDetection(Image<Gray, byte> I, int threshold, double targetAngle, double tolerance)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(I.Width, I.Height);

            int[,] Sx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] Sy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            double targetAngleRad = targetAngle * System.Math.PI / 180.0;
            double toleranceRad = tolerance * System.Math.PI / 180.0;

            for (int y = 1; y < I.Height - 1; y++)
            {
                for (int x = 1; x < I.Width - 1; x++)
                {
                    int fx = 0;
                    int fy = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            fx += I.Data[y + i, x + j, 0] * Sx[i + 1, j + 1];
                            fy += I.Data[y + i, x + j, 0] * Sy[i + 1, j + 1];
                        }
                    }

                    double magnitude = System.Math.Sqrt(fx * fx + fy * fy);

                    if (magnitude > threshold)
                    {
                        double angle = System.Math.Atan2(fy, fx);

                        double angleDiff = System.Math.Abs(angle - targetAngleRad);
                        if (angleDiff > System.Math.PI)
                        {
                            angleDiff = 2 * System.Math.PI - angleDiff;
                        }

                        if (angleDiff <= toleranceRad)
                        {
                            result.Data[y, x, 0] = 255;
                        }
                        else
                        {
                            result.Data[y, x, 0] = 0;
                        }
                    }
                    else
                    {
                        result.Data[y, x, 0] = 0;
                    }
                }
            }

            return result;
        }
    }
}