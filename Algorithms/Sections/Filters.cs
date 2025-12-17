using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<Gray, byte> PadImage(Image<Gray, byte> I, int padding)
        {
            Image<Gray, byte> padded = new Image<Gray, byte>(I.Width + 2 * padding, I.Height + 2 * padding);

            for (int y = 0; y < I.Height; y++)
            {
                for (int x = 0; x < I.Width; x++)
                {
                    padded.Data[y + padding, x + padding, 0] = I.Data[y, x, 0];
                }
            }

            for (int y = 0; y < padding; y++)
            {
                for (int x = 0; x < I.Width; x++)
                {
                    padded.Data[y, x + padding, 0] = I.Data[0, x, 0];
                    padded.Data[I.Height + padding + y, x + padding, 0] = I.Data[I.Height - 1, x, 0];
                }
            }

            for (int y = 0; y < I.Height; y++)
            {
                for (int x = 0; x < padding; x++)
                {
                    padded.Data[y + padding, x, 0] = I.Data[y, 0, 0];
                    padded.Data[y + padding, I.Width + padding + x, 0] = I.Data[y, I.Width - 1, 0];
                }
            }

            for (int y = 0; y < padding; y++)
            {
                for (int x = 0; x < padding; x++)
                {
                    padded.Data[y, x, 0] = I.Data[0, 0, 0];
                    padded.Data[y, I.Width + padding + x, 0] = I.Data[0, I.Width - 1, 0];
                    padded.Data[I.Height + padding + y, x, 0] = I.Data[I.Height - 1, 0, 0];
                    padded.Data[I.Height + padding + y, I.Width + padding + x, 0] = I.Data[I.Height - 1, I.Width - 1, 0];
                }
            }

            return padded;
        }

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

            int r = l / 2;

            Image<Gray, byte> padded = PadImage(I, r);

            Image<Gray, byte> result = new Image<Gray, byte>(I.Width, I.Height);
            int[] H = new int[256];

            for (int y = 0; y < I.Height; y++)
            {
                for (int x = 0; x < I.Width; x++)
                {
                    if (x == 0)
                    {
                        for (int i = 0; i < 256; i++)
                        {
                            H[i] = 0;
                        }

                        for (int i = 0; i <= 2 * r; i++)
                        {
                            for (int j = 0; j <= 2 * r; j++)
                            {
                                H[padded.Data[y + i, x + j, 0]]++;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k <= 2 * r; k++)
                        {
                            H[padded.Data[y + k, x - 1, 0]]--;
                            H[padded.Data[y + k, x + 2 * r, 0]]++;
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

        public static Image<Gray, byte> SobelEdgeDetection(Image<Gray, byte> I)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(I.Width, I.Height);

            int[,] Sx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] Sy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

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
                    result.Data[y, x, 0] = (byte)System.Math.Min(255, magnitude);
                }
            }

            return result;
        }
    }
}