using UnityEngine;

public class TimerTask {
    private float interval;
    private float lastTime;

    public TimerTask() : this(0) {}

    public TimerTask(float interval) {
        Interval(interval);
        Set(0);
    }

    public bool Next() {
        return Next(interval);
    }

    public bool Next(float interval) {
        this.interval = interval;
        float time = Time.time;
        
        bool ready = Ready(time);
        if(ready) Set(time);

        return ready;
    }

    public bool Ready() {
        return Ready(Time.time);
    }

    public bool Ready(float time) {
        return time >= lastTime + interval;
    }

    public TimerTask Interval(float interval) {
        this.interval = interval;
        return this;
    }

    public void Set(float time) {
        lastTime = time;
    }

    public float Till() {
        return (lastTime + interval) - Time.time;
    }
}