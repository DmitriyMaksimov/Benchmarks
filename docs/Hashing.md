# Hashing

Compare performance of hashing algorithms.
- SHA256
- SHA384
- SHA512
- xxHash3
- xxHash32
- xxHash64
- xxHash128


## Results

```
BenchmarkDotNet v0.14.0, Ubuntu 25.04 (Plucky Puffin)
AMD Ryzen 5 8600G w/ Radeon 760M Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.109
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-CUQAJI : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Server=False  
```

```
| Method         | DataLength | Mean          | Ratio    | Gen0   | Allocated | Alloc Ratio |
|--------------- |----------- |--------------:|---------:|-------:|----------:|------------:|
| SHA256_Hash    | 16         |    404.495 ns |  +5,826% | 0.0067 |      56 B |        +40% |
| SHA384_Hash    | 16         |    529.346 ns |  +7,655% | 0.0086 |      72 B |        +80% |
| SHA512_Hash    | 16         |    528.810 ns |  +7,647% | 0.0105 |      88 B |       +120% |
| XXHash3_Hash   | 16         |      5.317 ns |     -22% | 0.0038 |      32 B |        -20% |
| XXHash32_Hash  | 16         |      6.573 ns |      -4% | 0.0038 |      32 B |        -20% |
| XXHash64_Hash  | 16         |      8.732 ns |     +28% | 0.0038 |      32 B |        -20% |
| XXHash128_Hash | 16         |      6.826 ns | baseline | 0.0048 |      40 B |             |
|                |            |               |          |        |           |             |
| SHA256_Hash    | 64         |    419.927 ns |  +4,337% | 0.0067 |      56 B |        +40% |
| SHA384_Hash    | 64         |    526.934 ns |  +5,467% | 0.0086 |      72 B |        +80% |
| SHA512_Hash    | 64         |    598.298 ns |  +6,221% | 0.0105 |      88 B |       +120% |
| XXHash3_Hash   | 64         |      7.681 ns |     -19% | 0.0038 |      32 B |        -20% |
| XXHash32_Hash  | 64         |     10.768 ns |     +14% | 0.0038 |      32 B |        -20% |
| XXHash64_Hash  | 64         |     15.447 ns |     +63% | 0.0038 |      32 B |        -20% |
| XXHash128_Hash | 64         |      9.465 ns | baseline | 0.0048 |      40 B |             |
|                |            |               |          |        |           |             |
| SHA256_Hash    | 256        |    498.319 ns |  +2,111% | 0.0067 |      56 B |        +40% |
| SHA384_Hash    | 256        |    798.109 ns |  +3,442% | 0.0086 |      72 B |        +80% |
| SHA512_Hash    | 256        |    799.853 ns |  +3,450% | 0.0105 |      88 B |       +120% |
| XXHash3_Hash   | 256        |     15.343 ns |     -32% | 0.0038 |      32 B |        -20% |
| XXHash32_Hash  | 256        |     30.300 ns |     +34% | 0.0038 |      32 B |        -20% |
| XXHash64_Hash  | 256        |     28.593 ns |     +27% | 0.0038 |      32 B |        -20% |
| XXHash128_Hash | 256        |     22.535 ns | baseline | 0.0048 |      40 B |             |
|                |            |               |          |        |           |             |
| SHA256_Hash    | 1024       |    807.616 ns |  +2,288% | 0.0067 |      56 B |        +40% |
| SHA384_Hash    | 1024       |  1,447.259 ns |  +4,180% | 0.0076 |      72 B |        +80% |
| SHA512_Hash    | 1024       |  1,402.562 ns |  +4,048% | 0.0095 |      88 B |       +120% |
| XXHash3_Hash   | 1024       |     26.087 ns |     -23% | 0.0038 |      32 B |        -20% |
| XXHash32_Hash  | 1024       |    107.918 ns |    +219% | 0.0038 |      32 B |        -20% |
| XXHash64_Hash  | 1024       |     74.509 ns |    +120% | 0.0038 |      32 B |        -20% |
| XXHash128_Hash | 1024       |     33.816 ns | baseline | 0.0048 |      40 B |             |
|                |            |               |          |        |           |             |
| SHA256_Hash    | 4096       |  2,066.933 ns |  +2,262% | 0.0038 |      56 B |        +40% |
| SHA384_Hash    | 4096       |  4,029.760 ns |  +4,505% | 0.0076 |      72 B |        +80% |
| SHA512_Hash    | 4096       |  4,027.082 ns |  +4,502% | 0.0076 |      88 B |       +120% |
| XXHash3_Hash   | 4096       |     68.829 ns |     -21% | 0.0038 |      32 B |        -20% |
| XXHash32_Hash  | 4096       |    420.423 ns |    +380% | 0.0038 |      32 B |        -20% |
| XXHash64_Hash  | 4096       |    259.803 ns |    +197% | 0.0038 |      32 B |        -20% |
| XXHash128_Hash | 4096       |     87.511 ns | baseline | 0.0048 |      40 B |             |
|                |            |               |          |        |           |             |
| SHA256_Hash    | 16384      |  7,409.923 ns |  +2,733% |      - |      56 B |        +40% |
| SHA384_Hash    | 16384      | 14,954.881 ns |  +5,618% |      - |      72 B |        +80% |
| SHA512_Hash    | 16384      | 14,819.415 ns |  +5,566% |      - |      88 B |       +120% |
| XXHash3_Hash   | 16384      |    254.137 ns |      -3% | 0.0038 |      32 B |        -20% |
| XXHash32_Hash  | 16384      |  1,717.854 ns |    +557% | 0.0038 |      32 B |        -20% |
| XXHash64_Hash  | 16384      |    998.470 ns |    +282% | 0.0038 |      32 B |        -20% |
| XXHash128_Hash | 16384      |    261.528 ns | baseline | 0.0048 |      40 B |             |
```

## Analysis
- `xxHash` family dominates performance. For most sizes, `xxHash3` and `xxHash128` are the fastest; xxHash3 often slightly edges out xxHash128 on small/medium inputs, while xxHash128 is the chosen baseline and remains extremely fast across all sizes. `xxHash64/32` are slower than `xxHash3/128`, with `xxHash32` falling behind markedly as input grows.
- Cryptographic hashes `SHA-256/384/512` are orders of magnitude slower than `xxHash` variants at every size; `SHA-256` is the quickest among them for small inputs, while `SHA-384/512` track closely and scale similarly.
- Allocations are tiny across the board; `xxHash` variants allocate 32–40 B, `SHAs` 56–88 B.

## Conclusions
- For **non-cryptographic** hashing, prefer `xxHash3` or `xxHash128`; they deliver the best overall throughput across sizes.
- Use `SHA` only when cryptographic integrity is required.
- `xxHash64` is acceptable but slower than `xxHash3/128`; `xxHash32` is not recommended for larger inputs.
