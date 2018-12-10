const pdf = require('html-pdf');

module.exports = function (result, html, options) {
    options = {
        "format": "Letter",
        "orientation": "landscape",
        "zoomFactor": "1",
        "type": "pdf",
        "childProcessOptions": {
            "detached": true
        },
        "border": "0",      
        paginationOffset: 1,       // Override the initial pagination number
        "header": {
            "height": "20mm",
            //"contents": '<div style="text-align: center;">Author: Marc Bachmann</div>'
        },
        "footer": {
            "height": "15mm",
            //"contents": {
            //    first: 'Cover page',
            //    2: 'Second page', // Any page number is working. 1-based index
            //    default: '<span style="color: #444;">{{page}}</span>/<span>{{pages}}</span>', // fallback value
            //    last: 'Last Page'
            //}
        },
        //"paginationOffset": 1,     
        //"phantomPath": "./node_modules/phantomjs/bin/phantomjs", // PhantomJS binary which should get downloaded automatically
        //"phantomArgs": [], 
    }
    pdf.create(html, options).toStream(function (err, stream) {
        stream.pipe(result.stream);
    });
};


//module.exports = function (callback, htmlstring) {

//    var fs = require('fs'); // get the FileSystem module, provided by the node envrionment  
//    var pdf = require('html-pdf'); // get the html-pdf module which we have added in the dependency and downloaded.  
//    //var html = fs.readFileSync(inputFilePath, 'utf8'); //read the contents of the html file, from the path  
//    var options = { format: 'Letter' }; //options provided to html-pdf module. More can be explored in the module documentation  

//    //create the pdf file  
//    pdf.create(htmlstring, options).toStream(function (err, pdfStream) {
//        if (err) return callback(null, "Failed to generate PDF");

//        //pdfStream.on('end', () => {
//        //    // done reading
//        //    return res.end()
//        //})

//        // pipe the contents of the PDF directly to the response
//        //pdfStream.pipe(res)
//        callback(null, pdfStream);
//    });

//}   


//module.exports = function (result, html, options) {
//    pdf.create(html, options).toStream(function (err, stream) {
//        stream.pipe(result.stream);
//    });
//}; 

//module.exports = function (callback, num1, num2) {
//    var result = num1 + num2;
//    callback(null, result);
//};  