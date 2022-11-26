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
        console.log(imageBitmap);
        imgbuffer = imageBitmap;
        ctx.drawImage(imageBitmap, 0, 0)
        const x = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
        window.imgdatabuf = x;
        setHexDump()
    })

}

function hexDump() {
    //let imgdata = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
    let npixels = {...window.imgdatabuf };
    let pixels = npixels.data;

    let hexdump = ""

    let rowSpace = 16;
    for (let i = 0; i < pixels.length; i += rowSpace) {
        hexdump += i.toString(16).padStart(8, '0');
        hexdump += "    "
        for (let j = i; j < i + rowSpace; j++) {
            hexdump += pixels[j].toString('16').padStart(2, '0').toUpperCase() + " ";
            if (j == i + rowSpace / 2) hexdump += "  "
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
    return ((code >= " ".charCodeAt(0) && code <= 1 << 7) ? String.fromCharCode(code) : " ");
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

function adjustGamma(ctx, gamma) {
    let ginv = 1 / gamma;
    let nimage = getCopyData();
    console.log("nimage");
    console.log(nimage);
    let data = nimage.data;
    for (let i = 0; i < data.length; i += 4) {
        nimage.data[i] = 255 * Math.pow((data[i] / 255), ginv);
        nimage.data[i + 1] = 255 * Math.pow((data[i + 1] / 255), ginv);
        nimage.data[i + 2] = 255 * Math.pow((data[i + 2] / 255), ginv);
    }
    console.log("Isequal: " + JSON.stringify(_.difference(data, nimage.data)))
    ctx.putImageData(nimage, 0, 0)
}


let xhr = new XMLHttpRequest()
xhr.open('GET', "/", true);
xhr.send();

as();


let gamma = document.getElementById('gamma');
gamma.addEventListener('input', () => {
    document.getElementById('gammavalue').innerText = gamma.value;
    adjustGamma(ctx, parseFloat(gamma.value))
})