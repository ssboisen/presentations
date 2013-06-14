var spawn = require('child_process').spawn,
    carrier = require('carrier'),
    os = require('os'),
    platform = os.platform();

var fapreducePath = 'fapreduce/bin/Debug/FapReduce.exe';
var program = platform === 'darwin' ? spawn('mono', [fapreducePath]) : spawn(fapreducePath);

program.on('exit', function () {
	console.log("FapReduce is exiting");
});

carrier.carry(program.stdout, function (line) {
  console.log('stdout: ' + line);
});

setTimeout( function () {
	console.log("writing to stdin");
	program.stdin.write('Message My first name is Simon\r\n');
	program.stdin.write('Message My last name is Boisen\r\n');
}, 2000);

setTimeout(function () {
	console.log('sending Stop');
	program.stdin.write('Stop\r\n');
}, 4000);
