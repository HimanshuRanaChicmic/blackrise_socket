'use strict';

/********************************
 **** Managing all the services ***
 ********* independently ********
 ********************************/
module.exports = {
	swaggerService: require('./swaggerService'),
	fileUploadService: require('./fileUploadService'),
	socketService: require('./socketService'),
};