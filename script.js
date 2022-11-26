let imgbuffer;
let rawdata;

function as() {
    let xhr = new XMLHttpRequest()
    xhr.open('GET', "http://localhost:6969/image", true);
    xhr.responseType = 'blob';
    xhr.onload = function() {
        var nbuf = xhr.response;
        rawdata = nbuf;
        var words = new Uint8Array(nbuf);
        // hex = '';
        // for (var i = 0; i < words.length; i++) {
        //     hex += words.get(i).toString(16); // this will convert it to a 4byte hex string
        // }
        drawToCanvas(rawdata);
    }
    xhr.send()
}

function drawToCanvas(buffer) {
    let canvas = document.getElementById('maincanvas');
    canvas.width = 500;
    canvas.height = 500;
    let ctx = canvas.getContext('2d');
    createImageBitmap(buffer).then(imageBitmap => {
        console.log(imageBitmap);
        imgbuffer = imageBitmap;
        ctx.drawImage(imageBitmap, 0, 0)
        setHexDump()
    })

}

function hexDump() {
    let ctx = document.getElementById('maincanvas').getContext('2d');
    let imgdata = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
    let pixels = imgdata.data;

    let hexdump = ""

    let rowSpace = 16;
    for (let i = 0; i < pixels.length; i += rowSpace) {
        hexdump += i.toString(16).padStart(8, '0');
        hexdump += "    "
        for (let j = i; j < i + rowSpace; j++) {
            hexdump += pixels[i].toString('16').padStart(2, '0').toUpperCase() + " ";
            if (j == i + rowSpace / 2) hexdump += "  "
        }
        hexdump += '    |'
        for (let j = i; j < i + rowSpace; j++) {
            hexdump += String.fromCharCode(pixels[i]);
        }
        hexdump += "|\n"
    }

    return hexdump;
}

let hexdump;

function setHexDump() {
    hexdump = hexDump()
    document.getElementById('hexdump').innerText = hexdump;
}

let xhr = new XMLHttpRequest()
xhr.open('GET', "/", true);
xhr.send();

as();