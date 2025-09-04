'use strict';

const { Joi } = require('./joiUtils');

const routes = require('../routes/socketRoutes');

const utils = require('./utils');
const HELPERS = require('../helpers');
const SERVICES = require('../services');
// const routesArray = require('../routes/socket/socketRoutes');
const { MESSAGES, ERROR_TYPES } = require('./constants');
const swaggerUi = require('swagger-ui-express');
const CONFIG = require('../../config');

const routeUtils = {};

routeUtils.route = async (packet, socket) => {
	// eslint-disable-next-line no-useless-catch
	try {
		const result = routes.map(async (route) => {
			if(route.eventName === packet[0]){
				if(route.auth) {
					await SERVICES.authService.socketAuthentication(socket, route.continueWithoutToken);
				}
				await joiValidatorForSocketMethod(packet[1], route);
			}
		});
		await Promise.all(result);
	} catch (error) {
		throw error;
	}
};
/**
 * function to create routes in the express.
 */
routeUtils.apiRoute = async (app, routes = []) => {
	routes.forEach((route) => {
		const middlewares = [];
		middlewares.push(getValidatorMiddleware(route));
		app.route(route.path)[route.method.toLowerCase()](...middlewares, getHandlerMethod(route));
	});
	createSwaggerUIForRoutes(app, routes);
};

/**
* function to check the error of all joi validations
* @param {*} joiValidatedObject 
*/
const checkJoiValidationError = (joiValidatedObject) => {
	if (joiValidatedObject.error) throw joiValidatedObject.error;
};

/**
* function to validate request body/params/query/headers with joi schema to validate a request is valid or not.
* @param {*} route 
*/
const joiValidatorForSocketMethod = async (request, route) => {

	if (route.joiSchemaForSocket && Object.keys(route.joiSchemaForSocket).length) {
		request = await Joi.object(route.joiSchemaForSocket).validate(request);
		checkJoiValidationError(request);
	}
};

/**
 * function to validate request body/params/query/headers with joi schema to validate a request is valid or not.
 * @param {*} route
 */
const joiValidatorMethod = async (request, route) => {
	if (route.joiSchemaForSwagger.params && Object.keys(route.joiSchemaForSwagger.params).length) {
		request.params = Joi.object(route.joiSchemaForSwagger.params).validate(request.params);
		checkJoiValidationError(request.params);
	}
	if (route.joiSchemaForSwagger.body && Object.keys(route.joiSchemaForSwagger.body).length) {
		request.body = Joi.object(route.joiSchemaForSwagger.body).unknown(false).validate(request.body);
		checkJoiValidationError(request.body);
	}
	if (route.joiSchemaForSwagger.query && Object.keys(route.joiSchemaForSwagger.query).length) {
		request.query = Joi.object(route.joiSchemaForSwagger.query).unknown(false).validate(request.query);
		checkJoiValidationError(request.query);
	}
	if (route.joiSchemaForSwagger.headers && Object.keys(route.joiSchemaForSwagger.headers).length) {
		const headersObject = Joi.object(route.joiSchemaForSwagger.headers).unknown(true).validate(request.headers);
		checkJoiValidationError(headersObject);
		request.headers.authorization = ((headersObject || {}).value || {}).authorization;
	}
};

/**
 * middleware to validate request body/params/query/headers with JOI.
 * @param {*} route
 */
const getValidatorMiddleware = (route) => {
	return (request, response, next) => {
		joiValidatorMethod(request, route)
			.then(() => {
				return next();
			})
			.catch((err) => {
				const error = utils.convertErrorIntoReadableForm(err);
				const responseObject = HELPERS.createErrorResponse(error.message.toString(), ERROR_TYPES.BAD_REQUEST);
				return response.status(responseObject.statusCode).json(responseObject);
			});
	};
};

/**
 * middleware
 * @param {*} handler
 */
const getHandlerMethod = (route) => {
	const handler = route.handler;
	return (request, response) => {
		let payload = {
			...(request?.body?.value || {}),
			...(request?.params?.value || {}),
			...(request?.query?.value || {}),
			file: request.file || {},
			user: request?.user || {}
		};
		// request handler/controller
		if (route.getExactRequest) {
			request.payload = payload;
			payload = request;
		}
		handler(payload)
			.then((result) => {
				if (result.filePath) {
					return response.status(result.statusCode).sendFile(result.filePath);
				} else if (result.fileData) {
					response.attachment(result.fileName);
					response.send(result.fileData.Body);
					return response;
				} else if (result.redirectUrl) {
					return response.redirect(result.redirectUrl);
				}
				response.status(result.statusCode).json(result);
			})
			.catch((err) => {
				console.log('Error is ', err);
				request.body.error = {};
				request.body.error.message = err.message;
				if (!err.statusCode && !err.status) {
					err = HELPERS.createErrorResponse(MESSAGES.SOMETHING_WENT_WRONG, ERROR_TYPES.INTERNAL_SERVER_ERROR);
				}
				response.status(err.statusCode).json(err);
			});
	};
};

/**
 * function to create Swagger UI for routes
 */
const createSwaggerUIForRoutes = (app, routes) => {
	// Generate swagger documentation for routes
	routes.forEach((route) => {
		if (route.joiSchemaForSwagger) {
			SERVICES.swaggerService.swaggerDoc.addNewRoute(route.joiSchemaForSwagger, route.path, route.method);
		}
	});

	// Create swagger.json file
	SERVICES.swaggerService.swaggerDoc.createJsonDoc(
		CONFIG.swagger.info,
		`${CONFIG.SERVER.HOST}:${CONFIG.SERVER.PORT}`,
		'/api'
	);

	// Serve swagger.json
	app.get('/swagger.json', (req, res) => {
		const swaggerDocument = require('../../swagger.json');
		res.json(swaggerDocument);
	});

	// Serve Swagger UI
	app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(require('../../swagger.json')));
};

module.exports = routeUtils;