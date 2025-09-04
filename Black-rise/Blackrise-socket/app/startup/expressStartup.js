'use strict';

const cors = require('cors');
const routes = require('../routes/api');
const routeUtils = require('../utils/routeUtils');

const { migerateDatabase } = require('../utils/dbMigrations');

module.exports = async function (app) {
	app.use(cors());
	app.use(require('body-parser').json({ limit: '50mb' }));
	app.use(require('body-parser').urlencoded({ limit: '50mb', extended: true }));

	// let socketRedisAdapter = process.env.SOCKET_REDIS_ADAPTER;
	// if(socketRedisAdapter && socketRedisAdapter.toLowerCase() === 'true'){
	//     await require('./db_redis')();
	// }

	/********************************
    ***** For handling CORS Error ***
    *********************************/
	app.all('/*', (request, response, next) => {
		response.header('Access-Control-Allow-Origin', '*');
		response.header('Access-Control-Allow-Headers', 'Content-Type, api_key, Authorization, x-requested-with, Total-Count, Total-Pages, Error-Message');
		response.header('Access-Control-Allow-Methods', 'POST, GET, DELETE, PUT, OPTIONS');
		response.header('Access-Control-Max-Age', 1800);
		next();
	});

	// serve static folder.
	// app.use('/public', express.static('public'));
	// app.use('/uploads', express.static('uploads'));

	await migerateDatabase(); // run database migrations.

	await routeUtils.apiRoute(app, routes);

	// Cron service removed due to MongoDB dependency

};