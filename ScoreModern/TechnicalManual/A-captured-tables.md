# Captured table evidence

These blocks preserve the selected original extraction or comparison records verbatim. Read each record's address space and scope before using it as a numeric reference.

## CallbackAnalysis · callback-tables-evidence.json

[Workspace evidence](../../ScoreReverseEngineering/CallbackAnalysis/callback-tables-evidence.json)

````json
[
  {
    "routine": "callback",
    "table": "0x3a0",
    "count": 5,
    "targets": [
      "0268",
      "026c",
      "02c0",
      "0326",
      "0326"
    ],
    "distinct_targets": 4,
    "runtime_bytes_match": 10
  },
  {
    "routine": "data-helper",
    "table": "0x39c4",
    "count": 30,
    "targets": [
      "37c0",
      "37e3",
      "3693",
      "3693",
      "399c",
      "399c",
      "399c",
      "399c",
      "399c",
      "399c",
      "399c",
      "399c",
      "38eb",
      "3693",
      "3693",
      "3693",
      "396c",
      "38c2",
      "3693",
      "38a2",
      "38aa",
      "38ba",
      "38b2",
      "38ce",
      "3919",
      "37ec",
      "37ec",
      "37ec",
      "394f",
      "372c"
    ],
    "distinct_targets": 16,
    "runtime_bytes_match": 60
  }
]
````

## FontconvAnalysis · decoded-constants.json

[Workspace evidence](../../ScoreReverseEngineering/FontconvAnalysis/decoded-constants.json)

````json
{
  "dataSegment": "0x1222",
  "specialPrefixes": [
    "Ari",
    "Ati",
    "Nti",
    "Oti",
    "Sca",
    "Zca",
    "ari",
    "ati",
    "cop",
    "min",
    "nti",
    "oti",
    "reg",
    "sca",
    "tra",
    "zca"
  ],
  "specialSlots": [
    112,
    97,
    99,
    101,
    114,
    115,
    116,
    96,
    150,
    138,
    98,
    100,
    155,
    130,
    157,
    132
  ],
  "code32Through251": [
    95,
    70,
    79,
    133,
    134,
    135,
    136,
    77,
    39,
    40,
    73,
    71,
    38,
    72,
    37,
    74,
    1,
    2,
    3,
    4,
    5,
    6,
    7,
    8,
    9,
    10,
    67,
    68,
    139,
    75,
    140,
    69,
    141,
    11,
    12,
    13,
    14,
    15,
    16,
    17,
    18,
    19,
    20,
    21,
    22,
    23,
    24,
    25,
    26,
    27,
    28,
    29,
    30,
    31,
    32,
    33,
    34,
    35,
    36,
    146,
    147,
    148,
    163,
    76,
    137,
    41,
    42,
    43,
    44,
    45,
    46,
    47,
    48,
    49,
    50,
    51,
    52,
    53,
    54,
    55,
    56,
    57,
    58,
    59,
    60,
    61,
    62,
    63,
    64,
    65,
    66,
    158,
    159,
    160,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    105,
    106,
    107,
    163,
    131,
    119,
    108,
    109,
    110,
    111,
    120,
    123,
    124,
    122,
    125,
    163,
    127,
    117,
    113,
    163,
    163,
    128,
    102,
    163,
    103,
    104,
    121,
    118,
    163,
    163,
    129,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    126,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    163,
    142,
    163,
    161,
    163,
    163,
    163,
    163,
    144,
    145,
    143,
    162,
    163,
    163,
    163,
    163,
    163,
    149,
    163,
    163,
    163,
    152,
    163,
    163,
    153,
    154,
    151,
    156
  ]
}
````

## DatLineBatch · dat-wave-table-provenance.json

[Workspace evidence](../../ScoreReverseEngineering/DatLineBatch/dat-wave-table-provenance.json)

````json
{
  "source_sha256": "1a21a97099b53a2b4a1a02fb19a9edee75ef6c7fe37bce71c9886222cb74d781",
  "x_offset": 168138,
  "y_runtime_offset": 1170,
  "y_compiled_nonzero_run_offset": 168393,
  "words_per_table": 14,
  "scope": "Original compiled waveform constants; equality verified against accepted runtime DS capture; no external LIB glyph data embedded."
}
````

## DatLineBatch · dat-line-table-stability.json

[Workspace evidence](../../ScoreReverseEngineering/DatLineBatch/dat-line-table-stability.json)

````json
{
  "original_snapshots": 83,
  "bytes_per_table": 56,
  "identical": true,
  "scope": "Original runtime constants remain identical across all accepted item captures"
}
````

## SlurTieOriginsAnalysis · offset-tables.json

[Workspace evidence](../../ScoreReverseEngineering/SlurTieOriginsAnalysis/offset-tables.json)

````json
{
  "left": [
    2.664989419452937e-41,
    0.0,
    9.0,
    27.0,
    36.0,
    18.0,
    9.0,
    18.0,
    9.0,
    0.0,
    0.0,
    17.700000762939453
  ],
  "right": [
    12.600000381469727,
    0.0,
    9.0,
    -6.0,
    -6.0,
    14.0,
    14.0,
    9.0,
    0.0,
    9.0,
    14.0,
    -4.0
  ]
}
````

## SlurTieOriginsAnalysis · literal-constants.json

[Workspace evidence](../../ScoreReverseEngineering/SlurTieOriginsAnalysis/literal-constants.json)

````json
{
  "6C48": 0.5,
  "6C4C": 7.0,
  "6C52": 36.0,
  "6C56": 18.0,
  "6C5A": 3.0,
  "6C5E": 150.0,
  "6C64": 2.0
}
````

## rest-spacing · compiler-constants.json

[Workspace evidence](../../../work/step5-layout/original/justify/rest-spacing/compiler-constants.json)

````json
{
  "source": "work/score-unpacked/SCORE-MAIN.bin",
  "sha256": "392b955d45cfe0310894035e679537e35e12e224da40b0ebace6b4cc42d8e9d4",
  "all_runtime_values_equal_original_initial_data": true,
  "constants": [
    {
      "ds_offset": "0x77e0",
      "unpacked_offset": "0x86780",
      "bits": "0000803f",
      "value": 1.0
    },
    {
      "ds_offset": "0x77e6",
      "unpacked_offset": "0x86786",
      "bits": "00009040",
      "value": 4.5
    },
    {
      "ds_offset": "0x77ea",
      "unpacked_offset": "0x8678a",
      "bits": "00000000",
      "value": 0.0
    },
    {
      "ds_offset": "0x77ee",
      "unpacked_offset": "0x8678e",
      "bits": "0000003f",
      "value": 0.5
    },
    {
      "ds_offset": "0x77f2",
      "unpacked_offset": "0x86792",
      "bits": "0000c040",
      "value": 6.0
    },
    {
      "ds_offset": "0x77f6",
      "unpacked_offset": "0x86796",
      "bits": "000080c0",
      "value": -4.0
    },
    {
      "ds_offset": "0x77fa",
      "unpacked_offset": "0x8679a",
      "bits": "00001841",
      "value": 9.5
    },
    {
      "ds_offset": "0x77fe",
      "unpacked_offset": "0x8679e",
      "bits": "0000e03f",
      "value": 1.75
    },
    {
      "ds_offset": "0x7802",
      "unpacked_offset": "0x867a2",
      "bits": "0000c842",
      "value": 100.0
    },
    {
      "ds_offset": "0x7806",
      "unpacked_offset": "0x867a6",
      "bits": "9a99193f",
      "value": 0.6000000238418579
    }
  ]
}
````
