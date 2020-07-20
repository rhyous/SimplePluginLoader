namespace Tool.DependencyInjection
{
    public interface IScopedSetting { int id { get; } }

    public class ScopedSetting1 : IScopedSetting { public int id => 1; }

    public class ScopedSetting2 : IScopedSetting { public int id => 2; }
}
