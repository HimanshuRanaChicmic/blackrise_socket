const Time = require('./clock');
const CONSTANTS = require('../utils/constants');
const MODELS = require('../models');

const { getRandomInteger } = require('../utils/utils');
const { dbService } = require('../services');

class Game {
	constructor(roomId, roomTime) {

		this.clock = new Time(roomTime);
		// this.crash = new Crash();
		this.allIntervals = [];
		this.isGameCompleted = false;

		this.roomId = roomId;
		this.crashValue = 1;
		this.currentValue = 0; 
		this.sockets = {};

		this.allIntervals.push(setInterval(this.sendGameUpdates.bind(this), 1000 / 2));
	}

	startWaitingTimer(roomTime) {
		this.clock.startTime();
		setTimeout(async () => {
			const randomNumber = await getRandomInteger(110, 300);
			this.clock.timeFinished(true);

			console.log(randomNumber / 100);
            
			this.crashValue = randomNumber / 100;
			// this.crash.startTime(randomNumber / 100);

			await dbService.findOneAndUpdate(MODELS.crashGameModel, { _id: this.roomId }, { status: CONSTANTS.CRASH_GAME_STATUS.GAMEPLAY });
		
		}, roomTime * 1000);
	}

	sendGameUpdates() {

		if (!this.clock.timeComplete) { 
			console.log( this.clock);
			io.emit(CONSTANTS.SOCKET_EVENTS.TIMER, this.clock);
		} else {
			if (!this.isGameCompleted) {
			
				console.log(this.currentValue, this.crashValue);	
			}
			
			this.currentValue = this.currentValue + 0.01;
			if (this.currentValue >= this.crashValue && !this.isGameCompleted) {

				this.isGameCompleted = true;
				setTimeout(async () => {

					const players = await dbService.find(MODELS.crashGamePlayersModel, { gameId: this.roomId });

					for (const playerData of players) {
						if (playerData.cashOut <= this.crashValue) {
							await dbService.findOneAndUpdate(MODELS.crashGamePlayersModel, { userId: playerData.userId, gameId: this.roomId }, { isWinner: true });
						}
					
					}
					
					await dbService.findOneAndUpdate(MODELS.crashGameModel, { _id: this.roomId }, { status: CONSTANTS.CRASH_GAME_STATUS.FINISHED, crashValue: this.crashValue });
					const gameData = await new MODELS.crashGameModel({ status: CONSTANTS.CRASH_GAME_STATUS.WAITING_ROOM }).save();

					const newGame = new Game(gameData._id, CONSTANTS.WAITING_TIMER);

					newGame.startWaitingTimer(CONSTANTS.WAITING_TIMER);
				}, 1000);

			}
		}

	}
}

module.exports = Game;
