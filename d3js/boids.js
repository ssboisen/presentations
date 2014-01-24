function pos(n) { return function (b) { return b.position[n]; } };
function s(n) { return function (d) { return d[n]; }; };
function div(n, d) { return n / d };
var c = d3.scale.category20();

function makeBoids(translation) {
  var boids = d3.range(30).map(function (i) {
    return  {
      key : i,
      position : [translation + Math.random() * -20 * i, Math.random() * -20 * i],
      velocity: [Math.random() * 12 + 1, Math.random() * 12 + 1]
    };
  });

  return boids;
};

var vector = {
  add: function (v1) {
    var args = Array.prototype.slice.call(arguments, 1);
    return args.reduce(function (a, c) { return [a[0] + c[0], a[1] + c[1]]; }, v1);
  },
  subtract: function (v1) {
    var args = Array.prototype.slice.call(arguments, 1);
    return args.reduce(function (a, c) { return [a[0] - c[0], a[1] - c[1]]; }, v1);
  },
  min: function (v, max) { return [Math.min(v[0], max), Math.min(v[1], max)]; },

  divide: function (v, s) { return [v[0] / s, v[1] / s]; },

  magnitude: function (v) { return Math.sqrt(Math.pow(v[0], 2) + Math.pow(v[1], 2)); },

  multiply: function (v, s) { return [v[0] * s, v[1] * s]; },

  distance: function(v1, v2) {
    return Math.sqrt(Math.pow(v1[0] - v2[0], 2) + Math.pow(v1[1] - v2[1], 2));
  }
};


var width = d3.select("svg").style('width'),
      height = d3.select("svg").style('height'),
      width = +width.substring(0,width.length - 2),
      height = +height.substring(0,height.length - 2);

var place = [width/2,height/2],
    follow = false;
d3.select("svg")
  .on("dblclick", function () {
    follow = !follow; })
  .on("mousemove", function(d) {
    place = d3.mouse(this); });

function addBoids(boids) {
  var svg = d3.select("svg")
  .append('g')
  .selectAll('circle')
  .data(boids);

  svg .enter()
  .append("circle")
  .on("mouseenter", function (d) {
    d3.select(this).transition().attr('r', 25);
    d.slowdown = true;
  })
  .on("mouseout", function (d) {
    d3.select(this).transition().attr('r', 5);
    d.slowdown = false;
  })
  .on("click", function () {
    d3.select(this).transition()
    .duration(2500)
    .attr('r',0).style('fill','red').remove();
    follow = false;
  } )
  .attr('r', 5)
  .attr('cx', pos(0))
  .attr('cy', pos(1))
  .style('fill', function (d, i) { return c(i); })
  .style('stroke', 'black')
  .style('stroke-width', 3);

  return { boids: boids, svg: svg };
};

var boidgrps = [];

boidgrps.push(addBoids(makeBoids(0)));
boidgrps.push(addBoids(makeBoids(1000)));

var allboids = [].concat.apply([], boidgrps.map(s('boids')));

function move(boids) {
  boids.forEach(function (boid){

    var f1 = 1,
        f2 = 1,
        f3 = 1,
        f4 = 1,
        fb = 1;

    var v1 = vector.multiply(flockCenter(boid, boids), f1);
    var v2 = vector.multiply(distancing(boid, 13, allboids),f2);
    var v3 = vector.multiply(matchVelocity(boid, boids),f3);
    var v4 = follow ? vector.multiply(tendToPlace(boid, 1), f4) : [0,0];
    var vb = vector.multiply(boundary(boid, 100, 5), fb);


    boid.velocity = vector.add(boid.velocity, v1, v2, v3, v4, vb);

    limitVelocity(boid, 5);

    if(boid.slowdown)
      boid.velocity = vector.multiply(boid.velocity,0);
    boid.position = vector.add(boid.position, boid.velocity);

  });

};

function flockCenter(boid, boids) {
  var cx = d3.mean(boids.filter(function (b) { return b.key !== boid.key; }), pos(0));
  var cy = d3.mean(boids.filter(function (b) { return b.key != boid.key; }), pos(1));
  var divisor = 5000;

  return [div(cx - boid.position[0], divisor), div(cy - boid.position[1], divisor)];
};

function distancing(boid, mindistance, boids) {
  var c = [0,0];

  for(var i = 0; i < boids.length; i++) {
    var b = boids[i];

    if(boid.key != b.key) {
      var d = vector.magnitude(vector.subtract(b.position, boid.position));


      if(d < mindistance) {
        c = vector.subtract(c, vector.subtract(b.position, boid.position));
      }
    }
  }

  return c;
};

function matchVelocity (boid, boids) {
  var pv = [0,0];

  for(var i = 0; i < boids.length; i++) {
    var b = boids[i];

    if(boid.key != b.key) {
      pv = vector.add(pv, b.velocity);
    }
  }
  pv = vector.divide(pv, boids.length - 1);

  return vector.divide(vector.subtract(pv, boid.velocity), 8);
};

function boundary(boid, margin, bounce) {

  var x = boid.position[0], y = boid.position[1];

  var v = [0,0];
  if (x < 0 - margin)
    v[0] = bounce;
  else if (x > width + margin)
    v[0] = -1 * bounce;

  if(y < 0 - margin)
    v[1] = bounce;
  else if (y > height + margin)
    v[1] = -1 * bounce;


  return v;
};

function limitVelocity(boid, vlim) {

  var magnitude = vector.magnitude(boid.velocity);

  if(magnitude > vlim) {
    boid.velocity = vector.multiply(vector.divide(boid.velocity, magnitude), vlim);
  }
}

function tendToPlace(boid, pctmovepertime) {
  return vector.multiply(vector.subtract(place, boid.position), pctmovepertime * (1/100));
};

requestAnimationFrame(function () {
  boidgrps.forEach(function (boidgrp, i) {
    move(boidgrp.boids);
    boidgrp.svg
    .data(boidgrp.boids)
    .attr('cx', pos(0))
    .attr('cy', pos(1));
  });

});