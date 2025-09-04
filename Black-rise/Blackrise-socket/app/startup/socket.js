/* eslint-disable no-console */
/** -- import all modules */
const { SOCKET_EVENTS } = require('../utils/constants');
const routeUtils = require('../utils/routeUtils');
const { auctionController } = require('../controllers');


const socketConnection = {};

socketConnection.connect = async function (io) {
	io.on('connection', async (socket) => {
		socket.use(async (packet, next) => {
			/** -- validate here */
			try {
				await routeUtils.route(packet, socket);
				next();
			} catch (error) {
				packet[2]({ success: false, message: error.message });
			}  
		});

		socket.on(SOCKET_EVENTS.JOIN_ROOM, async (payload, callback) => {
			await auctionController.joinRoom({ ...payload }, socket, callback);
		});

		socket.on(SOCKET_EVENTS.CREATE_BID, async (payload, callback) => {
			await auctionController.createBid({ ...payload, user: socket.user, userId: socket.userId }, io, callback);
		});

		socket.on(SOCKET_EVENTS.CANCEL_AUTO_BID, async (payload, callback) => {
			await auctionController.cancelAutoBid({ ...payload, user: socket.user, userId: socket.userId }, io, callback);
		});

			
		socket.on(SOCKET_EVENTS.DISCONNECT, async () => {
			// console.log('Disconnected socket id: ', socket.id);
		});
	});
};

module.exports = socketConnection;
