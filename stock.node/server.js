var express = require('express');
var bodyParser = require('body-parser')
var util = require('util');
var fs = require('fs');

// create application/json parser
var jsonParser = bodyParser.json()

// create application/x-www-form-urlencoded parser
var urlencodedParser = bodyParser.urlencoded({ extended: false })

var app = express();

app.post('/', urlencodedParser, function (req, res) {
  if (!req.body) 
	  return res.sendStatus(400);
  
	console.log(req.body.date + ' ' + req.body.price);
	var newDate = new Date(Number(req.body.date));
	fs.appendFile('out.csv', newDate.getFullYear() + '/' + (newDate.getMonth() + 1) + '/' + newDate.getDate()  + ' ' + newDate.getHours() + ':' + newDate.getMinutes() + ':' + newDate.getSeconds() + ';' + req.body.price.replace(",",".") + '\n');

	res.send('OK');
});

app.listen(8082, function () {
  console.log('Example app listening');
});