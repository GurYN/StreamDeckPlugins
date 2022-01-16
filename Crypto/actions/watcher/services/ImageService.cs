using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Crypto.Actions.Watcher.Models;
using SkiaSharp;

namespace Crypto.actions.watcher.services
{
    public class ImageService
    {
        private CoinModel Coin;
        private string Currency;

        public ImageService(CoinModel coin, string currency)
        {
            this.Coin = coin;
            this.Currency = currency;
        }

        public async Task<string> GetImage()
        {
            var info = new SKImageInfo(140, 140, SKColorType.Rgba8888, SKAlphaType.Premul);
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            canvas.DrawColor(SKColor.Parse("#222222"));

            await drawIcon(canvas);
            drawCointName(canvas);
            drawValue(canvas);

            if (this.Coin.MarketData.PriceChangePercentage24H >= 0)
            {
                drawPositiveArrow(canvas);
            }
            else
            {
                drawNegativeArrow(canvas);
            }

            drawPercentage(canvas);

            drawCandle(canvas);

            using var img = new FileStream(string.Format("images/{0}.png", this.Coin.Id), FileMode.Create, FileAccess.Write);
            using var image = surface.Snapshot();
            using var data = image.Encode();

            img.Write(data.ToArray());

            return img.Name;
        }

        private async Task drawIcon(SKCanvas canvas)
        {
            try
            {
                var httpClient = new HttpClient();

                using (Stream stream = await httpClient.GetStreamAsync(this.Coin.Image.Thumb))
                using (MemoryStream memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);

                    var icon = SKBitmap.Decode(memStream);
                    canvas.DrawBitmap(icon, new SKRect(12, 12, 32, 32));
                };
            }
            catch
            {
            }
        }

        private void drawCointName(SKCanvas canvas)
        {
            canvas.DrawText(
                this.Coin.Symbol.ToUpper(),
                40,
                28,
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.White,
                    TextSize = 18
                }); ;
        }

        private void drawValue(SKCanvas canvas)
        {
            var val = this.Coin.MarketData.CurrentPrice[this.Currency];
            var format = "#####";
            if (val < 10000)
            {
                if (val < 10) { format = "0.0000"; }
                else if (val < 100) { format = "00.000"; }
                else if (val < 1000) { format = "000.00"; }
                else { format = "0000.0"; }
            }
            
            canvas.DrawText(
                            val.ToString(format),
                            55,
                            80,
                            new SKPaint
                            {
                                IsAntialias = true,
                                Style = SKPaintStyle.Fill,
                                Color = SKColors.White,
                                TextSize = 24
                            });
        }

        private void drawPositiveArrow(SKCanvas canvas)
        {
            canvas.DrawLine(
                            new SKPoint(16, 75),
                            new SKPoint(36, 55),
                            new SKPaint
                            {
                                IsAntialias = true,
                                Style = SKPaintStyle.Fill,
                                Color = SKColors.Green,
                                StrokeWidth = 4
                            });

            canvas.DrawLine(
                new SKPoint(26, 56),
                new SKPoint(37, 56),
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Green,
                    StrokeWidth = 4
                });

            canvas.DrawLine(
                new SKPoint(35, 55),
                new SKPoint(35, 65),
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Green,
                    StrokeWidth = 4
                });
        }

        private void drawNegativeArrow(SKCanvas canvas)
        {
            canvas.DrawLine(
                            new SKPoint(16, 55),
                            new SKPoint(36, 75),
                            new SKPaint
                            {
                                IsAntialias = true,
                                Style = SKPaintStyle.Fill,
                                Color = SKColors.Red,
                                StrokeWidth = 4
                            });

            canvas.DrawLine(
                new SKPoint(26, 74),
                new SKPoint(37, 74),
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Red,
                    StrokeWidth = 4
                });

            canvas.DrawLine(
                new SKPoint(35, 65),
                new SKPoint(35, 75),
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Red,
                    StrokeWidth = 4
                });
        }

        private void drawPercentage(SKCanvas canvas)
        {
            canvas.DrawText(
                           this.Coin.MarketData.PriceChangePercentage24H.ToString("##0.0")+"%",
                           28,
                           95,
                           new SKPaint
                           {
                               IsAntialias = true,
                               Style = SKPaintStyle.Fill,
                               Color = SKColors.White,
                               TextSize = 12,
                               TextAlign = SKTextAlign.Center,
                           });
        }

        private void drawCandle(SKCanvas canvas)
        {
            canvas.DrawLine(
                new SKPoint(20, 122),
                new SKPoint(120, 122),
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Gray,
                    StrokeWidth = 15
                });

            var range = Coin.MarketData.High24H[Currency] - Coin.MarketData.Low24H[Currency];
            var current = Coin.MarketData.CurrentPrice[Currency] - Coin.MarketData.Low24H[Currency];
            var val = 100 / range * current;

            canvas.DrawLine(
                new SKPoint(20, 122),
                new SKPoint(Convert.ToSingle(val) + 20, 122),
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Green,
                    StrokeWidth = 15
                });

            canvas.Save();

            canvas.RotateDegrees(-90, 72, 72);

            canvas.DrawText(
              "LOW",
              31,
              12,
              new SKPaint
              {
                  IsAntialias = true,
                  Style = SKPaintStyle.Fill,
                  Color = SKColors.White,
                  TextSize = 8,
                  TextAlign = SKTextAlign.Right,
              });

            canvas.Restore();

            canvas.RotateDegrees(90, 72, 72);

            canvas.DrawText(
              "HIGH",
              132,
              17,
              new SKPaint
              {
                  IsAntialias = true,
                  Style = SKPaintStyle.Fill,
                  Color = SKColors.White,
                  TextSize = 8,
                  TextAlign = SKTextAlign.Right,
              });

            canvas.Restore();
        }
    }
}
