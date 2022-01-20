const page = require('webpage').create();
const system = require('system');

page.onLoadFinished = function () {
    // wait some time for the comments to load
    setTimeout(function () {
        console.log(page.content);
        phantom.exit();
    }, 1000);
};

page.open(system.args[1]);
