# csharp-offshore-load-shed-optimizer

Select nonessential loads to shed with an exact capacity-constrained search while preserving protected consumers.

```sh
dotnet run -- --self-test
dotnet run < examples/input.txt
```

Offline engineering analysis. Installation-specific limits and source records determine the result; this program does not operate equipment.

MIT. Built by [brunnodev](https://brunnodev.store).

## Execution proof

[![Executable proof](https://github.com/brunnojob/csharp-offshore-load-shed-optimizer/actions/workflows/proof.yml/badge.svg)](https://github.com/brunnojob/csharp-offshore-load-shed-optimizer/actions/workflows/proof.yml)

![Recorded program execution](docs/proof/execution.svg)

[Verified run](https://github.com/brunnojob/csharp-offshore-load-shed-optimizer/actions/runs/38017614436) · [Execution report](docs/proof/evidence.json)

Run `python .proof/record.py` after installing the prerequisites above. The scenarios execute repository code and verify exit codes and expected output. CI publishes `execution-proof` with the transcript, input fingerprints and source commit. The downloadable report identifies the exact tested version; the workflow badge tracks the latest run.
