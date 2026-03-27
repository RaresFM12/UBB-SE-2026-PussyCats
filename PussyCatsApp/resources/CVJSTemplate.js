/**
 * CVJSTemplate.js
 * ─────────────────────────────────────────────────────────────────
 * Handles PDF generation from the populated CV HTML template.
 * Loaded by CVHtmlTemplate.html via <script src="CVJSTemplate.js">.
 *
 * generatePDF() is called either:
 *   - Manually: when the user clicks "Download CV as PDF"
 *   - Automatically: by cv-generator.js after data injection
 *     (it appends window.addEventListener('load', generatePDF))
 * ─────────────────────────────────────────────────────────────────
 */

function generatePDF() {
    const btn = document.getElementById('download-btn');
    const spinner = document.getElementById('btn-spinner');
    const label = document.getElementById('btn-label');
    const element = document.getElementById('cv-content');

    // Show loading state (R52 — loading spinner while generating)
    if (btn) btn.disabled = true;
    if (spinner) spinner.style.display = 'block';
    if (label) label.textContent = 'Generating…';

    // Build the file name from the rendered <h1> (R56 — FirstName LastName CV.pdf)
    const h1Text = element.querySelector('h1')?.textContent?.trim() || 'User CV';
    const fileName = h1Text + ' CV.pdf';

    const opt = {
        margin: 0,                      // margins are handled entirely in CSS
        filename: fileName,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: {
            scale: 2,              // retina-quality rendering
            useCORS: true,
            letterRendering: true,
        },
        jsPDF: {
            unit: 'mm',
            format: 'a4',
            orientation: 'portrait',
        },
        pagebreak: { mode: ['avoid-all', 'css'] },  // R55 — no section split across pages
    };

    html2pdf()
        .set(opt)
        .from(element)
        .save()
        .then(() => {
            // Reset button state
            if (btn) btn.disabled = false;
            if (spinner) spinner.style.display = 'none';
            if (label) label.textContent = '⬇ Download CV as PDF';
        })
        .catch(() => {
            // R57 — user-friendly error message
            if (btn) btn.disabled = false;
            if (spinner) spinner.style.display = 'none';
            if (label) label.textContent = '⬇ Download CV as PDF';
            alert('PDF generation failed. Please try again or use a different browser.');
        });
}