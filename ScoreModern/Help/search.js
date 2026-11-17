"use strict";
const query = document.getElementById("query");
const topic = document.getElementById("topic");
const results = document.getElementById("results");
function search() {
    const words = query.value.trim().toLowerCase().split(/\s+/).filter(Boolean);
    topic.hidden = words.length > 0;
    results.hidden = !topic.hidden;
    if (results.hidden) return;
    const matches = helpTopics.map(([url, title, body]) => {
        const text = body.toLowerCase();
        if (!words.every(word => text.includes(word))) return null;
        const score = words.filter(word => title.toLowerCase().includes(word)).length;
        const snippet = body.split("\n").find(line => !line.startsWith("#") &&
            words.every(word => line.toLowerCase().includes(word.trim()))) || "";
        return {url, title, score, snippet};
    }).filter(Boolean).sort((a, b) => b.score - a.score || a.title.localeCompare(b.title));
    results.replaceChildren();
    const heading = document.createElement("h1");
    heading.textContent = matches.length ? "Search results" : "No matching topics";
    results.append(heading);
    const count = document.createElement("p");
    count.setAttribute("role", "status");
    count.textContent = matches.length === 1 ? "1 topic" : matches.length + " topics";
    results.append(count);
    const list = document.createElement("ul");
    for (const match of matches) {
        const item = document.createElement("li");
        const link = document.createElement("a");
        link.href = match.url;
        link.textContent = match.title;
        item.append(link);
        if (match.snippet) {
            const snippet = document.createElement("p");
            snippet.textContent = match.snippet.slice(0, 200);
            item.append(snippet);
        }
        list.append(item);
    }
    results.append(list);
}
query.addEventListener("input", search);
query.form.addEventListener("submit", event => { event.preventDefault(); search(); });
query.addEventListener("keydown", event => {
    if (event.key === "Escape" && query.value) { query.value = ""; search(); }
});
