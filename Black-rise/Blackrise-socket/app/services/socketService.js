'use strict';

const service = {
	emit: async (eventName, socketId, sendData) => {
		console.log('emitToUser', eventName);
		global.io.to(socketId).emit(eventName, sendData);
		return true;
	},
	emitToManyUsers: async (eventName, users, sendData) => {
		for await (const userId of users) {
			global.io.to(userId.toString()).emit(eventName, sendData);
		}
		return true;
	},
	emitToRoom: async (eventName, roomId, sender, sendData) => {
		/** -- Broadcast to room except the sender. */
		global.io.sockets.connected[sender.toString()].to(roomId.toString()).emit(eventName, sendData);
		return true;
	},
	joinRoom: async (roomId, userId) => {
		Array.from(global.io.sockets.sockets).map((socket) => {
			if(socket[0] === userId.toString()){
				socket[1].join(roomId.toString());
			}
		});
		return true;
	},
	leaveRoom: async (roomId, userId) => {
		Array.from(global.io.sockets.sockets).map((socket) => {
			if(socket[0] === userId.toString()){
				socket[1].leave(roomId.toString());
			}
		});
	},
	emitInRoom: async (eventName, roomId, sendData) => {
		console.log('emitToRoom', eventName);

		/** -- Broadcast in room. */
		global.io.sockets.in(roomId.toString()).emit(eventName, sendData);
		return true;
	},
	checkUserInRoom: (userId, roomId) => {
		if (global.io && global.io.sockets && global.io.sockets.adapter && global.io.sockets.adapter.sids && global.io.sockets.adapter.sids[userId.toString()] && global.io.sockets.adapter.sids[userId.toString()][roomId.toString()]) {
			return true;
		}
		return false;
	},
	emitToAll: (eventName, sendData) => {
		global.io.emit(eventName, sendData);
	},
	emitToAllExceptItsSelf: (eventName, userId, sendData) => {
		if (global.io && global.io.sockets && global.io.sockets.connected && global.io.sockets.connected[userId.toString()]) {
			global.io.sockets.connected[userId.toString()].broadcast.emit(eventName, sendData);
		} else {
			global.io.emit(eventName, sendData);
		}
		return;
	}
};

module.exports = service;