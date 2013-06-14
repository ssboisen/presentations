var ytdl = require('ytdl')
  , express = require('express')
  , spawn = require('child_process').spawn
  , app = express();

var counter = 0;

app.get('/', function (req, res) {

    if(!req.query.yt) {
        res.contentType('text');
        return res.end('Usage: /?yt=:yturl');
    }

    ytdl.getInfo(req.query.yt, function(err, info) {
       
        if(err) { return console.error(err); }

        var ip = req.connection.remoteAddress;
        console.log('Streaming', info.title, "to", ip);
        console.log('Stream #', ++counter);
      
        res.contentType('mp3');

        var ytStream = ytdl(req.query.yt, { quality: 'lowest' } );

        var ffmpegProcess = spawn('ffmpeg', ['-i', '-', '-vn', '-acodec', 'libmp3lame', '-f','mp3','-']);

        req.connection.addListener('close', function() {
          console.log("closed connection to", ip, " - killing ffmpeg with id#",  ffmpegProcess.pid);
          ffmpegProcess.kill('SIGHUP');
        });

        ytStream.pipe(ffmpegProcess.stdin);

        ffmpegProcess.stdout.pipe(res); 
    });
});

app.listen(3000);
