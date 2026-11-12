using System.Net;
using TrafikRuler.Native;

namespace TrafikRuler.UI.ViewModels;

internal sealed record ConnectionEvidence(string[] LocalEndpoints, int TargetConnections);

internal static class ConnectionDiagnostics
{
    public static ConnectionEvidence Inspect(IReadOnlyCollection<uint> pids,
        IEnumerable<TcpConnectionInfo> connections, IPAddress? target)
    {
        var owned = connections.Where(c => pids.Contains(c.Pid) && c.State == IpHelper.MIB_TCP_STATE_ESTAB).ToArray();
        return new(
            owned.Where(c => IPAddress.IsLoopback(c.RemoteAddress))
                .Select(c => new IPEndPoint(c.RemoteAddress, c.RemotePort).ToString()).Distinct().Order().ToArray(),
            target == null ? 0 : owned.Count(c => !IPAddress.IsLoopback(c.RemoteAddress) && c.LocalAddress.Equals(target)));
    }
}
