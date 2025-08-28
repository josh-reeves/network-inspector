namespace NetworkInspector.Interfaces;

public interface IPort
{
    public enum Protocols
    {
        TCP,
        UDP
    }

    public int PortNumber { get; set; }
    
    public Protocols Protocol { get; set; }

}