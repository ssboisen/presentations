var cluster = require('cluster');
var http = require('http');
// Workers can share any TCP connection
// In this case its a HTTP server
http.createServer(function(req, res) {
  res.writeHead(200);
  res.end("hello from worker " + cluster.worker.process.pid + " \n");
}).listen(8000);
