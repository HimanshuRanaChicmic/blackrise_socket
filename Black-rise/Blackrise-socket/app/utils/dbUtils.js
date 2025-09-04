'use strict';

const CONSTANTS = require('../utils/constants');

const dbUtils = {};

/**
* function to check valid reference from models.
*/
dbUtils.checkValidReference = async (document, referenceMapping) => {
	for (const key in referenceMapping) {
		const model = referenceMapping[key];
		if (!!document[key] && !(await model.findById(document[key]))) {
			throw CONSTANTS.RESPONSE.ERROR.BAD_REQUEST(key + ' is invalid.');
		}
	}
};

module.exports = dbUtils;