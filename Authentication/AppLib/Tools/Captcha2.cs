namespace Authentication.AppLib.Tools
{
    using Authenticate.AppLib.Concrete;
    using Authentication.AppLib.StartupExt;
    using Authentication.Models;
    using Microsoft.AspNetCore.Http;
    using SixLabors.Fonts;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Drawing.Processing;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Formats.Png;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.IO;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public static class Captcha2
    {
        public static IImageEncoder GetEncoder(EncoderTypes encoderType)
        {
            IImageEncoder encoder;
            switch (encoderType)
            {
                case EncoderTypes.Png:
                    encoder = new PngEncoder();
                    break;
                case EncoderTypes.Jpeg:
                    encoder = new JpegEncoder();
                    break;
                default:
                    throw new ArgumentException($"Encoder '{ encoderType }' not found!");
            };
            return encoder;
        }

        public enum EncoderTypes
        {
            Jpeg,
            Png,
        }

        public static Color[] TextColor { get; set; } = new Color[] { Color.Blue, Color.Black, Color.Black, Color.Brown, Color.Gray, Color.Green };

        public static Color[] NoiseRateColor { get; set; } = new Color[] { Color.Gray };

        private static CaptchaResult GenerateCaptchaImage(string captchaCode, int width = 100, int height = 36)
        {
            AffineTransformBuilder getRotation(int w, int h)
            {
                Random random = new Random();
                var builder = new AffineTransformBuilder();
                var width = random.Next(10, w);
                var height = random.Next(10, h);
                var pointF = new PointF(width, height);
                var rotationDegrees = random.Next(0, 5);
                var result = builder.PrependRotationDegrees(rotationDegrees, pointF);
                return result;
            }

            float GenerateNextFloat(double min = -3.40282347E+38, double max = 3.40282347E+38)
            {
                Random random = new Random();
                double range = max - min;
                double sample = random.NextDouble();
                double scaled = (sample * range) + min;
                float result = (float)scaled;
                return result;
            }

            byte[] result;

            using (var imgText = new Image<Rgba32>(width, height))
            {
                float position = 0;
                Random random = new Random();
                byte startWith = (byte)random.Next(5, 10);
                imgText.Mutate(ctx => ctx.BackgroundColor(Color.Transparent));

                string fontName = "Arial";
                Font font = SystemFonts.CreateFont(fontName, 29, FontStyle.Regular);

                foreach (char c in captchaCode)
                {
                    var location = new PointF(startWith + position, random.Next(6, 13));
                    imgText.Mutate(ctx => ctx.DrawText(c.ToString(), font, TextColor[random.Next(0, TextColor.Length)], location));
                    position += TextMeasurer.Measure(c.ToString(), new RendererOptions(font, location)).Width;
                }

                //add rotation
                AffineTransformBuilder rotation = getRotation(width, height);
                imgText.Mutate(ctx => ctx.Transform(rotation));

                // add the dynamic image to original image
                ushort size = (ushort)TextMeasurer.Measure(captchaCode, new RendererOptions(font)).Width;
                var img = new Image<Rgba32>(size + 10 + 5, height);
                img.Mutate(ctx => ctx.BackgroundColor(Color.White));


                Parallel.For(0, 5, i =>
                {
                    int x0 = random.Next(0, random.Next(0, 30));
                    int y0 = random.Next(10, img.Height);
                    int x1 = random.Next(70, img.Width);
                    int y1 = random.Next(0, img.Height);
                    img.Mutate(ctx =>
                            ctx.DrawLines(TextColor[random.Next(0, TextColor.Length)],
                                          GenerateNextFloat(0.7f, 2.0f),
                                          new PointF[] { new PointF(x0, y0), new PointF(x1, y1) })
                            );
                });

                img.Mutate(ctx => ctx.DrawImage(imgText, 0.80f));

                Parallel.For(0, 800, i =>
                {
                    int x0 = random.Next(0, img.Width);
                    int y0 = random.Next(0, img.Height);
                    img.Mutate(
                                ctx => ctx
                                    .DrawLines(NoiseRateColor[random.Next(0, NoiseRateColor.Length)],
                                    GenerateNextFloat(0.5, 1.5), new PointF[] { new Vector2(x0, y0), new Vector2(x0, y0) })
                            );
                });

                img.Mutate(x =>
                {
                    x.Resize(width, height);
                });

                using (var ms = new MemoryStream())
                {
                    img.Save(ms, GetEncoder(EncoderTypes.Png));
                    result = ms.ToArray();
                }
            }

            return new CaptchaResult { CaptchaCode = captchaCode, CaptchaByteData = result, Timestamp = DateTime.Now };

        }


        // INFO Capctha letters:
        private const string Letters = "0123456789";

        private static string GenerateCaptchaCode(int characterCount = 4)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < characterCount; i++)
            {
                int index = RandomGenerator.Next(0, Letters.Length - 1);
                sb.Append(Letters[index]);
            }

            return sb.ToString();
        }

        public static bool ValidateCaptchaCode(string userInputCaptcha, HttpContext context)
        {
            var isValid = (userInputCaptcha == context.Session.GetKey<string>(Constants.SessionKeyCaptcha));
            context.Session.RemoveKey(Constants.SessionKeyCaptcha);
            return isValid;
        }

        public static CaptchaResult GenerateCaptchaImage(HttpContext context)
        {
            CaptchaResult cr = GenerateCaptchaImage(captchaCode: GenerateCaptchaCode());
            context.Session.SetKey<string>(Constants.SessionKeyCaptcha, cr.CaptchaCode);
            return cr;
        }


    }

    
}
