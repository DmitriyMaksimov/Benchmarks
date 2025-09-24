# Hashing

Compare performance of hashing algorithms.
- SHA256
- SHA384
- SHA512
- SHA3_256
- SHA3_384
- SHA3_512
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
| Method         | DataLength | Mean          | Median        | Ratio    | Gen0   | Allocated | Alloc Ratio |
|--------------- |----------- |--------------:|--------------:|---------:|-------:|----------:|------------:|
| SHA256_Hash    | 16         |    427.132 ns |    427.073 ns |  +6,962% | 0.0067 |      56 B |        +75% |
| SHA3_256_Hash  | 16         |    746.417 ns |    745.650 ns | +12,240% | 0.0067 |      56 B |        +75% |
| SHA384_Hash    | 16         |    624.403 ns |    622.865 ns | +10,223% | 0.0086 |      72 B |       +125% |
| SHA3_384_Hash  | 16         |    748.352 ns |    747.908 ns | +12,272% | 0.0086 |      72 B |       +125% |
| SHA512_Hash    | 16         |    574.492 ns |    574.984 ns |  +9,398% | 0.0105 |      88 B |       +175% |
| SHA3_512_Hash  | 16         |    676.873 ns |    676.239 ns | +11,090% | 0.0105 |      88 B |       +175% |
| XXHash3_Hash   | 16         |      6.073 ns |      6.257 ns | baseline | 0.0038 |      32 B |             |
| XXHash32_Hash  | 16         |      6.730 ns |      6.736 ns |     +11% | 0.0038 |      32 B |         +0% |
| XXHash64_Hash  | 16         |      9.948 ns |      9.961 ns |     +64% | 0.0038 |      32 B |         +0% |
| XXHash128_Hash | 16         |      7.187 ns |      7.205 ns |     +19% | 0.0048 |      40 B |        +25% |
|                |            |               |               |          |        |           |             |
| SHA256_Hash    | 64         |    499.810 ns |    499.160 ns |  +6,241% | 0.0067 |      56 B |        +75% |
| SHA3_256_Hash  | 64         |    730.711 ns |    730.869 ns |  +9,170% | 0.0067 |      56 B |        +75% |
| SHA384_Hash    | 64         |    547.997 ns |    547.497 ns |  +6,852% | 0.0086 |      72 B |       +125% |
| SHA3_384_Hash  | 64         |    662.118 ns |    660.840 ns |  +8,300% | 0.0086 |      72 B |       +125% |
| SHA512_Hash    | 64         |    540.671 ns |    540.481 ns |  +6,759% | 0.0105 |      88 B |       +175% |
| SHA3_512_Hash  | 64         |    652.331 ns |    651.717 ns |  +8,176% | 0.0105 |      88 B |       +175% |
| XXHash3_Hash   | 64         |      7.883 ns |      7.874 ns | baseline | 0.0038 |      32 B |             |
| XXHash32_Hash  | 64         |     11.341 ns |     11.347 ns |     +44% | 0.0038 |      32 B |         +0% |
| XXHash64_Hash  | 64         |     14.730 ns |     14.735 ns |     +87% | 0.0038 |      32 B |         +0% |
| XXHash128_Hash | 64         |      9.778 ns |      9.785 ns |     +24% | 0.0048 |      40 B |        +25% |
|                |            |               |               |          |        |           |             |
| SHA256_Hash    | 256        |    510.053 ns |    509.878 ns |  +3,053% | 0.0067 |      56 B |        +75% |
| SHA3_256_Hash  | 256        |    923.121 ns |    922.383 ns |  +5,606% | 0.0067 |      56 B |        +75% |
| SHA384_Hash    | 256        |    756.970 ns |    757.151 ns |  +4,579% | 0.0086 |      72 B |       +125% |
| SHA3_384_Hash  | 256        |  1,125.249 ns |  1,126.106 ns |  +6,856% | 0.0076 |      72 B |       +125% |
| SHA512_Hash    | 256        |    825.051 ns |    825.281 ns |  +5,000% | 0.0105 |      88 B |       +175% |
| SHA3_512_Hash  | 256        |  1,342.695 ns |  1,342.520 ns |  +8,200% | 0.0095 |      88 B |       +175% |
| XXHash3_Hash   | 256        |     16.178 ns |     16.195 ns | baseline | 0.0038 |      32 B |             |
| XXHash32_Hash  | 256        |     31.085 ns |     31.100 ns |     +92% | 0.0038 |      32 B |         +0% |
| XXHash64_Hash  | 256        |     29.238 ns |     29.172 ns |     +81% | 0.0038 |      32 B |         +0% |
| XXHash128_Hash | 256        |     23.233 ns |     23.183 ns |     +44% | 0.0048 |      40 B |        +25% |
|                |            |               |               |          |        |           |             |
| SHA256_Hash    | 1024       |    840.679 ns |    839.523 ns |  +3,135% | 0.0067 |      56 B |        +75% |
| SHA3_256_Hash  | 1024       |  2,097.994 ns |  2,095.120 ns |  +7,973% | 0.0038 |      56 B |        +75% |
| SHA384_Hash    | 1024       |  1,437.034 ns |  1,439.302 ns |  +5,429% | 0.0076 |      72 B |       +125% |
| SHA3_384_Hash  | 1024       |  2,509.749 ns |  2,508.906 ns |  +9,557% | 0.0076 |      72 B |       +125% |
| SHA512_Hash    | 1024       |  1,462.950 ns |  1,459.699 ns |  +5,529% | 0.0095 |      88 B |       +175% |
| SHA3_512_Hash  | 1024       |  3,486.286 ns |  3,487.674 ns | +13,314% | 0.0076 |      88 B |       +175% |
| XXHash3_Hash   | 1024       |     25.990 ns |     25.916 ns | baseline | 0.0038 |      32 B |             |
| XXHash32_Hash  | 1024       |    110.778 ns |    110.748 ns |    +326% | 0.0038 |      32 B |         +0% |
| XXHash64_Hash  | 1024       |     73.116 ns |     73.039 ns |    +181% | 0.0038 |      32 B |         +0% |
| XXHash128_Hash | 1024       |     33.772 ns |     33.709 ns |     +30% | 0.0048 |      40 B |        +25% |
|                |            |               |               |          |        |           |             |
| SHA256_Hash    | 4096       |  2,166.550 ns |  2,167.163 ns |  +2,970% | 0.0038 |      56 B |        +75% |
| SHA3_256_Hash  | 4096       |  6,845.434 ns |  6,840.005 ns |  +9,600% |      - |      56 B |        +75% |
| SHA384_Hash    | 4096       |  4,059.270 ns |  4,058.648 ns |  +5,652% | 0.0076 |      72 B |       +125% |
| SHA3_384_Hash  | 4096       |  8,613.059 ns |  8,627.349 ns | +12,105% |      - |      72 B |       +125% |
| SHA512_Hash    | 4096       |  4,084.150 ns |  4,080.470 ns |  +5,687% | 0.0076 |      88 B |       +175% |
| SHA3_512_Hash  | 4096       | 11,900.160 ns | 11,907.786 ns | +16,762% |      - |      88 B |       +175% |
| XXHash3_Hash   | 4096       |     70.575 ns |     70.518 ns | baseline | 0.0038 |      32 B |             |
| XXHash32_Hash  | 4096       |    427.204 ns |    427.211 ns |    +505% | 0.0038 |      32 B |         +0% |
| XXHash64_Hash  | 4096       |    259.457 ns |    259.433 ns |    +268% | 0.0038 |      32 B |         +0% |
| XXHash128_Hash | 4096       |     78.855 ns |     78.778 ns |     +12% | 0.0048 |      40 B |        +25% |
|                |            |               |               |          |        |           |             |
| SHA256_Hash    | 16384      |  7,252.406 ns |  7,250.405 ns |  +2,715% |      - |      56 B |        +75% |
| SHA3_256_Hash  | 16384      | 25,175.975 ns | 25,132.856 ns |  +9,673% |      - |      56 B |        +75% |
| SHA384_Hash    | 16384      | 14,905.958 ns | 14,934.133 ns |  +5,686% |      - |      72 B |       +125% |
| SHA3_384_Hash  | 16384      | 33,106.578 ns | 33,069.699 ns | +12,751% |      - |      72 B |       +125% |
| SHA512_Hash    | 16384      | 14,924.352 ns | 14,913.343 ns |  +5,693% |      - |      88 B |       +175% |
| SHA3_512_Hash  | 16384      | 47,157.042 ns | 47,138.544 ns | +18,205% |      - |      88 B |       +175% |
| XXHash3_Hash   | 16384      |    257.621 ns |    257.354 ns | baseline | 0.0038 |      32 B |             |
| XXHash32_Hash  | 16384      |  1,728.072 ns |  1,729.843 ns |    +571% | 0.0038 |      32 B |         +0% |
| XXHash64_Hash  | 16384      |  1,005.517 ns |  1,004.400 ns |    +290% | 0.0038 |      32 B |         +0% |
| XXHash128_Hash | 16384      |    272.182 ns |    272.450 ns |      +6% | 0.0048 |      40 B |        +25% |
```

## Analysis
- `xxHash` family dominates performance. For most sizes, `xxHash3` and `xxHash128` are the fastest; xxHash3 often slightly edges out xxHash128 on small/medium inputs, while xxHash128 is the chosen baseline and remains extremely fast across all sizes. `xxHash64/32` are slower than `xxHash3/128`, with `xxHash32` falling behind markedly as input grows.
- Cryptographic hashes `SHA-256/384/512` are orders of magnitude slower than `xxHash` variants at every size; `SHA-256` is the quickest among them for small inputs, while `SHA-384/512` track closely and scale similarly.
- Across all sizes, `SHA-2` (256/384/512) is consistently and substantially faster than `SHA-3`, often by 20-60% and growing with input size.
  Example trends: at 4096–16384 bytes, `SHA-3` variants are roughly 2–3 times slower than their `SHA-2` counterparts.
- Allocations are tiny across the board; `xxHash` variants allocate 32–40 B, `SHAs` 56–88 B.

## Conclusions
- For **non-cryptographic** hashing, prefer `xxHash3` or `xxHash128`; they deliver the best overall throughput across sizes.
- Use `SHA` only when cryptographic integrity is required.
- `xxHash64` is acceptable but slower than `xxHash3/128`; `xxHash32` is not recommended for larger inputs.
