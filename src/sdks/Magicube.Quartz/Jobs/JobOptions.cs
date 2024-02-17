namespace Magicube.Quartz.Jobs {
    public class JobOptions {
        public int RetryTimes { get; set; } = 3;
        public int WaitSecond { get; set; } = 5;
    }
}
