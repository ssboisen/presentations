var cluster = require('cluster');
var numCPUs = require('os').cpus().length;

cluster.setupMaster({
  exec: "worker.js"
});

cluster.on('online', function(worker) {
  console.log('Worker',worker.process.pid, "is online");
});

cluster.on('exit', function(worker, code, signal) {
  console.log('worker ' + worker.process.pid + ' died');
});

for (var i = 0; i < numCPUs; i++) {
  cluster.fork();
}
