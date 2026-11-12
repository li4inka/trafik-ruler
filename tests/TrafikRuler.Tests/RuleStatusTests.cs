using TrafikRuler.Core;
using TrafikRuler.Localization;
using TrafikRuler.UI.ViewModels;

namespace TrafikRuler.Tests;

[Collection("Localization")]
public class RuleStatusTests
{
    [Fact]
    public void StatusTracksCurrentConnectionsInsteadOfHistoricalTraffic()
    {
        Loc.SetLanguage("en");
        var stats = new RuleStats { BytesIn = 1000 };
        var rule = new Rule { ProgramPath = "demo.exe" };
        var view = new RuleView(rule, RuleState.Active, null, stats);
        var card = new RuleCardViewModel(view, _ => { });
        Assert.Equal(Loc.T("StateChecking"), card.StateText);
        card.Processes = 0;
        Assert.Equal(Loc.T("StateNoProcess"), card.StateText);
        card.Processes = 1;
        Assert.Equal(Loc.T("StateAwaitingConnection"), card.StateText);
        stats.ActiveFlows = 1;
        card.Sample();
        Assert.Equal(Loc.T("StateActive"), card.StateText);
        stats.ActiveFlows = 0;
        card.Sample();
        Assert.Equal(Loc.T("StateAwaitingConnection"), card.StateText);
        var missingRule = new Rule { ProgramPath = @"Z:\missing\old-version\demo.exe" };
        card.Update(view with { Rule = missingRule }, false);
        card.Processes = 0;
        Assert.Equal(Loc.T("StatePathMissing"), card.StateText);
        Assert.Contains(missingRule.ProgramPath, card.PathText);
        card.Update(view with { State = RuleState.Blocking }, false);
        Assert.Equal(Loc.T("StateBlocking"), card.StateText);
        card.Update(view with { State = RuleState.Disabled }, true);
        Assert.Equal(Loc.T("StatePaused"), card.StateText);
    }
}
