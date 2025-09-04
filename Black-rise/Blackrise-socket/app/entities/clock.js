

class Time {
	constructor(roomTime) {
		this.currentTime = roomTime;
		this.timeComplete = false;
	}

	timeFinished() {
		this.timeComplete = true;
	}

	startTime() {

		setInterval(() => {

			if (this.currentTime >= 0) {
				this.currentTime--;
			} else {
				this.timeComplete = true;
			}
		}, (1000));
	}

	serialize() {
		return {
			t: this.currentTime,
			c: this.timeComplete
		};
	}
}

module.exports = Time;