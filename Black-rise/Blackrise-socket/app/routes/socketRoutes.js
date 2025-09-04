'use strict';
const { Joi } = require('../utils/joiUtils');
const CONSTANTS = require('../utils/constants');

const routes = [
	{
		action: 'emit',
		eventName: 'test',
		joiSchemaForSocket: {
			data: Joi.string().required(),
		},
		group: 'message',
		description: 'socket event for send message'
	},
	{
		action: 'emit',
		eventName: CONSTANTS.SOCKET_EVENTS.CREATE_BID,
		joiSchemaForSocket: {
			auctionId: Joi.string().objectId().required().description('Id of the auction.'),
			bidType: Joi.number().valid(...Object.values(CONSTANTS.BIDS)).required().description('Auto or manual.'),
			bids: Joi.alternatives().conditional('bidType', { is: CONSTANTS.BIDS.AUTO, then: Joi.number().integer().required().min(1), otherwise: Joi.number().optional() }),
		},
		group: 'AUCTION',
		auth: CONSTANTS.AVAILABLE_AUTHS.USER,
		description: 'socket event to create a bid.'

	},
	{
		action: 'emit',
		eventName: CONSTANTS.SOCKET_EVENTS.CANCEL_AUTO_BID,
		joiSchemaForSocket: {
			auctionId: Joi.string().objectId().required().description('Id of the auction.'),

		},
		group: 'AUCTION',
		auth: CONSTANTS.AVAILABLE_AUTHS.USER,
		description: 'socket event to cancel a auto bid.'

	},
	{
		action: 'emit',
		eventName: CONSTANTS.SOCKET_EVENTS.JOIN_ROOM,
		joiSchemaForSocket: {
			section: Joi.number().valid(...Object.values(CONSTANTS.SECTION_TYPE)).required().description('section name.'),
			auctionId: Joi.alternatives().conditional('section', { is: CONSTANTS.SECTION_TYPE.AUCTION_DETAILED_PAGE, then: Joi.string().objectId().required().description('Id of the auction.'), otherwise: Joi.string().objectId().optional().description('Id of the auction.') }),
		},
		group: 'AUCTION',
		continueWithoutToken: true,
		auth: CONSTANTS.AVAILABLE_AUTHS.USER,
		description: 'socket event to join a room.'

	},
];

module.exports = routes;