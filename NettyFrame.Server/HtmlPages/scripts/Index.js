var indexViewModel = function () {
    const timerDiv = document.getElementById("timer");
    let interval;
    let count = 0;
    function updateTimerTxt() {
        count++;
        if (timerDiv) {
            timerDiv.innerText = count.toString();
        }
    }

    function start() {
        stop();
        interval = setInterval(updateTimerTxt, 1000);
    }

    function stop() {
        if (interval) {
            clearInterval(interval);
        }
    }

    return {
        start,
        stop
    };
}
