#!/bin/sh
# Rebuilds docs/Poliekran-guide-*.pdf from the Markdown guides.
# Needs pandoc 3+ and Chromium or Chrome (set CHROME=/path/to/chrome if it is not on PATH).
set -e
cd "$(dirname "$0")"
CHROME="${CHROME:-$(command -v chromium || command -v chromium-browser || command -v google-chrome)}"
LOGO="$(base64 -w0 ../assets/icon.png)"
for l in ru en; do
  pandoc "guide-$l.md" -s --metadata pagetitle="Poliekran" --css guide.css --embed-resources -o "guide-$l.html"
  sed -i -e 's#<h1 id="[^"]*">#&<img class="logo" src="data:image/png;base64,'"$LOGO"'">#' \
         -e 's#<a href="guide-..\.md">\([^<]*\)</a>#\1#g' "guide-$l.html"
  "$CHROME" --headless --disable-gpu --no-sandbox --no-pdf-header-footer --print-to-pdf="Poliekran-guide-$l.pdf" "guide-$l.html" 2>/dev/null
  rm -f "guide-$l.html"
done
