using BenchmarkDotNet.Attributes;

namespace Benchmarks;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StaticMethods
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Local
    private class Implementation
    {
        public virtual int Virtual() => 42;
        public int NonStatic() => 42;
        public static int Static() => 42;
    }

    private Implementation _implementation = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _implementation = new Implementation();
    }

    [Benchmark]
    public int NonStatic() => _implementation.NonStatic();

    [Benchmark]
    public int Virtual() => _implementation.Virtual();

    [Benchmark]
    public int Static() => Implementation.Static();
}