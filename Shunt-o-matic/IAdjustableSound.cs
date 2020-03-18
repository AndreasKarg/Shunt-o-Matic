namespace Shunt_o_matic
{
    public interface IAdjustableSound
    {
        float Volume { get; set; }
        double PlaybackSpeed { get; set; }
    }
}