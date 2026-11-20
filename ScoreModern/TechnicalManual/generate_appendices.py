"""Generate Markdown source and table appendices from the recovered workspace."""
import argparse
import re
import struct
from pathlib import Path

HERE = Path(__file__).resolve().parent
MODERN = HERE.parent
OUTPUTS = MODERN.parent
RECOVERY = OUTPUTS / "ScoreReverseEngineering"
FRAGMENTS = RECOVERY / "NativeFragments"
WORKSPACE = OUTPUTS.parent


def save(name, sections):
    (HERE / name).write_text("\n".join(sections).rstrip()+"\n", encoding="utf-8")


def rel(path):
    import os
    return Path(os.path.relpath(path, HERE)).as_posix()


def recovered_sources():
    recipe = (FRAGMENTS / "Build-Fragments.ps1").read_text(encoding="utf-8-sig")
    names = set(re.findall(r"'([A-Za-z][A-Za-z0-9]+)'", recipe))
    return sorted(p for p in FRAGMENTS.glob("*.cs") if
                  p.stem in names or (p.stem.startswith(("Layout","Input")) and "Proof" not in p.stem)
                  or p.stem in {"ScanParameterPreparation","ScanP8Conversion","IndependentFlagArithmetic"}
                  or p.stem.endswith("FlagsModel"))


def source_volume(name, label, paths):
    out = [f"# {label}", "", "The listings reproduce the local source verbatim. Source comments state the scope of each model.", ""]
    for p in paths:
        out.extend([f"## {p.stem}", "", f"[Workspace source]({rel(p)})", "",
                    "````csharp", p.read_text(encoding="utf-8-sig").rstrip(), "````", ""])
    save(name, out)


# Mask comments, quoted strings and chars so their braces cannot terminate an initializer.
MASK = re.compile(r'/\*.*?\*/|//[^\r\n]*|@"(?:[^"]|"")*"|"(?:[^"\\]|\\.)*"|\'(?:[^\'\\]|\\.)*\'', re.S)
ARRAY = re.compile(r'\bnew\s*(?:[\w.<>]+\s*)?(?:\[\s*,?\s*\]\s*)+\{|\b\w+\s*=\s*\{')


def initializers(source):
    masked = MASK.sub(lambda m: ''.join('\n' if c=='\n' else ' ' for c in m[0]), source)
    for match in ARRAY.finditer(masked):
        opening = match.end()-1
        depth = 1
        end = opening+1
        while end<len(masked) and depth:
            depth += int(masked[end]=='{')-int(masked[end]=='}')
            end+=1
        if depth:
            raise ValueError("Unbalanced initializer")
        start=match.start()
        yield source.count("\n",0,start)+1, source[start:end]


def dat_tables(dat):
    b=dat.read_bytes()
    if len(b)<10464:
        raise ValueError("Incomplete geometry DAT")
    def floatval(at):
        value,=struct.unpack_from("<f",b,at)
        bits,=struct.unpack_from("<I",b,at)
        return f"{value:.17g} (0x{bits:08X})"
    def word(at):
        return struct.unpack_from("<h",b,at)[0]
    directory=struct.unpack_from("<46H",b,9422)
    assert list(directory)==sorted(directory) and 1<=directory[0]<=directory[-1]<=476
    out=["# External DAT geometry tables","",
         f"Decoded from the local `{dat.name}` resource. These are file offsets and geometry/metric cells. Installation identity data is outside the extracted slices.","",
         "## Circle samples at offset 8","",
         "| Index | X binary32 | Y binary32 |","| ---: | --- | --- |"]
    out += [f"| {i} | {floatval(8+i*8)} | {floatval(12+i*8)} |" for i in range(360)]
    out += ["","The ordinary shape consumer wraps the angle modulo 360. The separate repeat-dot route reads angle 360 directly.",
            "",f"| Literal index 360 | {floatval(8+360*8)} | {floatval(12+360*8)} |",""]
    out += ["## Text metric slice at offset 2896","","| Code | Metric binary32 |","| ---: | --- |"]
    out += [f"| {i} | {floatval(2896+i*4)} |" for i in range(105)]
    out += ["","## Spacing display scale at offset 4794","","| Pair | First signed word | Second signed word |","| ---: | ---: | ---: |"]
    out += [f"| {i} | {word(4794+i*4)} | {word(4796+i*4)} |" for i in range(207)]
    out += ["","## Music glyph directory at offset 9422","","| Glyph / end slot | One-based point address |","| ---: | ---: |"]
    out += [f"| {i} | {value} |" for i,value in enumerate(directory)]
    out += ["","## Music point streams","","X starts at 5622, Y at 7522 and operations at 9514. The point directory limits the live range. Equal start and end addresses give an empty direct stream.","",
            "| One-based point | X binary32 | Y binary32 | Operation word |","| ---: | --- | --- | ---: |"]
    out += [f"| {i} | {floatval(5622+(i-1)*4)} | {floatval(7522+(i-1)*4)} | {word(9514+(i-1)*2)} |" for i in range(directory[0],directory[-1])]
    save("A-dat-tables.md",out)


def main():
    arg=argparse.ArgumentParser(description=__doc__)
    arg.add_argument("dat",type=Path,nargs="?")
    arg.add_argument("--source-only",action="store_true",help="Refresh source listings and initializers; preserve captured and external DAT tables.")
    args=arg.parse_args()
    fragments=recovered_sources()
    integrations=sorted(p for p in MODERN.glob("*.cs") if not p.stem.endswith("Checks"))
    vectors=[p for p in fragments if not p.stem.startswith(("Layout","Input"))]
    layout=[p for p in fragments if p.stem.startswith(("Layout","Input"))]
    engine=[p for p in integrations if p.stem.startswith(("Main","NativePage","NativeJust","NativeScorlas","NativePrint","NativeEps",
            "NativeDat","NativeNote","NativeBeam","NativeSlur","NativeText","NativeMeter","NativeKey","NativeLibr","NativeHairpin",
            "NativeSpacing","NativeMouse","NativeEditCursor","NativeRawArray","NativeNumber","NativeTrill","IncludedPostScript"))
            or p.stem in {"FontconvPscModel","ScoreModern","DrawingLibrary","ScoreText","OriginalMusSave","OriginalDrawingNameRequests"}]
    source_volume("B-layout-and-input-listings.md","Recovered input, spacing and layout routines",layout)
    source_volume("B-machine-and-engraving-listings.md","Recovered machine, geometry and output routines",vectors)
    source_volume("B-native-integration-listings.md","SCORE Modern callers and engraving integration",engine)
    out=["# Appendix B. Routine and evidence index","",
         f"- [Recovered input, spacing and layout listings](B-layout-and-input-listings.md) · {len(layout)} source modules",
         f"- [Recovered machine, geometry and output listings](B-machine-and-engraving-listings.md) · {len(vectors)} source modules",
         f"- [SCORE Modern caller and engraving listings](B-native-integration-listings.md) · {len(engine)} source modules","",
         "## Recovered modules","","| Module | Workspace source |","| --- | --- |"]
    out += [f"| {p.stem} | [Source]({rel(p)}) |" for p in fragments]
    out += ["","## Original recovery notes","","| Evidence set | Notes |","| --- | --- |"]
    evidence=sorted(RECOVERY.glob("*/README.md"))
    out += [f"| {p.parent.name} | [Notes]({rel(p)}) |" for p in evidence]
    save("B-routine-index.md",out)

    out=["# Appendix A. Lookup tables and literal data","",
         "- [Exact source array initializers](A-initializers.md). This inventory preserves original ordering, literal syntax and source locations.",
         "- [Decoded external DAT tables](A-dat-tables.md). Raw float bits and signed word values from the local geometry resource.",
         "- [Captured table evidence](A-captured-tables.md). Machine-readable tables preserved in Markdown code blocks.",
         "- [Routine listings](B-routine-index.md). Review branch literals, inline switch tables and computed or retained tables at their point of use.","",
         "## J compiler workspace","",
         "These offsets initialize the recovered 64 KiB workspace. The four entries at 3882–388E are adjacent DOS descriptors, not ordinary metric values. The standalone JUST path replaces the two specific descriptor words at 3886 and 388E. A decimal interpretation is supplied only for the ordinary binary32 cells.","",
         "| Offset | Raw bits | Binary32 interpretation |","| --- | --- | --- |"]
    workspace=(FRAGMENTS/"LayoutSpacingWorkspace.cs").read_text(encoding="utf-8-sig")
    for bits,addr in re.findall(r"GetBytes\(0x([0-9a-fA-F]{8})u\),0,data,0x([0-9a-fA-F]+),4",workspace):
        value=struct.unpack("<f",struct.pack("<I",int(bits,16)))[0]
        decimal="DOS descriptor" if 0x3882<=int(addr,16)<=0x388E else f"{value:.17g}"
        out.append(f"| 0x{addr} | 0x{bits} | {decimal} |")
    out += ["","JUST replacement bits are `0x00001A94` at `0x3886` and `0x4C2D1A94` at `0x388E`. See the [workspace initializer](../../ScoreReverseEngineering/NativeFragments/LayoutSpacingWorkspace.cs) and [spacing consumer](../../ScoreReverseEngineering/NativeFragments/LayoutItemSpacing.cs).","",
            "## External data that varies with the installation","",
            "| Resource | Located data | Consumer |","| --- | --- | --- |",
            "| DAT | Geometry arrays, glyph directory, metrics and scale vectors | [DAT carrier](../NativeDatGlyphs.cs) |",
            "| LIB drawing banks | Ten slots per directory and signed path tuples | [DRW loader](../DrawingLibrary.cs) |",
            "| PSC and Type 1 | Font assignments, advances, right extents, and outline programs | [FONTCONV](../FontconvPscModel.cs), [text layout](../NativeTextLayout.cs) |",
            "| CODE9.DAT | Text terms selected by the Code 9 term control | [term reader](../NativeCode9Terms.cs) |",
            "| GUITAR.DAT | Named chord entries and fingering data | [grid reader](../NativePrintedChordGrid.cs) |"]
    save("A-lookup-tables.md",out)

    count=0
    out=["# Exact source array initializers","","This mechanical inventory includes both recovered tables and native integration arrays. A source declaration alone does not certify that its values came from DOS. The routine comments and captured evidence establish provenance. Source line numbers apply to the workspace at generation time.",""]
    for p in sorted(fragments+integrations):
        entries=list(initializers(p.read_text(encoding="utf-8-sig")))
        if not entries:continue
        out += [f"## {p.name}","","[Workspace source]("+rel(p)+")",""]
        for line,code in entries:
            count+=1
            out += [f"Line {line}","","````csharp",code,"````",""]
    save("A-initializers.md",out)

    if args.source_only:
        print(f"Refreshed {len(fragments)} recovered modules and {len(engine)} integration listings; external tables preserved")
        return
    if args.dat is None:
        arg.error("DAT path required unless --source-only is used")

    selected=[
        RECOVERY/"CallbackAnalysis/callback-tables-evidence.json",
        RECOVERY/"FontconvAnalysis/decoded-constants.json",
        RECOVERY/"DatLineBatch/dat-wave-table-provenance.json",
        RECOVERY/"DatLineBatch/dat-line-table-stability.json",
        RECOVERY/"SlurTieOriginsAnalysis/offset-tables.json",
        RECOVERY/"SlurTieOriginsAnalysis/literal-constants.json",
        WORKSPACE/"work/step5-layout/original/justify/rest-spacing/compiler-constants.json"
    ]
    out=["# Captured table evidence","","These blocks preserve the selected original extraction or comparison records verbatim. Read each record's address space and scope before using it as a numeric reference.",""]
    for p in selected:
        out += [f"## {p.parent.name} · {p.name}","",f"[Workspace evidence]({rel(p)})","","````json",p.read_text(encoding="utf-8-sig").rstrip(),"````",""]
    save("A-captured-tables.md",out)
    dat_tables(args.dat)
    print(f"{len(fragments)} recovered modules, {len(engine)} integration modules, {count} initializer entries")


if __name__=="__main__":
    main()
