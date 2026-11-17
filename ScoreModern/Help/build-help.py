"""Build the offline help site from the Markdown topics using Python's standard library."""
from html import escape
import json
from pathlib import Path
import re
import shutil
import sys


def inline(text):
    pieces = re.split(r"(`[^`]+`|\[[^\]]+\]\([A-Z0-9]+\.md\)|\*\*[^*]+\*\*)", text)
    result = []
    for piece in pieces:
        link = re.fullmatch(r"\[([^\]]+)\]\(([A-Z0-9]+)\.md\)", piece)
        if link:
            label, target = link.groups()
            filename = "index" if target == "START" else target
            result.append(f'<a href="{filename}.html">{escape(label)}</a>')
        elif piece.startswith("`") and piece.endswith("`") and len(piece) > 1:
            result.append(f"<code>{escape(piece[1:-1])}</code>")
        elif piece.startswith("**") and piece.endswith("**") and len(piece) > 4:
            result.append(f"<strong>{escape(piece[2:-2])}</strong>")
        else:
            result.append(escape(piece))
    return "".join(result)


def render(text):
    lines = iter(text.splitlines())
    result = []
    for line in lines:
        if not line:
            continue
        if line.startswith("```"):
            code = []
            for following in lines:
                if following == "```":
                    break
                code.append(following)
            result.append("<pre><code>" + escape("\n".join(code)) + "</code></pre>")
        elif line.startswith("| "):
            def row(value, tag):
                return "<tr>" + "".join(f"<{tag}>{inline(cell.strip())}</{tag}>" for cell in value.strip("|").split("|")) + "</tr>"
            next(lines)
            rows = []
            for following in lines:
                if not following.startswith("| "):
                    break
                rows.append(row(following, "td"))
            result.append('<div class="table"><table><thead>' + row(line, "th") + "</thead><tbody>" + "".join(rows) + "</tbody></table></div>")
        elif line.startswith("#"):
            prefix, label = line.split(" ", 1)
            result.append(f"<h{len(prefix)}>{inline(label)}</h{len(prefix)}>")
        elif line.startswith("> "):
            result.append(f"<blockquote>{inline(line[2:])}</blockquote>")
        elif re.match(r"(?:- |\d+\. )", line):
            result.append(f'<p class="step">{inline(line)}</p>')
        else:
            result.append(f"<p>{inline(line)}</p>")
    return "\n".join(result)


def category(stem):
    if stem in {"SETUP", "WORKFLOW", "VIEW", "RESOURCES"}:
        return 4
    if re.fullmatch(r"CODE\d{2}", stem):
        return 0 if int(stem[4:]) < 20 else 1
    return 2 if len(stem) == 1 and stem != "Q" else 3


def build(source, output):
    output.mkdir(parents=True, exist_ok=True)
    files = sorted((source / "Topics").glob("*.md"))
    topics = {}
    for path in files:
        body = path.read_text(encoding="utf-8-sig")
        topics[path.stem] = (body.splitlines()[0][2:], body)
    for required in ("START", "HELP", "PRINT", "PREF"):
        if required not in topics:
            raise ValueError(f"Missing topic {required}")
    def filename(topic):
        return "index.html" if topic == "START" else topic + ".html"
    def nav(current):
        groups = []
        for index, section in ((4, "SCORE Modern"), (0, "Item codes"), (1, "Keys and input stages"),
                               (2, "Letter commands"), (3, "Other references")):
            members = [stem for stem in topics if stem != "START" and category(stem) == index]
            attrs = " open" if index==4 or current in members else ""
            items = "".join(f'<li><a href="{filename(stem)}"'
                            + (' aria-current="page"' if stem == current else '')
                            + f'>{escape(topics[stem][0])}</a></li>' for stem in members)
            groups.append(f"<details{attrs}><summary>{section}</summary><ul>{items}</ul></details>")
        return "".join(groups)
    search = []
    for stem, (title, body) in topics.items():
        for link in re.findall(r"\]\(([A-Z0-9]+)\.md\)", body):
            if link not in topics:
                raise ValueError(f"Broken topic link {stem} → {link}")
        search.append([filename(stem), title, body])
        page = f"""<!doctype html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>{escape(title)} | SCORE Modern</title><link rel="stylesheet" href="help.css">
<script defer src="search-data.js"></script><script defer src="search.js"></script></head>
<body><a class="skip" href="#topic">Skip to topic</a>
<header><a href="index.html">SCORE Modern Help</a><form role="search"><label for="query">Search every topic</label>
<input id="query" type="search" placeholder="Try ‘slur P8’ or ‘G command’" autocomplete="off"></form></header>
<div class="layout"><nav aria-label="Help topics"><a class="start" href="index.html">Start here</a>{nav(stem)}</nav>
<main><section id="results" hidden aria-label="Search results"></section><article id="topic">{render(body)}</article></main></div></body></html>"""
        (output / filename(stem)).write_text(page, encoding="utf-8")
    (output / "search-data.js").write_text("const helpTopics = " + json.dumps(search) + ";\n", encoding="utf-8")
    for asset in ("help.css", "search.js"):
        shutil.copyfile(source / asset, output / asset)


if __name__ == "__main__":
    build(Path(__file__).resolve().parent, Path(sys.argv[1]))
