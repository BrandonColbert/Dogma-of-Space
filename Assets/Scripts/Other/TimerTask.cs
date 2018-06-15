using UnityEngine;

public class TimerTask {
    private float interval;
    private float lastTime;

    public TimerTask() : this(0) {}

    public TimerTask(float interval) {
        this.interval = interval;
        lastTime = 0;
    }

    public bool Ready() {
        return Ready(interval);
    }

    public bool Ready(float interval) {
        float time = Time.time;

        if(time >= lastTime + interval) {
            lastTime = time;
            return true;
        }

        return false;
    }
}