"""Import the supplied Tom's Help files as UTF-8 Markdown."""
import argparse
from pathlib import Path
import re

KEYS = {30: "Main mode keys", 31: "Item Edit keys", 33: "Pitch input",
        34: "Rhythm input", 35: "Marks input", 36: "Beam input",
        37: "Slur input", 38: "Edit menu keys", 45: "DOS playback setup",
        47: "Disk menu keys"}
ITEMS = ["Notes", "Rests", "Clefs", "Lines, arrows and hairpins", "Slurs, brackets and endings",
         "Beams", "Trills, ottava and pedal marks", "Staves", "Library symbols",
         "Numbers and rehearsal letters", "User drawings", "Shapes and guitar grids",
         "Invisible dummy items", "Barlines", "Included PostScript", "Text",
         "Key signatures", "Time signatures", "Invisible MPUMX spacing items"]
TITLES = {"HELP": "Command and item index from SCORE", "PREF": "SCORE preferences",
          "PRINT": "Printing and EPS export", "Q": "Authoring legacy help",
          "11": "Included PostScript programming supplement"}
NOTES = {
    "HELP": "Use the [SCORE Modern guide](START.md) for current workflows. The original command index below describes DOS SCORE. For item codes 13 and 19, see the individual topics on invisible items.",
    "CODE01": "P5 packs stem direction, accidental, parentheses and horizontal displacement into one value. For example, `13` gives an up-stem with a natural and `123` gives a down-stem with a parenthesized natural. Use the parameter table to identify a field, then read its detailed controls before editing.",
    "CODE05": "In the supplied SCORE executable, integer `P8=10` follows the slur path. The Windows compatibility option **Endings: P8=10 follows Tom’s Help** selects the documented ending interpretation.",
    "CODE09": "Open **Symbol Browser…** from the context menu outside the page to look up symbol numbers by drawing bank. Right-click a symbol to copy its number or open its bank slot in DRAW.",
    "CODE11": "Use **DRAW…** from the context menu outside the page to edit drawing banks. The active drawing LIB takes priority over the installation’s LIB for matching filenames. Named drawings and numbered library symbols have separate item codes.",
    "CODE13": "Earlier SCORE help described this code as reserved. Tom’s topic below specifies its use as an invisible dummy item. SCORE Modern retains the record and does not paint it.",
    "CODE15": "See the [PostScript programming supplement](11.md) for the distinction between a conventional EPS and a program that consumes SCORE parameters. Display depends on the included PostScript settings and supported rendering path.",
    "CODE16": "SCORE font numbers resolve through PSC metrics and the internal PostScript font name. See the [SCORE Modern guide](START.md) for font setup and duplicate versions.",
    "CODE19": "SCORE Modern retains these spacing records without painting them.",
    "CODE30": "F5 playback remains outside the verified SCORE Modern workflow. F6 and F7 can use SCORE menus or the Windows options selected in Preferences.",
    "CODE35": "The score shows note numbers in this stage. Use those numbers to address the notes being marked.",
    "CODE36": "During input, the score shows note numbers for the marks, beams and slurs stages. These numbers identify the items referred to by the stage syntax.",
    "CODE45": "This topic documents DOS playback setup. SCORE Modern’s MIDI input and live rhythm entry are separate workflows from F5 playback.",
    "CODE47": "F7 can use Windows file and folder dialogs when enabled in Preferences. Strict SCORE behaviour uses SCORE prompts.",
    "G": "In SCORE Modern, a relative name such as `G MOZART` resolves from the current working directory. Use `CD` to change it, or **Open…** to browse.",
    "N": "SCORE Modern handles numeric suffixes such as `WANDH01.MUS` and `WANDH02.MUS`. In normal mode, alphabetic names follow Tom’s stem convention. Strict SCORE behaviour uses the recovered executable rules.",
    "P": "See [Printing and EPS export](PRINT.md) and [SCORE preferences](PREF.md).",
    "PREF": "Windows settings live in `ScoreModern.ini` beside the program. The SCORE tab edits preference overrides and preserves the supplied PREF.SCR. Explicit folder selections take priority over mapped SCORE resource paths. DOS drive and directory restrictions below describe the original program.",
    "PRINT": "F6 opens the configured print menu. In the Windows export menu, F5 exports EPS. The SCORE-format menu reads and saves PRINT.SCR and uses its configured output settings.",
    "Q": "The 25-line instructions describe original DOS help authoring. This edition uses UTF-8 Markdown and scrollable topics in the help window.",
    "S": "Leave Item Edit before saving. In Windows prompts, check the destination folder and filename before confirming a save.",
    "V": "`VER` also displays the SCORE Modern version and private build number shown on the title screen.",
    "Z": "The executable defaults retain zoom for bare `Z` and use 2 for bare `ZZ`. Preferences also offers Tom’s defaults of 1 and 3 and lets you choose the framing mode."
}

def title(stem):
    if stem.startswith("CODE"):
        code = int(stem[4:])
        return f"Code {code} · {ITEMS[code - 1]}" if code <= len(ITEMS) else KEYS[code]
    return TITLES.get(stem, f"{stem} commands")

def read_source(path):
    data = path.read_bytes()
    try:
        plain = data.decode("utf-8-sig")
    except UnicodeDecodeError:
        plain = data.decode("cp437")
    return plain.replace("\r\r\n", "\n").replace("\r\n", "\n").replace("\r", "\n")

def clean(text):
    lines = []
    for line in text.split("\n"):
        line = line.rstrip(" \t\x1a")
        if re.fullmatch(r"\s*(?:[*.]|\d{1,2}/\d{1,2}|-{8,}end[- ]*)\s*", line, re.I):
            lines.append("")
            continue
        if re.search(r"[ \t]{2,}\d{1,2}/\d{1,2}$", line):
            line = re.sub(r"[ \t]{2,}\d{1,2}/\d{1,2}$", "", line)
        lines.append(line)
    return lines

def aligned(line):
    plain = line.strip()
    return bool(re.search(r"[│║─━═┌-╿]", plain)
                or re.search(r"\S {3,}\S", plain)
                or re.match(r"(?:[<>]|[+-]?\d+\s+[-+.\d]+)", plain))

def paragraph(lines):
    if not lines:
        return []
    if any(aligned(line) for line in lines) or sum(bool(re.match(r"\s*(?:P\d+|[A-Z]{1,5}|[+-]?\d+)\s*=", line)) for line in lines) > 1:
        indent = min(len(line) - len(line.lstrip()) for line in lines)
        return ["```text", *(line[indent:] for line in lines), "```", ""]
    return [" ".join(line.strip() for line in lines), ""]

def transform(lines):
    result, block = [], []
    index = 0
    while index < len(lines):
        line = lines[index]
        if "╔" in line:
            end = index + 1
            while end < len(lines) and lines[end].strip().startswith("║"):
                end += 1
            if end < len(lines) and lines[end].strip().startswith("╚") and end - index == 3:
                headers, labels = [item.strip().strip("║").split("║") for item in lines[index+1:end]]
                if len(headers) == len(labels) and all(re.fullmatch(r"p\d+", h.strip(), re.I) for h in headers):
                    result.extend(paragraph(block))
                    block.clear()
                    prefix = line.split("╔", 1)[0].strip()
                    if prefix:
                        result.extend([prefix, ""])
                    result.extend(["| Parameter | Meaning |", "| --- | --- |"])
                    for header, label in zip(headers, labels):
                        result.append(f"| `{header.strip().upper()}` | {label.strip().replace('|', '&#124;')} |")
                    result.append("")
                    index = end + 1
                    continue
        if line.strip():
            block.append(line)
        else:
            result.extend(paragraph(block))
            block.clear()
        index += 1
    return result + paragraph(block)

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source", type=Path)
    parser.add_argument("destination", type=Path)
    args = parser.parse_args()
    args.destination.mkdir(parents=True, exist_ok=True)
    topics = sorted(p for p in args.source.iterdir() if p.suffix.lower() == ".hlp")
    if len(topics) != 54:
        raise ValueError(f"Expected 54 Tom's Help topics, found {len(topics)}")
    for path in topics:
        stem = path.stem.upper()
        lines = [f"# {title(stem)}", ""]
        if stem in NOTES:
            lines += ["## In SCORE Modern", "", NOTES[stem], ""]
        credit = "Tom’s Help, edited by Thomas M. Brodhead"
        lines += ["## SCORE reference", "", f"> Source: {credit}.", ""]
        original = read_source(path)
        original = re.sub(r"(?i)\barbitray\b", "arbitrary", original)
        original = re.sub(r"(?i)\bnormal postions\b", "normal positions", original)
        if stem == "PREF":
            original = re.sub(r"LIBRary .*?USE ONLY 3 LETTERS FOR DIRECTORY NAMES\.", "See the SCORE Modern setup guide for library locations.", original, flags=re.S)
        original = original.replace("See also E.HLP.", "See also the E commands.").replace("See S.HLP also.", "See also the S commands.")
        if stem == "HELP":
            original = original.replace("(Reserved for future use.)", "Invisible dummy item.")
        lines += transform(clean(original))
        (args.destination / f"{stem}.md").write_text("\n".join(lines).rstrip()+"\n", encoding="utf-8")

if __name__ == "__main__":
    main()
