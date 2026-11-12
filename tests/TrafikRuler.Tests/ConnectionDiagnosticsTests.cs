using System.Net;
using TrafikRuler.Native;
using TrafikRuler.UI.ViewModels;
namespace TrafikRuler.Tests;
public class ConnectionDiagnosticsTests
{
    [Fact]
    public void SeparatesOwnedLoopbackAndTargetConnections()
    {
        var target = IPAddress.Parse("192.168.4.179");
        TcpConnectionInfo[] rows = [
            new(IPAddress.Loopback, 45000, IPAddress.Loopback, 7897, 5, 10),
            new(target, 45001, IPAddress.Parse("1.1.1.1"), 443, 5, 10),
            new(IPAddress.IPv6Loopback, 45002, IPAddress.IPv6Loopback, 8000, 5, 11),
            new(target, 45003, IPAddress.Parse("1.0.0.1"), 443, 3, 10)];
        var evidence = ConnectionDiagnostics.Inspect(new uint[] { 10 }, rows, target);
        Assert.Equal(new[] { "127.0.0.1:7897" }, evidence.LocalEndpoints);
        Assert.Equal(1, evidence.TargetConnections);
        Assert.Empty(ConnectionDiagnostics.Inspect(new uint[] { 99 }, rows, target).LocalEndpoints);
        Assert.Equal(0, ConnectionDiagnostics.Inspect(new uint[] { 10 }, rows, null).TargetConnections);
    }
}
