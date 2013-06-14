var app = require('http').createServer(handler)
, io = require('socket.io').listen(app)
, fs = require('fs');

app.listen(3000);

function handler (req, res) {
  fs.readFile(__dirname + '/index.html', function (err, data) {
    if (err) {
      res.writeHead(500);
      return res.end('Error loading index.html');
    }

    res.writeHead(200);
    res.end(data);
  });
}

var playerQueue = [];

var activeGame = false;
io.set('log level', 1);

io.sockets.on('connection', function (socket) {
  socket.on('join-game', function() {
    if(!activeGame) {
      if(playerQueue.length > 0) {
        var opponent = playerQueue.shift();

        socket.join('active-game');
        opponent.join('active-game');
        socket.emit('game-ready', 'upper');
        opponent.emit('game-ready', 'lower');

        activeGame = true;

        var ballPosition = { x: 150, y: 150 };
        socket.on('new-ball-position', function(pos) {
          ballPosition = pos;
        });

        var score = { lowerScore: 0, upperScore: 0 };
        socket.on('new-score', function(s) {
          score = s;
        });

        setInterval(function() {
          var gameState = { 
            "upperPosition": socket.position, 
            "lowerPosition": opponent.position, 
            "ballPosition": ballPosition, 
            "score": score
          };
          io.sockets.in('active-game').emit('tick', gameState);
        }, 40);

        console.log("started game");
      }
      else {
        playerQueue.push(socket);
      }
    }
    else {
      socket.join('active-game');
      socket.emit('game-ready','spectator');
    }
  });

  socket.on('new-position', function(position) {
    socket.position = position;
  });
});
