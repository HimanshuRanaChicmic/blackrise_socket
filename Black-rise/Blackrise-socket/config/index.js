'use strict';

const path = require('path');
const development = require('./env/development');
const production = require('./env/production');
const staging = require('./env/staging');

const PLATFORM = process.env.PLATFORM || 'Backend';

const defaults = {
	PLATFORM: PLATFORM,
	SERVER_TYPE_ENV: process.env.SERVER_TYPE || '',
	ROOT_PATH: path.normalize(__dirname + '/../app'),
	ENV_STAGING: 'staging',
	ENV_DEVELOPMENT: 'development',
	ENV_PRODUCTION: 'production',
	ENVIRONMENT: process.env.NODE_ENV || 'production',
	DEFAULT_TZ: process.env.TZ || 'UTC',
	API_AUTH_KEY: process.env.API_AUTH_KEY || 'apitestkey', /** -- this requires in header to authenticate each api under "X-API-KEY" */
	SERVER_URL: process.env.SERVER_URL || 'http://localhost:3001',
	SOCKET_SERVER_URL: process.env.SOCKET_SERVER_URL || 'http://localhost:3001',
	S3_ASSETS_URL: process.env.S3_ASSETS_URL + '/' || 'http://localhost:3001/',
	WEB_URL: process.env.WEB_URL || 'http://localhost:3001',
	ADMIN_WEB_URL: process.env.ADMIN_WEB_URL || 'http://localhost:3001',
	swagger: require('./swagger'),
	PATH_TO_UPLOAD_FILES: process.env.PATH_TO_UPLOAD_FILES || '/uploads/files',
	UPLOAD_TO_S3_BUCKET: process.env.UPLOAD_TO_S3_BUCKET || false,
	USER_DEFAULT_PASSWORD: process.env.USER_DEFAULT_PASSWORD || '@1User-Password@1',
	SENDINBLUE: {
		API_KEY: 'dummy',
		SENDER_EMAIL: 'contact@projectdomain.com'
	},
	SMTP: {
		TRANSPORT: {
			host: process.env.NODEMAILER_HOST || 'node-mailer-host-name',
			port: process.env.NODEMAILER_PORT || 'node-mailer-host-port',
			// service: process.env.NODEMAILER_SERVICE || `gmail`,
			auth: {
				user: process.env.NODEMAILER_USER || 'node-mailer-user',
				pass: process.env.NODEMAILER_PASSWORD || 'node-mailer-pass'
			},
			secure: false,
			tls: { rejectUnauthorized: false },
		},
		SENDER: `${process.env.SENDER_NAME} <${process.env.SENDER_EMAIL}>`,
	},
	MICROSERVICE_CONFIG: {
		API_URL: process.env.API_URL || 'http://localhost:5000',
	},
	
	REDIS: {
		PORT: process.env.REDIS_PORT || '6379',
		HOST: process.env.REDIS_HOST || '127.0.0.1',
		PASSWORD: process.env.REDIS_PASSWORD || ''
	},
	FIREBASE: {
		SERVER_KEY: process.env.FIREBASE_SERVER_KEY || 'firebase server key',
		WEB_SERVER_KEY: process.env.FIREBASE_WEB_SERVER_KEY_PATH || 'firebase web-server key'
	},
	SERVER: {
		PROTOCOL: process.env.SERVER_PROTOCOL || 'http',
		HOST: process.env.SERVER_HOST || '0.0.0.0',
		PORT: process.env.SERVER_PORT || '3001',
		SOCKET_PORT: process.env.SOCKET_PORT || '4001',
		get URL() { return `${this.PROTOCOL}://${this.HOST}:${this.PORT}`; }
	},
	SWAGGER_AUTH: {
		USERNAME: process.env.SWAGGER_AUTH_USERNAME || 'username',
		PASSWORD: process.env.SWAGGER_AUTH_PASSWORD || 'password'
	},
	S3_BUCKET: {
		accessKeyId: process.env.AWS_ACCESS_KEY_ID || 'access-key-id',
		secretAccessKey: process.env.AWS_SECRET_ACESS_KEY || 'secret-access-key',
		bucketName: process.env.S3_BUCKET_NAME || 'bucket-name',
		cloudfront_url: process.env.CLOUDFRONT_URL || 'cloudfront-url'
	},
	AWS: {
		accessKeyId: process.env.AWS_ACCESS_KEY_ID || 'aws_access_key',
		secretAccessKey: process.env.AWS_SECRET_ACESS_KEY || 'aws_secret_key',
		awsRegion: process.env.AWS_REGION || 'us-east-1',
		smsSender: process.env.SMS_SENDER || 'sms_sender'
	},
	AWS_CFS_SERVER: {
		URL: process.env.AWS_CLOUDFRONT_URL || '',
		KEY_PAIR_ID: process.env.AWS_CLOUDFRONT_KEY_PAIR_ID || '',
		PRIVATE_KEY_PATH: process.env.AWS_CLOUDFRONT_PRIVATE_KEY_PATH || ''
	},
	AWS_CFS_EXERCISE: {
		URL: process.env.AWS_EXERCISE_CLOUDFRONT_URL || '',
		KEY_PAIR_ID: process.env.AWS_EXERCISE_CLOUDFRONT_KEY_PAIR_ID || '',
		PRIVATE_KEY_PATH: process.env.AWS_EXERCISE_CLOUDFRONT_PRIVATE_KEY_PATH || ''
	},
	SUPER_ADMIN: {
		EMAIL: process.env.SUPER_ADMIN_EMAIL || 'superadmin@yopmail.com',
		PASSWORD: process.env.SUPER_ADMIN_PASS || '@1SuperAdmin@1',
		NAME: process.env.SUPER_ADMIN_NAME || 'Super Admin'
	},
	PINO: {
		API_KEY: process.env.PINO_API_KEY || 'pino api key',
		API_SECRET: process.env.PINO_API_SECRET || 'pino secret key',    
	},
	
	PUSH_ALERT_API_URL: process.env.PUSH_ALERT_API_URL || 'https://api.pushalert.com',
	PUSH_ALERT_API_KEY: process.env.PUSH_ALERT_API_KEY || 'test',
	CLIENT_SIDE_REDIRECT_URL: process.env.CLIENT_SIDE_REDIRECT_URL || 'http://localhost:4001',
	ADMIN_SIDE_REDIRECT_URL: process.env.ADMIN_SIDE_REDIRECT_URL || 'http://localhost:4001',
};

const currentEnvironment = process.env.NODE_ENV || 'production';

function myConfig(envConfig) {
	return { ...defaults, ...envConfig };
}

module.exports = {
	development: myConfig(development),
	production: myConfig(production),
	staging: myConfig(staging)
}[currentEnvironment];