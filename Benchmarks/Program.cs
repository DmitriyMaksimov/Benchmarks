﻿using BenchmarkDotNet.Running;

BenchmarkRunner.Run<Benchmarks.StaticMethods>();
BenchmarkRunner.Run<Benchmarks.Logging.Logging>();
