'use strict';

const pino = require('pino');
const MONGOOSE = require('mongoose');
const CONSTANTS = require('./constants');
const JWT = require('jsonwebtoken');
// const { Web3 } = require('web3');
const crypto = require('crypto');
const etherscan = require('etherscan-api').init('N1KVJ2VFVFRAEFJMZDNI94RSV1K96X7YTB');

const axios = require('axios');

const AsyncLock = require('async-lock');

const { createPinoBrowserSend, createWriteStream } = require('pino-logflare');


const { AWS, SMTP, WEB_URL, ADMIN_WEB_URL, AWS_CFS_SERVER, AWS_CFS_EXERCISE, PINO, ENVIRONMENT } = require('../../config');

const PINO_CRED = { apiKey: PINO.API_KEY, sourceToken: PINO.API_SECRET };

const stream = createWriteStream(PINO_CRED); // create pino-logflare stream
const send = createPinoBrowserSend(PINO_CRED); // create pino-logflare browser stream



const awsSnsConfig = {
	accessKeyId: AWS.accessKeyId,
	secretAccessKey: AWS.secretAccessKey,
	region: AWS.awsRegion,
};

const awsCFSignParams = {
	keypairId: AWS_CFS_SERVER.KEY_PAIR_ID, /** -- this is public key */
	privateKeyPath: AWS_CFS_SERVER.PRIVATE_KEY_PATH
	// privateKeyString: AWS_CFS_SERVER.PRIVATE_KEY /** --Optional - this can be used as an alternative to privateKeyPath */ 
};

const awsDataContentCFSignParams = {
	keypairId: AWS_CFS_EXERCISE.KEY_PAIR_ID, /** -- this is public key */
	privateKeyPath: AWS_CFS_EXERCISE.PRIVATE_KEY_PATH
	// privateKeyString: AWS_CFS_SERVER.PRIVATE_KEY /** --Optional - this can be used as an alternative to privateKeyPath */ 
};

const commonFunctions = {};

/**
* incrypt password in case user login implementation
* @param {*} payloadString 
*/
commonFunctions.hashPassword = (payloadString) => {
	return BCRYPT.hashSync(payloadString, CONSTANTS.SECURITY.BCRYPT_SALT);
};

/**
* @param {string} plainText 
* @param {string} hash 
*/
commonFunctions.compareHash = (payloadPassword, userPassword) => {
	return BCRYPT.compareSync(payloadPassword, userPassword);
};

/**
* function to get array of key-values by using key name of the object.
*/
commonFunctions.getEnumArray = (obj) => {
	return Object.keys(obj).map((key) => obj[key]);
};

/** used for converting string id to mongoose object id */
commonFunctions.convertIdToMongooseId = (stringId) => {
	return MONGOOSE.Types.ObjectId(stringId);
};

/** create jsonwebtoken **/
commonFunctions.encryptJwt = (payload) => {
	return JWT.sign(payload, CONSTANTS.SECURITY.JWT_SIGN_KEY, { algorithm: 'HS256' });
};

commonFunctions.decryptJwt = (token) => {
	return JWT.verify(token, CONSTANTS.SECURITY.JWT_SIGN_KEY, { algorithm: 'HS256' });
};

/**
* function to convert an error into a readable form.
* @param {} error 
*/
commonFunctions.convertErrorIntoReadableForm = (error) => {
	let errorMessage = '';
	if (error.message.indexOf('[') > -1) {
		errorMessage = error.message.substr(error.message.indexOf('['));
	} else {
		errorMessage = error.message;
	}
	errorMessage = errorMessage.replace(/"/g, '');
	errorMessage = errorMessage.replace('[', '');
	errorMessage = errorMessage.replace(']', '');
	error.message = errorMessage;
	return error;
};

/***************************************
**** Logger for error and success *****
***************************************/
commonFunctions.log = {
	info: (data) => {
		console.log('\x1b[33m' + data, '\x1b[0m');
	},
	success: (data) => {
		console.log('\x1b[32m' + data, '\x1b[0m');
	},
	error: (data) => {
		console.log('\x1b[31m' + data, '\x1b[0m');
	},
	default: (data) => {
		console.log(data, '\x1b[0m');
	}
};

/**
* function to get pagination condition for aggregate query.
* @param {*} sort 
* @param {*} skip 
* @param {*} limit 
*/
commonFunctions.getPaginationConditionForAggregate = (sort, skip, limit) => {
	const condition = [
		...(sort ? [ { $sort: sort } ] : []),
		{ $skip: skip },
		{ $limit: limit }
	];
	return condition;
};

/**
* function to remove undefined keys from the payload.
* @param {*} payload 
*/
commonFunctions.removeUndefinedKeysFromPayload = (payload = {}) => {
	for (const key in payload) {
		if (!payload[key]) {
			delete payload[key];
		}
	}
};

/**
* Send an email to perticular user mail 
* @param {*} email email address
* @param {*} subject  subject
* @param {*} content content
* @param {*} cb callback
*/
commonFunctions.sendEmail = async (userData, type) => {
	const transporter = require('nodemailer').createTransport(SMTP.TRANSPORT);
	const handleBars = require('handlebars');
	/** setup email data with unicode symbols **/
	const mailData = commonFunctions.emailTypes(userData, type), email = userData.email, ccEmail = userData.ccEmail, bccEmail = userData.bccEmail;
	let template = '';
	let result = '';
	if (mailData && mailData.template) {
		template = handleBars.compile(mailData.template);
	}
	if (template) {
		result = template(mailData.data);
	}

	const emailToSend = {
		to: email,
		cc: ccEmail,
		bcc: bccEmail,
		from: SMTP.SENDER,
		subject: mailData.Subject
	};
	if (type == CONSTANTS.EMAIL_TYPES.FORGOT_PASSWORD_EMAIL) {
		emailToSend.text = userData.token;
	}
	if (type == CONSTANTS.EMAIL_TYPES.SEND_INVITE_EMAIL) {
		emailToSend.html = userData.link;
	}
	if (userData.attachments && userData.attachments.length) {
		emailToSend.attachments = userData.attachments;
	}
	if (result) {
		emailToSend.html = result;
	}
	if (userData.icalEvent) {
		emailToSend.icalEvent = userData.icalEvent;
	}
	return await transporter.sendMail(emailToSend);
};


commonFunctions.emailTypes = (user, type) => {
	const EmailStatus = {
		Subject: '',
		data: {},
		template: ''
	};
	switch (type) {
	case CONSTANTS.EMAIL_TYPES.WELCOME_EMAIL:
		EmailStatus['Subject'] = CONSTANTS.EMAIL_SUBJECTS.WELCOME_EMAIL;
		EmailStatus.template = CONSTANTS.EMAIL_CONTENTS.WELCOME_EMAIL;
		EmailStatus.data['name'] = user.name;
		break;

	case CONSTANTS.EMAIL_TYPES.VERIFICATION_EMAIL:
		EmailStatus['Subject'] = CONSTANTS.EMAIL_SUBJECTS.VERIFICATION_EMAIL;
		EmailStatus.template = CONSTANTS.EMAIL_CONTENTS.VERIFICATION_EMAIL;
		EmailStatus.data['name'] = user.name;
		break;

	case CONSTANTS.EMAIL_TYPES.FORGOT_PASSWORD_EMAIL:
		EmailStatus['Subject'] = CONSTANTS.EMAIL_SUBJECTS.FORGOT_PASSWORD_EMAIL;
		EmailStatus.template = CONSTANTS.EMAIL_CONTENTS.FORGOT_PASSWORD_EMAIL;
		EmailStatus.data['name'] = user.name;
		EmailStatus.data['token'] = user.token;
		break;

	case CONSTANTS.EMAIL_TYPES.ICALENDER_EMAIL:
		EmailStatus['Subject'] = CONSTANTS.EMAIL_SUBJECTS.ICALENDER_EMAIL;
		break;
        
	case CONSTANTS.EMAIL_TYPES.SEND_INVITE_EMAIL:
		EmailStatus['Subject'] = CONSTANTS.EMAIL_SUBJECTS.SEND_INVITE_EMAIL;
		break;    

	default:
		EmailStatus['Subject'] = 'Welcome Email!';
		break;
	}
	return EmailStatus;
};

commonFunctions.renderTemplate = (template, data) => {
	return handlebars.compile(template)(data);
};

/**
* function to create reset password link.
*/
commonFunctions.createResetPasswordLink = (userData) => {
	const dataForJWT = { _id: userData._id, Date: Date.now, email: userData.email };
	const baseUrl = (userData.userType == CONSTANTS.USER_TYPE.STAFF) ? WEB_URL : ADMIN_WEB_URL;
	const resetPasswordLink = baseUrl + '/reset-password/' + commonFunctions.encryptJwt(dataForJWT);
	return resetPasswordLink;
};

/**
* function to generate random otp string
*/
commonFunctions.generateOTP = (length) => {
	const chracters = '0123456789';
	let randomString = '';
	for (let i = length; i > 0; --i)
		randomString += chracters[Math.floor(Math.random() * chracters.length)];

	return randomString;
};

/** -- function to returns a random number between min and max (both included) */
commonFunctions.getRandomInteger = (min, max) => {
	return Math.floor(Math.random() * (max - min + 1)) + min;
};

/**
* function to sent sms via AWS-SNS
* @param {receiver} phoneNumber
* @param {content} SMS 
*/
commonFunctions.sendSms = async (receiver, content) => {
	const msg = {
		'message': content,
		'sender': AWS.smsSender || 'Backend Team',
		'phoneNumber': receiver
	};
	const smsResponse = await awsSms(awsSnsConfig, msg);
	return smsResponse;
};

// Function to generate expiry time in seconds
commonFunctions.generateExpiryTime = (seconds) => {
	return new Date(new Date().setSeconds(new Date().getSeconds() + seconds));
};

/**
 * 
 * @param {string} filePath
 * @returns aws cloudfront signed url
*/
commonFunctions.getServerCFSignedUrl = (filePath) => {
	try {
		if (!filePath) return filePath;
		awsCFSignParams.expireTime = Date.now() + 3600000;
		const cfUrl = AWS_CFS_SERVER.URL;
		const s3ObjectPath = `${cfUrl}/${filePath}`; /** -- example: 'http://example.cloudfront.net/path/to/s3/object' */
		const signedUrl = cfSign.getSignedUrl(s3ObjectPath, awsCFSignParams);
		return signedUrl;
	} catch (error) {
		return filePath;
	}
};

/**
 * 
 * @param {Object} data
 * @param {string} data.filePath
 * @returns aws cloudfront signed RTMP object
*/
commonFunctions.getServerCFSignedRTMPUrl = (data) => {
	awsCFSignParams.expireTime = Date.now() + 3600000;
	let cfUrl = AWS_CFS_SERVER.URL;
	cfUrl = cfUrl.replace(/(^\w+:|^)\/\//, ''); /** -- remove http protocol */
	const signedRTMPUrlObj = cfSign.getSignedRTMPUrl(cfUrl, data.filePath, awsCFSignParams);
	return signedRTMPUrlObj;
};

/**
 * 
 * @param {string} filePath
 * @returns aws cloudfront CONTENT signed url
*/
commonFunctions.getDataContentCFSignedUrl = (filePath) => {
	if (!filePath) return filePath;
	try {
		awsDataContentCFSignParams.expireTime = Date.now() + 3600000;
		const cfUrl = AWS_CFS_EXERCISE.URL;
		const s3ObjectPath = `${cfUrl}/${filePath}`; /** -- example: 'http://example.cloudfront.net/path/to/s3/object' */
		const signedUrl = cfSign.getSignedUrl(s3ObjectPath, awsDataContentCFSignParams);
		return signedUrl;
	} catch (error) {
		return filePath;
	}
};

/**
 * function to convert seconds in HMS string
 * @param {*} value 
 * @returns 
 */
commonFunctions.convertSecondsToHMS = (value) => {
	const sec = parseInt(value, 10);
	const hours = Math.floor(sec / 3600);
	const minutes = Math.floor((sec - (hours * 3600)) / 60);
	const seconds = sec - (hours * 3600) - (minutes * 60);
	str = '';
	if (hours) str = str + hours + (hours > 1 ? ' Hours' : ' Hour');
	if (minutes) str = str + ' ' + minutes + (minutes > 1 ? ' Minutes' : ' Minute');
	if (seconds) str = str + ' ' + seconds + (seconds > 1 ? ' Seconds' : ' Second');

	return str.trim();
};

/**
 * Variable to create logging
 */
commonFunctions.logger = pino({
	browser: {
		transmit: {
			send,
		},
	}
}, stream
);

/**
 * function to create session
 * @param {*} payload 
 * @returns 
 */
commonFunctions.createSession = async (payload) => {
	let sessionData = {};

	sessionData.token = commonFunctions.encryptJwt({
		userId: payload.userId,
		date: Date.now()
	});

	if(payload.tokenType === CONSTANTS.TOKEN_TYPES.OTP){
		sessionData.token = commonFunctions.generateOTP(CONSTANTS.OTP_LENGTH);
		sessionData.tokenExpDate = commonFunctions.generateExpiryTime(CONSTANTS.OTP_EXPIRIED_TIME_IN_SECONDS || 10);
	}

	if (ENVIRONMENT === 'development' || payload.tokenType != CONSTANTS.TOKEN_TYPES.LOGIN) {
		sessionData = {
			...sessionData,
			userId: payload.userId,
			userType: payload.userType,
			tokenType: payload.tokenType || CONSTANTS.TOKEN_TYPES.LOGIN
		};
		// TODO: Implement session storage without MongoDB
	}
	return sessionData.token;
};

/**
 * Function to connect to blockchain network
 */
// commonFunctions.connectToNetwork = async () => {
// 	const web3 = new WEB3(new WEB3.providers.HttpProvider(process.env.INFURA_RINKEBY_LINK));
// 	const contract = new web3.eth.Contract(require('../../abi.json'), process.env.CONTRACT_ADDRESS);
// 	return { web3, contract };
// };


/**
 * Function to hit Third party api's.
 */
commonFunctions.callThirdPartyAPI = {
	post: async ({ API = '', DATA = {}, HEADER = {} }) => {
		return axios.post(API, DATA, { headers: HEADER });
	},
	get: async ({ API = '', PARAMS = {}, HEADER = {}, responseType = 'json' }) => {
		return axios.get(API, { params: PARAMS, headers: HEADER, responseType });
	},
	put: async ({ API = '', DATA = {}, HEADER = {} }) => {
		return axios.put(API, DATA, { headers: HEADER });
	},
	delete: async ({ API = '', DATA = {}, HEADER = {} }) => {
		return axios.delete(API, DATA, { headers: HEADER });
	},
	patch: async ({ API = '', DATA = {}, HEADER = {} }) => {
		return axios.patch(API, DATA, { headers: HEADER });
	}
};

commonFunctions.call3rdPartyAPI = async (payload = { method, url, data, headers }) => {
	if (payload.method === CONSTANTS.METHODS_TYPE.GET) {
		payload.params = payload.params || payload.data;
	}

	const response = await axios(payload);
	return response?.data?.data || response?.data;
};

// const web3 = new Web3('https://cloudflare-eth.com');

// const latestBlock = await web3.eth.getBlock('latest');
// return latestBlock.hash;


//BLOCKSYPHER
// const url = 'https://api.blockcypher.com/v1/btc/main';
// const response = await axios.get(url);
// return response.data.hash;

// USING ETHERSCAN
// const url = 'https://api.etherscan.io/api?module=proxy&action=eth_blockNumber&apikey=N1KVJ2VFVFRAEFJMZDNI94RSV1K96X7YTB';
// const response = await axios.get(url);
// const latestBlock = response.data.result;

// const blockUrl = `https://api.etherscan.io/api?module=proxy&action=eth_getBlockByNumber&tag=${latestBlock}&boolean=true&apikey=N1KVJ2VFVFRAEFJMZDNI94RSV1K96X7YTB`;
// const blockResponse = await axios.get(blockUrl);
// return blockResponse.data.result.hash;
	
// Fetch the latest block hash from Ethereum blockchain
commonFunctions.getBlockHash = async () => {

	try{
		const blocks = await etherscan.proxy.eth_blockNumber();
		const totalBlocks = parseInt(blocks.result, 16);

		const randomBlockNumber = Math.floor(Math.random() * totalBlocks);
		const block = await etherscan.proxy.eth_getBlockByNumber(`0x${randomBlockNumber.toString(16)}`, false);

		const blockHash = block.result.hash;
		const timestamp = new Date().getTime();
		return { blockHash, timestamp, randomBlockNumber };
	} catch(err){
		console.log(err);
	}
};

// Generate a random number using the block hash
commonFunctions.generateRandomNumber = async (limit = 100, blockHash, timestamp) => {
	let blockNumber;
	if (!blockHash) { 
		const latestBlock = await commonFunctions.getBlockHash();
		blockHash = latestBlock.blockHash;
		timestamp = latestBlock.timestamp;
		blockNumber = latestBlock.randomBlockNumber;
	}    
	// Use the block hash as a seed to create a random number
	const hash = await crypto.createHash('sha256').update(blockHash + timestamp).digest('hex');
	const randomNumber = (parseInt(hash, 16) % limit) + 1; // Example: random number between 1 and 100
    
	return { randomNumber, blockHash, timestamp, blockNumber };
};


/**
 * Function to add delay
 * @param {*} delay 
 * @returns 
 */
commonFunctions.addDelay = async (delay) => {
	return new Promise((resolve) => setTimeout(() => resolve(), delay));
};

/**
 * Function t lock function.
 * @param {*} lockKeys 
 * @param {*} funtionToExecute 
 * @param {*} functionCallData 
 * @returns 
 */
commonFunctions.lockFunction = async (lockKeys, funtionToExecute, functionCallData = []) => {

	const lock = new AsyncLock();
	let data;
	await lock.acquire( lockKeys, async function (done) { 
		data = await funtionToExecute(...functionCallData);
		done();
	}, {});

	return data;
};

commonFunctions.getRandomFromArray = async (arr) => {
	const middleIndex = Math.floor(arr.length / 2);
	let middleValues;

	if (arr.length % 2 === 0) {
		// Even number of elements
		middleValues = arr.slice(middleIndex - 3, middleIndex + 3);
	} else {
		// Odd number of elements
		middleValues = arr.slice(middleIndex - 2, middleIndex + 3);
	}

	const randomNum = Math.random();
	if (randomNum < 0.9) {
		// 90% chance to pick from middleValues
		const middleIndex = Math.floor(Math.random() * middleValues.length);
		return middleValues[middleIndex];
	} else {
		// 10% chance to pick from entire array
		const randomIndex = Math.floor(Math.random() * arr.length);
		return arr[randomIndex];
	}

};

module.exports = commonFunctions;