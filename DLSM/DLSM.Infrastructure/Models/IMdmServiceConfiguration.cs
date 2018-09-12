namespace DLSM.Infrastructure.Models
{
    public interface IMdmServiceConfiguration
    {
        string Endpoints { get; set; }
        string Ip { get; set; }
        string Password { get; set; }
        string Thumprint { get; set; }
        string Uid { get; set; }
    }
}