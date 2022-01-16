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
            var info = new SKImageInfo(144, 144, SKColorType.Rgba8888, SKAlphaType.Premul);
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
                    canvas.DrawBitmap(icon, new SKRect(14, 14, 34, 34));
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
                42,
                30,
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
                            52,
                            82,
                            new SKPaint
                            {
                                IsAntialias = true,
                                Style = SKPaintStyle.Fill,
                                Color = SKColors.White,
                                TextSize = 25
                            });
        }

        private void drawPositiveArrow(SKCanvas canvas)
        {
            canvas.DrawLine(
                            new SKPoint(17, 79),
                            new SKPoint(37, 59),
                            new SKPaint
                            {
                                IsAntialias = true,
                                Style = SKPaintStyle.Fill,
                                Color = SKColors.Green,
                                StrokeWidth = 4
                            });

            canvas.DrawLine(
                new SKPoint(27, 60),
                new SKPoint(38, 60),
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Green,
                    StrokeWidth = 4
                });

            canvas.DrawLine(
                new SKPoint(36, 59),
                new SKPoint(36, 69),
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
                            new SKPoint(17, 59),
                            new SKPoint(37, 79),
                            new SKPaint
                            {
                                IsAntialias = true,
                                Style = SKPaintStyle.Fill,
                                Color = SKColors.Red,
                                StrokeWidth = 4
                            });

            canvas.DrawLine(
                new SKPoint(27, 78),
                new SKPoint(38, 78),
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Red,
                    StrokeWidth = 4
                });

            canvas.DrawLine(
                new SKPoint(36, 69),
                new SKPoint(36, 79),
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
                           29,
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
                new SKPoint(22, 124),
                new SKPoint(122, 124),
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
                new SKPoint(22, 124),
                new SKPoint(Convert.ToSingle(val) + 22, 124),
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
              29,
              16,
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
              134,
              16,
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
