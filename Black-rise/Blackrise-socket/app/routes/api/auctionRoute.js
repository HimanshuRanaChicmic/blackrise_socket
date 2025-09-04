'use strict';

const { auctionController } = require('../../controllers');
const { Joi } = require('../../utils/joiUtils');


const routes = [
	{
		method: 'POST',
		path: '/v1/auction/auctionAdded',
		joiSchemaForSwagger: {
			// headers: {
			// 	authorization: Joi.string().optional().description('authorization')
			// },
			group: 'AUCTION',
			description: 'Api to create notification or event for auction add',
			model: 'AuctionAdded'
		},
		// auth: AVAILABLE_AUTHS.USER_OR_KEY,
		handler: auctionController.auctionAdded
	},
	{
		method: 'POST',
		path: '/v1/auction/subscribeAuction',
		joiSchemaForSwagger: {
			body: {
				auctionId: Joi.string().objectId().required().description('Id of the auction.')
			},
			group: 'AUCTION',
			description: 'Route to send a subscription event to auction users.',
			model: 'AuctionSubscribed'
		},
		handler: auctionController.auctionSubscribed
	},
];

module.exports = routes;
