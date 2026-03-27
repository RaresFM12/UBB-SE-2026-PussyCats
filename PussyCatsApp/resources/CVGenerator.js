/**
 * cv-generator.js
 * ─────────────────────────────────────────────────────────────────
 * Populates CVHtmlTemplate.html with real user data and triggers
 * a PDF download — all client-side, no server required.
 * ─────────────────────────────────────────────────────────────────
 */

const CVGenerator = (() => {

    const TEMPLATE_PATH = 'CVHtmlTemplate.html';  

    // ─────────────────────────────────────────────────────────────
    // SKILL GROUP LABELS
    // Used to group skill tags into labelled sections.
    // ─────────────────────────────────────────────────────────────
    const SKILL_GROUPS = {
        Languages: ['JS', 'TS', 'Python', 'Java', 'C#', 'C++', 'Go', 'Rust', 'PHP', 'Ruby', 'Swift', 'Kotlin', 'Scala'],
        Frameworks: ['React', 'Angular', 'Vue.js', 'Next.js', 'ASP.NET Core', 'Spring Boot', 'Django', 'Flask', 'FastAPI', 'Node.js'],
        'DevOps / Cloud': ['Docker', 'Kubernetes', 'Git', 'CI/CD', 'Azure', 'AWS', 'GCP', 'Linux', 'Bash', 'Terraform'],
        Databases: ['SQL Server', 'PostgreSQL', 'MySQL', 'MongoDB', 'Redis', 'Oracle'],
        'Data & AI': ['ML', 'Deep Learning', 'TensorFlow', 'PyTorch', 'Pandas', 'NumPy', 'Tableau', 'Power BI'],
        Design: ['Figma', 'Adobe XD', 'UI/UX', 'Wireframing', 'Prototyping'],
        'Soft Skills': ['Teamwork', 'Communication', 'Problem Solving', 'Leadership', 'Time Management'],
        Other: [],  // catches anything not matched above
    };

    // ─────────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────────

    function esc(str) {
        if (!str) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    /** True when value is a non-empty string, a non-empty array, or any truthy value. */
    function hasValue(v) {
        if (v === null || v === undefined) return false;
        if (Array.isArray(v)) return v.length > 0;
        if (typeof v === 'string') return v.trim() !== '';
        return Boolean(v);
    }

    /**
     * Processes {{#if TOKEN}} … {{/if}} and {{#if TOKEN}} … {{else}} … {{/if}} blocks.
     * - If the token is truthy  → renders the "if" branch, removes the "else" branch.
     * - If the token is falsy   → removes the "if" branch, renders the "else" branch (if present).
     *
     * The {{else}} variant must be handled FIRST because its regex is more specific.
     */
    function processConditionals(html, data) {
        // 1. Handle {{#if}} … {{else}} … {{/if}}
        html = html.replace(
            /\{\{#if (\w+)\}\}([\s\S]*?)\{\{else\}\}([\s\S]*?)\{\{\/if\}\}/g,
            (_, key, ifBranch, elseBranch) =>
                hasValue(data[key]) ? ifBranch : elseBranch
        );

        // 2. Handle plain {{#if}} … {{/if}} (no else branch)
        html = html.replace(
            /\{\{#if (\w+)\}\}([\s\S]*?)\{\{\/if\}\}/g,
            (_, key, inner) => hasValue(data[key]) ? inner : ''
        );

        return html;
    }

    /** Replace all remaining {{TOKEN}} placeholders with their values. */
    function replaceTokens(html, data) {
        return html.replace(/\{\{(\w+)\}\}/g, (_, key) =>
            data[key] !== undefined ? String(data[key]) : ''
        );
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION RENDERERS
    // Each returns an HTML string injected in place of a list token.
    // ─────────────────────────────────────────────────────────────

    /**
     * EducationList
     * Input shape per entry:
     *   { university, programName, startYear, endYear, courses, description }
     *
     * courses can be a string or string[] — both are handled.
     */
    function renderEducation(entries = []) {
        if (!entries.length) return '';
        return entries.map(e => {
            const coursesText = Array.isArray(e.courses)
                ? e.courses.join(', ')
                : (e.courses || '');
            return `
<div class="edu-entry">
  <div class="edu-line">
    ${e.university ? `<span class="edu-university">${esc(e.university)}</span>` : ''}
    <span class="edu-divider"></span>
    <span class="edu-period">${esc(e.startYear || '')} – ${esc(e.endYear || '')}</span>
  </div>
  ${e.programName ? `<div class="edu-program">${esc(e.programName)}</div>` : ''}
  ${coursesText ? `<p class="edu-courses"><span class="edu-label">Relevant Courses:</span> ${esc(coursesText)}</p>` : ''}
  ${e.description ? `<p class="edu-description">${esc(e.description)}</p>` : ''}
</div>`;
        }).join('');
    }

    /**
     * WorkExperienceList
     * Input shape per entry:
     *   { jobTitle, company, startDate, endDate, description }
     */
    function renderWorkExperience(entries = []) {
        if (!entries.length) return '';
        return entries.map(e => `
<div class="work-entry">
  <div class="work-line">
    ${e.jobTitle ? `<span class="work-role">${esc(e.jobTitle)}</span>` : ''}
    ${e.company ? `<span class="work-company">${esc(e.company)}</span>` : ''}
    <span class="work-divider"></span>
    <span class="work-period">${esc(e.startDate || '')} – ${esc(e.endDate || 'Present')}</span>
  </div>
  ${e.description ? `<p class="work-description">${esc(e.description)}</p>` : ''}
</div>`).join('');
    }

    /**
     * ProjectsList
     * Input shape per entry:
     *   { name, description, technologies: string[], startYear, endYear, url }
     */
    function renderProjects(entries = []) {
        if (!entries.length) return '';
        return entries.map(p => `
<div class="project-entry-custom">
  <div class="project-line">
    ${p.name ? `<span class="project-title">${esc(p.name)}</span>` : ''}
    <span class="project-divider"></span>
    <span class="project-period">${esc(p.startYear || '')}${p.endYear ? ` – ${esc(p.endYear)}` : ''}</span>
  </div>
  ${p.technologies?.length ? `<div class="project-tech">${p.technologies.map(esc).join(' · ')}</div>` : ''}
  ${p.description ? `<p class="project-description">${esc(p.description)}</p>` : ''}
  ${p.url ? `<div style="font-size:7.5pt;margin-top:1px"><a href="${esc(p.url)}" style="color:#2d6a9f">${esc(p.url)}</a></div>` : ''}
</div>`).join('');
    }

    /**
     * ExtraCurricularList
     * Input shape per entry:
     *   { activityName, organisation, role, period, description }
     */
    function renderExtraCurricular(entries = []) {
        if (!entries.length) return '';
        return entries.map(a => `
<div class="extra-entry">
  <div class="extra-title">${esc(a.activityName)}</div>
  <div class="extra-meta">
    ${[a.organisation, a.role, a.period].filter(Boolean).map(esc).join(' · ')}
  </div>
  ${a.description ? `<div class="extra-desc">${esc(a.description)}</div>` : ''}
</div>`).join('');
    }

    /**
     * SkillsList — groups user skills into labelled sections with pill tags.
     * Input: string[]
     */
    function renderSkills(skills = []) {
        if (!skills.length) return '';

        const buckets = {};

        skills.map(s => s.trim()).forEach(skill => {
            let placed = false;
            for (const [group, keywords] of Object.entries(SKILL_GROUPS)) {
                if (group === 'Other') continue;
                if (keywords.some(k => k.toLowerCase() === skill.toLowerCase())) {
                    buckets[group] = buckets[group] || [];
                    buckets[group].push(skill);
                    placed = true;
                    break;
                }
            }
            if (!placed) {
                buckets['Other'] = buckets['Other'] || [];
                buckets['Other'].push(skill);
            }
        });

        return Object.entries(buckets).map(([label, tags]) => `
<div class="skill-group">
  <div class="skill-group-label">${esc(label)}</div>
  <div class="skill-tags">
    ${tags.map(t => `<span class="skill-tag">${esc(t)}</span>`).join('')}
  </div>
</div>`).join('');
    }

    /**
     * CertificatesList
     * Input shape per entry:  { name, uploadDate }
     */
    function renderCertificates(certs = []) {
        if (!certs.length) return '';
        return certs.map(c =>
            `<li>${esc(c.name)}${c.uploadDate
                ? ` <span style="color:#5a6a7a;font-size:7.5pt">(${esc(c.uploadDate)})</span>`
                : ''}</li>`
        ).join('');
    }

    // ─────────────────────────────────────────────────────────────
    // DATA → FLAT TOKEN MAP
    // Maps the structured userProfile to the flat key→value dict
    // that the template's {{tokens}} and {{#if}} blocks consume.
    // ─────────────────────────────────────────────────────────────
    function buildTokenMap(profile) {
        return {
            // ── Scalar header fields ──────────────────────────────
            FirstName: esc(profile.firstName),
            LastName: esc(profile.lastName),
            Email: esc(profile.email),
            PhoneNumber: esc(profile.phoneNumber),
            City: esc(profile.city),
            Country: esc(profile.country),
            GitHub: esc(profile.github),
            LinkedIn: esc(profile.linkedin),

            // ── Section presence flags (drive {{#if}} blocks) ─────
            // Passing the raw array lets hasValue() evaluate correctly.
            Education: profile.education,
            WorkExperience: profile.workExperience,
            Projects: profile.projects,
            ExtraCurricular: profile.extraCurricular,
            Certificates: profile.certificates,
            Skills: profile.skills,

            // ── Rendered HTML list blocks ─────────────────────────
            // These replace the {{XxxList}} tokens inside sections.
            // A non-empty rendered string also counts as truthy for
            // the {{#if EducationList}} … {{else}} … {{/if}} pattern,
            // so the {{else}} fallback (single-field placeholders) is
            // automatically suppressed when we inject a full list.
            EducationList: renderEducation(profile.education),
            WorkExperienceList: renderWorkExperience(profile.workExperience),
            ProjectsList: renderProjects(profile.projects),
            ExtraCurricularList: renderExtraCurricular(profile.extraCurricular),
            CertificatesList: renderCertificates(profile.certificates),
            SkillsList: renderSkills(profile.skills),
        };
    }

    // ─────────────────────────────────────────────────────────────
    // PUBLIC API
    // ─────────────────────────────────────────────────────────────

    /**
     * generate(userProfile)
     *
     * Fetches the template, injects user data, opens the populated
     * HTML in a hidden iframe, and lets CVJSTemplate.js trigger the
     * PDF download automatically — nothing is saved server-side.
     *
     * @param {Object} userProfile  — see example shape below
     */
    async function generate(userProfile) {
        // 1. Fetch the HTML template as raw text
        let templateHtml;
        try {
            const resp = await fetch(TEMPLATE_PATH);
            if (!resp.ok) throw new Error(`Template fetch failed: ${resp.status}`);
            templateHtml = await resp.text();
        } catch (err) {
            alert('Could not load CV template. Please try again.');
            console.error(err);
            return;
        }

        // 2. Build the flat token map from the profile object
        const tokens = buildTokenMap(userProfile);

        // 3. Resolve {{#if}} … {{else}} … {{/if}} conditional blocks
        let populated = processConditionals(templateHtml, tokens);

        // 4. Replace every remaining {{TOKEN}} placeholder with its value
        populated = replaceTokens(populated, tokens);

        // 5. Inject an auto-trigger so generatePDF() fires on load.
        //    CVJSTemplate.js (already linked in the template <head>)
        //    defines generatePDF(); we just need to call it once loaded.
        const autoTrigger = populated.replace(
            /<\/body>/i,
            `<script>window.addEventListener('load', generatePDF);<\/script></body>`
        );

        // 6. Turn the populated HTML into a blob URL and load it in a
        //    hidden iframe — the iframe runs generatePDF() automatically,
        //    which downloads the PDF straight to the user's device.
        const blob = new Blob([autoTrigger], { type: 'text/html' });
        const url = URL.createObjectURL(blob);

        const iframe = document.createElement('iframe');
        iframe.style.cssText =
            'position:fixed;width:0;height:0;border:none;opacity:0;pointer-events:none';
        document.body.appendChild(iframe);
        iframe.src = url;

        // 7. Clean up after 60 s (download should be complete by then)
        setTimeout(() => {
            document.body.removeChild(iframe);
            URL.revokeObjectURL(url);
        }, 60_000);
    }

    return { generate };

})();
