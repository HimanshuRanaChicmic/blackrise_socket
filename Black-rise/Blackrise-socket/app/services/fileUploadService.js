'use strict';

const AWS = require('aws-sdk');
const fs = require('fs');
const path = require('path');
const CONFIG = require('../../config');
const { FILE_UPLOAD_TYPE } = require('../utils/constants');

AWS.config.update({ accessKeyId: CONFIG.S3_BUCKET.accessKeyId, secretAccessKey: CONFIG.S3_BUCKET.secretAccessKey });
let s3Bucket = new AWS.S3();

const fileUploadService = {};

/**
 * function to upload a file to s3(AWS) bucket.
 */
fileUploadService.uploadFileToS3 = (payload, fileName, bucketName) => {
    return new Promise((resolve, reject) => {
        s3Bucket.upload({
            Bucket: bucketName,
            Key: fileName,
            Body: payload.file.buffer,
            ContentType: payload.file.mimetype
            // ACL: 'public-read',
        }, function (err, data) {
            if (err) {
                console.log('Error here', err);
                return reject(err);
            }
            resolve(`${CONFIG.S3_FILE_URL}${data.Key}`);
        });
    });
};

/**
 * function to upload file to local server.
 */
fileUploadService.uploadFileToLocal = async (payload, fileName, pathToUpload, pathOnServer) => {
    let directoryPath = pathToUpload ? pathToUpload : path.resolve(__dirname + `../../..${CONFIG.PATH_TO_UPLOAD_SUBMISSIONS_ON_LOCAL}`);
    // create user's directory if not present.
    if (!fs.existsSync(directoryPath)) {
        fs.mkdirSync(directoryPath);
    }
    let fileSavePath = `${directoryPath}/${fileName}`;
    let writeStream = fs.createWriteStream(fileSavePath);
    return new Promise((resolve, reject) => {
        writeStream.write(payload.file.buffer);
        writeStream.on('error', function (err) {
            reject(err);
        });
        writeStream.end(function (err) {
            if (err) {
                reject(err);
            } else {
                let fileUrl = pathToUpload ? `${CONFIG.SERVER_URL}${pathOnServer}/${fileName}` : `${CONFIG.SERVER_URL}${CONFIG.PATH_TO_UPLOAD_SUBMISSIONS_ON_LOCAL}/${fileName}`;
                resolve(fileUrl);
            }
        });
    });
};

/**
 * function to upload a file on either local server or on s3 bucket.
 */
fileUploadService.uploadFile = async (payload, pathToUpload, pathOnServer) => {
    const fileNameArray = payload.file.originalname.split('.');
    let fileExtention = fileNameArray[fileNameArray.length - 1] || 'png';
    let fileName = `${fileNameArray.filter(ele => ele != fileExtention).join('_')}_${Date.now()}.${fileExtention}`, fileUrl = '';
    
    if(payload.type == FILE_UPLOAD_TYPE.PROFILE_IMAGE){
        fileName = `profile_${Date.now()}.${fileExtention}`;
    }
    if(payload.type == FILE_UPLOAD_TYPE.CHAT_MEDIA){
        fileName = `${payload.groupId}/chat_${Date.now()}.${fileExtention}`;
    }
    
    if (CONFIG.UPLOAD_TO_S3_BUCKET) {
        fileUrl = await fileUploadService.uploadFileToS3(payload, fileName, CONFIG.S3_BUCKET.bucketName);
    } else {
        fileUrl = await fileUploadService.uploadFileToLocal(payload, fileName, pathToUpload, pathOnServer);
    }
    return fileUrl;
};

/**
 * function to get a file from s3(AWS) bucket.
 */
fileUploadService.getS3File = async (payload, bucketName) => {
    return new Promise((resolve,reject) =>{
        s3Bucket.getObject({ Bucket: bucketName || CONFIG.S3_BUCKET.bucketName, Key: payload.path }, function(err, data) {
            if (err){
                if(err && err.code == "AccessDenied"){
                    resolve({not_found: true});
                }
                else{
                    console.log("S3 file getting error", err);
                    reject(new Error("S3 file getting error"));
                }
            }
            else{
                resolve(data)
            }
        });
    })
};

/**
* function to get link to upload file direct to s3
*/
fileUploadService.getLinkToUploadFileDirectToS3 = async (payload) => {

    let key = (payload.type == FILE_UPLOAD_TYPE.CHAT_MEDIA) ? `${payload.groupId}/chat_${Date.now()}.${fileName}` : `profile_${Date.now()}.${fileName}`;

    const s3Params = {
        Bucket: CONFIG.S3_BUCKET.bucketName,
        Expires: 300,//time to expire in seconds - 5 min
        Fields: {
            Key: key
        }
    }

    const uploadData = await s3Bucket.createPresignedPost(s3Params);
    return uploadData
}

module.exports = fileUploadService;