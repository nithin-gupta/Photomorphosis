// See https://aka.ms/new-console-template for more information
BitmapInterface.Bitmap bmp = new BitmapInterface.Bitmap(@"../img/blackbuck.bmp");
Console.WriteLine(bmp.DIBColourDepth);

var data = bmp.GetPixelData();


var s = "Hello, world!";



BitmapInterface.RGBAPixel[,] Encontrast(BitmapInterface.RGBAPixel[ , ] npixels, double contrast)
{
    var pixels = npixels;
    Clamp(contrast, -100, 100);
    contrast *= 2.55;

    var factor = (255 + contrast) / (255.01 - contrast);

    foreach(var p in pixels)
    {
        p.Red = (byte)(Clamp(factor * (p.Red - 128) + 128, 0, 255));
        p.Blue = (byte)(Clamp(factor * (p.Blue - 128) + 128, 0, 255));
        p.Green = (byte)(Clamp(factor * (p.Green - 128) + 128, 0, 255));
    }

    return pixels;
}



double Clamp(double value, double min, double max)
{
    return (value > max) ? max : ((value < min) ? min : value);
}

//bmp.SetPixelData(Encontrast(bmp.GetPixelData(), 50));

//bmp.SetPixelData(data);

//bmp.GenerateBitmap(@"../img/blackbuck_cont");

//bmp.SetPixelData(Encontrast(data, 50));

//bmp.GenerateBitmap(@"../img/conv_enc");

//bmp.SetPixelData(Encontrast(data, -50));

//bmp.GenerateBitmap(@"../img/conv_dec");

var hellobmp = new BitmapInterface.Bitmap(@"../img/img\hi_l.bmp");
Console.WriteLine(hellobmp.DIBColourDepth);

////var allpixels = data.Zip(hellobmp.GetPixelData(), (n, w)=> new {org = n, hel = w });

var hdata = hellobmp.GetPixelData();


BitmapInterface.RGBAPixel[,] Convolute(BitmapInterface.RGBAPixel[,] adata, BitmapInterface.RGBAPixel[,] bdata) {

    var odata = adata;
    
    for(int i = 0; i < adata.GetLength(0); i++)
    {
        for(int j = 0; j < adata.GetLength(1); j++)
        {
            if (bdata[i,j].Red == 0xff && bdata[i, j].Blue == 0xff && bdata[i, j].Green == 0xff)
            {
                //Console.WriteLine(bdata[i, j]);
                odata[i, j].Red = (byte) Clamp(adata[i,j].Red +5, 0, 255);
                odata[i, j].Green = (byte)Clamp(adata[i, j].Green +5, 0, 255);
                odata[i, j].Blue = (byte)Clamp(adata[i, j].Blue +5, 0, 255);
            }
        }
    }
    Console.WriteLine(adata == odata);
    return odata;
}

bmp.SetPixelData(Convolute(data, hdata));

int i = 0;

data = bmp.GetPixelData();

foreach (var p in data)
{
    if (i >= s.Length) i = 0;
    p.Red = (byte)(s[i]);
    i++;
}

bmp.SetPixelData(data);

//var vdata = bmp.GetPixelData();
bmp.GenerateBitmap(@"../img/bb_wh_whw_l");

//bmp.SetPixelData(Encontrast(bmp.GetPixelData(), 100));
//bmp.GenerateBitmap(@"../img/blackbuck_whello_r_cont");

//var ndata = vdata;

//foreach (var p in ndata)
//{
//    p.Green = 0;
//    p.Blue = 0;
//}

//bmp.SetPixelData(ndata);
//bmp.GenerateBitmap(@"../img/blackbuck_whello_r_onlyred_b");

//ndata = vdata;

//foreach (var p in ndata)
//{
//    p.Green = 255;
//    p.Blue = 255;
//}

//bmp.SetPixelData(ndata);
//bmp.GenerateBitmap(@"../img/blackbuck_whello_r_onlyred_w");
