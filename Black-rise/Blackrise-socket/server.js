'use strict';

/***********************************
**** node module defined here *****
***********************************/
require('dotenv').config();
const EXPRESS = require('express');

const { SERVER } = require('./config');
const { EventEmitter } = require('events');
global.eventEmitter = new EventEmitter();

/**creating express server app for server */
const app = EXPRESS();

app.set('port', SERVER.SOCKET_PORT);
const server = require('http').Server(app);

global.io = require('socket.io')(server, {
	cors: {
		origin: '*',
		methods: [ 'GET', 'POST' ]
	}
});

const p2p = require('socket.io-p2p-server').Server;
global.io.use(p2p);

/********************************
***** Server Configuration *****
********************************/
/** Server is running here */
const startNodeserver = async () => {
    
	// await require('./app/startup/db_redis')(); //initialise redis.

	//intialize socket
	await require('./app/startup/socket').connect(global.io);
	await require('./app/startup/expressStartup')(app);
	return new Promise((resolve, reject) => {
		server.listen(SERVER.SOCKET_PORT, (err) => {
			if (err) reject(err);
			resolve();
		});
	});
};

startNodeserver().then(() => {
	console.log('Socket server running on port', SERVER.SOCKET_PORT);
}).catch((err) => {
	console.log('Error in starting server', err);
	process.exit(1);
});

process.on('unhandledRejection', (error) => {
	// Will print "unhandledRejection err is not defined"
	console.log('unhandledRejection', error);
});