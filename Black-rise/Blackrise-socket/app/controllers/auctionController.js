'use strict';

const MESSAGES = require('../utils/messages');
const CONFIG = require('../../config');
const { createSuccessResponse, createErrorResponse } = require('../helpers/common/resHelper');
const CONSTANTS = require('../utils/constants');

/**************************************************
 ***** Auction controller for auction logic ******
 **************************************************/
const auctionController = {};

/**
 * Function to handle auction added event
 * @param {*} payload 
 */
auctionController.auctionAdded = async () => {
	// Emit socket event for auction added
	io.sockets.emit(CONSTANTS.SOCKET_EVENTS.AUCTION_ADDED, { auctionAdded: true });
	return createSuccessResponse(MESSAGES.SUCCESS);
};

/**
 * Function for auction subscribed
 * @param {*} payload 
 */
auctionController.auctionSubscribed = async (payload) => {
	// TODO: Implement auction subscription logic without MongoDB
	// For now, just emit a socket event
	io.sockets.emit(CONSTANTS.SOCKET_EVENTS.AUCTION_UPDATED, { 
		data: { 
			auctionId: payload.auctionId,
			subscribed: true 
		} 
	});
	return createSuccessResponse(MESSAGES.SUCCESS);
};

/**
 * Function to join room
 * @param {*} payload 
 */
auctionController.joinRoom = async (payload, socket, callback) => {
	const currentRooms = Array.from(socket.rooms);

	currentRooms.forEach((room) => {
		if (room !== socket.id) {
			socket.leave(room);
		}
	});
		
	if (payload.section === CONSTANTS.SECTION_TYPE.AUCTION_PAGE) {
		socket.join(CONSTANTS.SOCKET_ROOMS.AUCTION_PAGE);
	}
	else if (payload.section === CONSTANTS.SECTION_TYPE.TOP_AUCTION) {
		socket.join(CONSTANTS.SOCKET_ROOMS.TOP_AUCTION);
	} else {
		// TODO: Implement auction existence check without MongoDB
		socket.join(payload.auctionId.toString());
	}

	return callback(createSuccessResponse(MESSAGES.SUCCESS));
};

/**
 * Function to cancel a autoBid
 * @param {*} payload 
 */
auctionController.cancelAutoBid = async (payload, socket, callback) => {
	// TODO: Implement auto bid cancellation logic without MongoDB
	return callback(createSuccessResponse(MESSAGES.SUCCESS));
};

/**
 * Function to create bid
 * @param {*} payload 
 */
auctionController.createBid = async (payload, io, callback) => {
	try {
		// TODO: Implement bid creation logic without MongoDB
		// For now, just emit a socket event
		io.sockets.emit(CONSTANTS.SOCKET_EVENTS.AUCTION_UPDATED, { 
			data: { 
				auctionId: payload.auctionId,
				bidCreated: true 
			} 
		});
		return callback(createSuccessResponse(MESSAGES.SUCCESS));
	} catch (err) {
		console.log(err);
		return callback(createErrorResponse(MESSAGES.SOMETHING_WENT_WRONG, CONSTANTS.ERROR_TYPES.SOMETHING_WENT_WRONG));
	}
};

/**
 * Function to process a auto bid
 * @param {*} payload 
 */
auctionController.processAutoBid = async (auctionId) => {
	// TODO: Implement auto bid processing logic without MongoDB
	console.log('Auto bid processing for auction:', auctionId);
};

/* export controller */
module.exports = auctionController;
