const express = require('express');
const path = require('path');
const bodyParser = require('body-parser');
const fs = require('fs');

const app = express()
app.use(express.static(__dirname));

app.get('/image', (req, res) => {
    fs.readFile('./img/bb_wh_whw_l.bmp', (err, data) => {
            if (err) throw err;
            var buffer = Buffer.from(data);

            res.send(buffer);
        })
        //res.sendFile(__dirname + '/blackbuck.bmp')
})

app.listen(6969)