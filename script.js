let imgbuffer;
let rawdata;
let original;
let canvas = document.getElementById('maincanvas');
let ctx = canvas.getContext('2d', {
    willReadFrequently: true
});

function as() {
    let xhr = new XMLHttpRequest()
    xhr.open('GET', "http://localhost:6969/image", true);
    xhr.responseType = 'blob';
    xhr.onload = function() {
        var nbuf = xhr.response;
        rawdata = nbuf;
        // hex = '';
        // for (var i = 0; i < words.length; i++) {
        //     hex += words.get(i).toString(16); // this will convert it to a 4byte hex string
        // }
        drawToCanvas(rawdata);
    }
    xhr.send()
}

function drawToCanvas(buffer) {
    createImageBitmap(buffer).then(imageBitmap => {
        imgbuffer = imageBitmap;
        ctx.drawImage(imageBitmap, 0, 0)
        const x = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
        window.imgdatabuf = x;
        //setHexDump()
    })

}

function hexDump(org = false) {
    //let imgdata = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
    let npixels = org ? {...window.imgdatabuf } : ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
    let pixels = npixels.data;

    let hexdump = ""

    let rowSpace = 16;
    for (let i = 0; i < pixels.length; i += rowSpace) {
        hexdump += i.toString(16).padStart(8, '0');
        hexdump += "    "
        for (let j = i; j < i + rowSpace; j++) {
            hexdump += pixels[j].toString('16').padStart(2, '0').toUpperCase() + " ";
            if (j == i + rowSpace / 2 - 1) hexdump += "  "
        }
        hexdump += '    |'
        for (let j = i; j < i + rowSpace; j++) {
            hexdump += ASCIICharFrom(pixels[j]);
        }
        hexdump += "|\n"
    }

    return hexdump;
}

function ASCIICharFrom(code) {
    return ((code >= " ".charCodeAt(0) && code <= 1 << 7) ? String.fromCharCode(code) : ".");
}

function Lerp(min, max, value) {
    return (1.0 - value) * min + value * max;
}

function InverseLerp(min, max, value) {
    return (value - min) / (max - value);
}

function getCopyData() {
    let x = new ImageData(window.imgdatabuf.width, window.imgdatabuf.width);
    for (let i = 0; i < x.data.length; ++i) x.data[i] = window.imgdatabuf.data[i];
    return x;
}

let hexdump;

function setHexDump() {
    hexdump = hexDump()
    document.getElementById('hexdump').innerText = hexdump;
}

function adjustGamma(nimage, gamma) {
    let ginv = 1 / gamma;
    // let nimage = getCopyData();
    let data = nimage.data;
    for (let i = 0; i < data.length; i += 4) {
        nimage.data[i] = 255 * Math.pow((data[i] / 255), ginv);
        nimage.data[i + 1] = 255 * Math.pow((data[i + 1] / 255), ginv);
        nimage.data[i + 2] = 255 * Math.pow((data[i + 2] / 255), ginv);
    }
    //return nimage;
    //ctx.putImageData(nimage, 0, 0)
}

function adjustBrightness(nimage, brightness) {
    let data = nimage.data;
    for (let i = 0; i < data.length; i += 4) {
        nimage.data[i] = (nimage.data[i] * brightness);
        nimage.data[i + 1] = (nimage.data[i + 1] * brightness);
        nimage.data[i + 2] = (nimage.data[i + 2] * brightness);
    }
    //return nimage;
}

function Clamp(value, min, max) {
    return (value > max) ? max : ((value < min) ? min : value);
}

function adjustContrast(nimage, contrast) {
    contrast = Clamp(contrast, -1, 1)
    let ncontrast = Lerp(0, 255, contrast);
    let cfactor = (255 + ncontrast) / (255.01 - ncontrast)

    for (let i = 0; i < nimage.data.length; i += 4) {
        nimage.data[i] = (cfactor * (nimage.data[i] - 128) + 128);
        nimage.data[i + 1] = (cfactor * (nimage.data[i + 1] - 128) + 128);
        nimage.data[i + 2] = (cfactor * (nimage.data[i + 2] - 128) + 128);
    }
}

function ImageFilter(ctx) {
    let nimage = getCopyData();

    adjustGamma(nimage, parseFloat(gamma.value))
    adjustBrightness(nimage, parseFloat(brightness.value /*InverseLerp(0, 200, parseFloat(brightness.value) + 100*/ ))
    adjustContrast(nimage, parseFloat(contrast.value))

    //TODO: Keep inversion while changing slider values
    if (isInvert) InvertImage(nimage);
    ctx.putImageData(nimage, 0, 0)
}

let isInvert = false;

let OnesComplement = (n) => 255 - n;

function InvertImage(nimage) {
    for (let i = 0; i < nimage.data.length; i += 4) {
        nimage.data[i] = OnesComplement(nimage.data[i]);
        nimage.data[i + 1] = OnesComplement(nimage.data[i + 1]);
        nimage.data[i + 2] = OnesComplement(nimage.data[i + 2]);
    }
    isInvert = !isInvert;
    return nimage;
}

let xhr = new XMLHttpRequest()
xhr.open('GET', "/", true);
xhr.send();

as();


let gamma = document.getElementById('gamma');
let brightness = document.getElementById('brightness');
let contrast = document.getElementById('contrast');
gamma.addEventListener('input', () => {
    ImageFilter(ctx)
    document.getElementById('gammavalue').innerText = gamma.value;
    // adjustGamma(ctx, parseFloat(gamma.value))
})

brightness.addEventListener('input', () => {
    ImageFilter(ctx)
    document.getElementById('brightnessvalue').innerText = ((parseFloat(brightness.value) - 1) * 100).toFixed();
})

contrast.addEventListener('input', () => {
    ImageFilter(ctx)
    document.getElementById('contrastvalue').innerText = (parseFloat(contrast.value) * 100).toFixed();
})

document.getElementById('hexdumpbut').addEventListener('click', () => {
    setHexDump(true)
})

document.getElementById('invertbut').addEventListener('click', () => {
    let nimage = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
    InvertImage(nimage);
    ctx.putImageData(nimage, 0, 0);
})